#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PageEditing
{
	/**
	 * Implements an image recognition algorithm
	 * to find the music systems on the page
	 * 
	 * It works roughly as follows:
	 * 1. Move a rectangular line pattern vertically over the page
	 * to find "line candidates" with low average brightness
	 * 2. Find local minima inside the resulting candidates' brightness and throw away the others
	 * 3. Group individual line candidates depending on their vertical distance
	 * 4. Throw away groups that have too view or too many lines
	 * 5. Now try to find the left and right endings of the group of lines. This is done
	 * in a way that tolerates when the lines are not strictly horizontal.
	 * 6. Finally when we have found the musical systems, calculate the position
	 * and size of corresponding enclosing boxes so that the vertical space 
	 * between the systems are optimal
	 */
	internal class BoxFinder
	{
		private class LinePattern
		{
			public readonly double MeanBrightness;
			public readonly Rect Rect;

			public LinePattern(Rect r, double mean)
			{
				Rect = r;
				MeanBrightness = mean;
			}

			public override string ToString()
			{
				return Rect + "(" + MeanBrightness + ")";
			}
		}

		private const double MAX_LINE_DISTANCE = 0.015D;
		private const double MIN_LINE_DISTANCE = 0.001D;
		private const double LINE_PATTERN_WIDTH = 0.1D;
		private const double SMALL_LINE_PATTERN_WIDTH = 0.01D;
		private const double LINE_PATTERN_HEIGHT = 0.0004D;
		private const double LINE_BRIGHTNESS_THRESHOLD = 0.5D;
		private const double EDGE_BRIGHTNESS_THRESHOLD = 0.85;
		private const double DEFAULT_BOX_HEIGHT = 0.1D;
		private const double MAX_BOX_HEIGHT = DEFAULT_BOX_HEIGHT * 1.3D;
		private const double MIN_SYSTEM_DISTANCE = 3D * MAX_LINE_DISTANCE;

		private readonly IGreyImage _image;

		private int LinePatternWidth => (int) Math.Round(_image.Width * LINE_PATTERN_WIDTH);

		private int SmallLinePatternWidth => (int) Math.Round(_image.Width * SMALL_LINE_PATTERN_WIDTH);

		private int LinePatternHeight => (int) Math.Ceiling(_image.Height * LINE_PATTERN_HEIGHT);

		private int MaxLineDistance => (int) Math.Round(_image.Height * MAX_LINE_DISTANCE);

		private int MinLineDistance => (int) Math.Round(_image.Height * MIN_LINE_DISTANCE);

		public double DefaultBoxHeight => _image.Height * DEFAULT_BOX_HEIGHT;

		public double MinSystemDistance => _image.Height * MIN_SYSTEM_DISTANCE;

		public double MaxBoxHeight => _image.Height * MAX_BOX_HEIGHT;

		public BoxFinder(IGreyImage image)
		{
			_image = image;
		}

		/** 
         * tries to locate all note lines on the current image
         * and returns a list of rectangles that enclose each line
         * result is in screen coordinates
         */
		public IEnumerable<Rect> Execute()
		{
			List<Rect> systems = DetectSystems().ToList();
			IEnumerable<Rect> boxes = GetBoxes(systems);

			//@todo p2 where can we get the 96 dpi from?
			//maybe from the paint graphics object?
			double xFactor = 96D / _image.DpiX;
			double yFactor = 96D / _image.DpiY;

			return boxes.Select(s => s.Scale(xFactor, yFactor));
		}

		public IEnumerable<Rect> GetBoxes(IEnumerable<Rect> systems)
		{
			List<Rect> boxes = new List<Rect>();
			int systemsCount = systems.Count();
			if (systemsCount == 0) return boxes;

			Rect last = null;
			List<Rect> systemList = systems.ToList();

			for (int i = 0; i < systemList.Count; i++)
			{
				Rect current = systemList[i];
				Rect next = null;
				if (i < systemList.Count - 1) next = systemList[i + 1];

				double defaultSpacing = DefaultBoxHeight / 2;

				double spaceAbove = Math.Min(current.Top, defaultSpacing);
				if (last != null)
					spaceAbove = Math.Min(
						(current.Top - last.Bottom) / 2D - 2D * Page.ClickTolerance,
						MaxBoxHeight / 2);

				double spaceBelow = Math.Min(_image.Height - current.Bottom - 1, defaultSpacing);
				if (next != null)
					spaceBelow = Math.Min(
						(next.Top - current.Bottom) / 2D - 2D * Page.ClickTolerance,
						MaxBoxHeight / 2);

				//boxes.Add(new Rect(current.X, current.Y - spaceAbove,
				//    current.Width, current.Height + spaceAbove + spaceBelow));
				boxes.Add(current.AddAbove(spaceAbove).AddBelow(spaceBelow));

				last = systemList[i];
			}

			return boxes;
		}

		public IEnumerable<Rect> DetectSystems()
		{
			IEnumerable<LinePattern> lineCandidates = GetLineCandidates();
			IEnumerable<LinePattern> skeletons = SkeletonizeLines(lineCandidates);
			List<IEnumerable<Rect>> groupedLines = GetGroupedLines(skeletons).ToList();
			IEnumerable<IEnumerable<Rect>> plausibleGroups = GetPlausibleGroups(groupedLines);

			IEnumerable<IEnumerable<Rect>> withEndings =
				from g in plausibleGroups
				select FindLineEndings(g);

			return from system in withEndings
				let first = system.First()
				let last = system.Last()
				select first.AddBelow(last.Bottom - first.Bottom);
		}

		private IEnumerable<IEnumerable<Rect>> GetPlausibleGroups(List<IEnumerable<Rect>> groupedLines)
		{
			List<IEnumerable<Rect>> groupsWithRightDistance = new List<IEnumerable<Rect>>();
			for (int i = 0; i < groupedLines.Count; i++)
			{
				Rect lastLineOfCurrent = groupedLines[i].Last();
				if (i + 1 < groupedLines.Count)
				{
					Rect firstLineOfNext = groupedLines[i + 1].First();
					double groupDistance = firstLineOfNext.Top - lastLineOfCurrent.Bottom;

					if (groupDistance > MinSystemDistance)
					{
						groupsWithRightDistance.Add(groupedLines[i]);
					}
					else
					{
						//only take the system with the max number of lines
						if (groupedLines[i].Count() > groupedLines[i + 1].Count())
							groupsWithRightDistance.Add(groupedLines[i]);
						else
							groupsWithRightDistance.Add(groupedLines[++i]);
					}
				}
				else
				{
					groupsWithRightDistance.Add(groupedLines[i]);
				}
			}

			//it can easily happen that there are more lines found when
			//@todo p3 for example there is a "Achtelbalken" right above a system
			//could be improved by a check inside the FindLineEndings method
			IEnumerable<IEnumerable<Rect>> groupsWithRightCount = from g in groupsWithRightDistance
				where g.Count() >= 3 && g.Count() <= 8
				select g;

			return groupsWithRightCount;
		}

		private IEnumerable<Rect> FindLineEndings(IEnumerable<Rect> system)
		{
			//move a smaller LinePattern horizontally until its content is "nearly white"
			//if no edge was found, we use the full image width
			//rotationCorrection is used to tolerate small rotations of the image
			//as follows: if we find that the small line pattern would better match
			//one pixel above or below, we increase rotationCorrection.

			//look left			
			bool edgeFound = false;
			int rotationCorrection = 0;
			int left;
			for (left = system.First().XInt; left > 0 && !edgeFound; left--)
				edgeFound = FindEndingAt(system, left, ref rotationCorrection);

			//look right
			edgeFound = false;
			rotationCorrection = 0;
			int right;
			for (right = system.First().RightInt; right < _image.Width && !edgeFound; right++)
				edgeFound = FindEndingAt(system, right - SmallLinePatternWidth, ref rotationCorrection);

			return from r in system
				select new Rect(left, r.Y, right - left, r.Height);
		}

		private bool FindEndingAt(IEnumerable<Rect> system, int x, ref int rotationCorrection)
		{
			double mean = GetSystemBrightnessAt(system, x, rotationCorrection);
			double meanAbove = GetSystemBrightnessAt(system, x, rotationCorrection - 1);
			double meanBelow = GetSystemBrightnessAt(system, x, rotationCorrection + 1);

			//it is intended that we don't change rotationCorrection when
			//meanAbove and meanBelow are lower than mean but equal
			double darkest = 1D;
			if (meanAbove < mean && meanAbove < meanBelow)
			{
				darkest = meanAbove;
				rotationCorrection -= 1;
			}
			else if (meanBelow < mean && meanBelow < meanAbove)
			{
				darkest = meanBelow;
				rotationCorrection += 1;
			}
			else
			{
				darkest = mean;
			}

			return darkest >= EDGE_BRIGHTNESS_THRESHOLD;
		}

		private double GetSystemBrightnessAt(IEnumerable<Rect> system, int x, int rotationCorrection)
		{
			double result = 1D;
			double count = 0D;
			foreach (Rect orig in system)
			{
				double y = orig.Y + rotationCorrection;
				if (y >= 0 && y < _image.Height)
				{
					Rect smallLinePattern = new Rect(x, y, SmallLinePatternWidth, LinePatternHeight);
					result += GetMeanBrightness(smallLinePattern);
					count += 1D;
				}
			}

			return result / count;
		}

		//move a "LinePattern" over the middle of the page and find rows with
		//low brightness			
		private IEnumerable<LinePattern> GetLineCandidates()
		{
			List<LinePattern> lineCandidates = new List<LinePattern>();
			int x = (_image.Width - LinePatternWidth) / 2;
			for (int y = 0; y <= _image.Height - LinePatternHeight; y++)
			{
				Rect linePattern = new Rect(x, y, LinePatternWidth, LinePatternHeight);
				double mean = GetMeanBrightness(linePattern);
				if (mean < LINE_BRIGHTNESS_THRESHOLD) lineCandidates.Add(new LinePattern(linePattern, mean));
			}

			return lineCandidates;
		}

		//put lines that are near to each other into a group
		private IEnumerable<IEnumerable<Rect>> GetGroupedLines(IEnumerable<LinePattern> skeletons)
		{
			List<IEnumerable<Rect>> groupedLines = new List<IEnumerable<Rect>>();
			LinePattern lastLine = null;
			List<Rect> currentPack = new List<Rect>();
			foreach (LinePattern currentLine in skeletons)
			{
				if (lastLine != null)
					if (currentLine.Rect.Y - lastLine.Rect.Y > MaxLineDistance)
					{
						groupedLines.Add(currentPack);
						currentPack = new List<Rect>();
					}

				lastLine = currentLine;
				currentPack.Add(currentLine.Rect);
			}

			if (currentPack.Count > 0)
				//can happen if no line candidates
				groupedLines.Add(currentPack);
			return groupedLines;
		}

		private IEnumerable<LinePattern> SkeletonizeLines(IEnumerable<LinePattern> lineCandidates)
		{
			//throw away all too near lines, prefer the one with best brightness
			//We take all line candidates that have Y value in a range of
			// [currentY - [currentY + MIN_LINE_DISTANCE] and elect the one with
			// maximum mean brightness
			List<LinePattern> candidateList = lineCandidates.ToList();
			List<LinePattern> selectedCandidates = new List<LinePattern>();

			for (int i = 0; i < candidateList.Count; i++)
			{
				LinePattern currentLine = candidateList[i];

				while (i < candidateList.Count - 1)
				{
					LinePattern nextLine = candidateList[i + 1];
					if (nextLine.Rect.Y > currentLine.Rect.Y + MinLineDistance) break;
					if (nextLine.MeanBrightness < currentLine.MeanBrightness) currentLine = nextLine;
					i++;
				}

				selectedCandidates.Add(currentLine);
			}

			return selectedCandidates;
		}

		private double GetMeanBrightness(Rect r)
		{
			double brightness = 0F;

			for (int i = r.XInt; i < r.Right; i++)
			for (int j = r.YInt; j < r.Bottom; j++)
				brightness += _image.Brightness(i, j);
			double mean = brightness / (r.Width * r.Height);
			Trace.Assert(mean >= 0F && mean <= 1F);
			return mean;
		}
	}
}