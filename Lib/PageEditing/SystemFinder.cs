#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
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
	 */
	public class SystemFinder
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

		private const double LINE_BRIGHTNESS_THRESHOLD = 0.5D;
		private const double EDGE_BRIGHTNESS_THRESHOLD = 0.85;

		private readonly IGreyImage _image;

		private int LinePatternWidth => (int) Math.Round(_image.Width * 0.1D);

		private int SmallLinePatternWidth => (int) Math.Round(_image.Width * 0.01D);

		private int LinePatternHeight => (int) Math.Ceiling(_image.Height * 0.0004D);

		private int MaxLineDistance => (int) Math.Round(_image.Height * 0.015D);

		private int MinLineDistance => (int) Math.Round(_image.Height * 0.001D);

		public double MinSystemDistance => _image.Height * 0.045D;

		[UsedImplicitly]
		public SystemFinder(IDrawingPanel panel)
		{
			_image = panel.CurrentImage;
		}

		public SystemFinder(IGreyImage image)
		{
			_image = image;
		}

		public IList<Rect> DetectSystems()
		{
			IEnumerable<LinePattern> lineCandidates = GetLineCandidates();
			IEnumerable<LinePattern> skeletons = SkeletonizeLines(lineCandidates);
			var groupedLines = GetGroupedLines(skeletons).ToList();
			var plausibleGroups = GetPlausibleGroups(groupedLines).ToList();

			var withEndings = plausibleGroups.Select(FindLineEndings).ToList();

			return (from system in withEndings
				let first = system.First()
				let last = system.Last()
				select first.AddBelow(last.Bottom - first.Bottom)).ToList();
		}

		private List<List<Rect>> GetPlausibleGroups(List<List<Rect>> groupedLines)
		{
			List<List<Rect>> groupsWithRightDistance = new List<List<Rect>>();
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
						if (groupedLines[i].Count > groupedLines[i + 1].Count)
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

			var groupsWithRightCount = groupsWithRightDistance
				.Where(g => g.Count >= 3 && g.Count <= 8)
				.ToList();


			return groupsWithRightCount;
		}

		private IEnumerable<Rect> FindLineEndings(List<Rect> system)
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

		private bool FindEndingAt(List<Rect> system, int x, ref int rotationCorrection)
		{
			double mean = GetSystemBrightnessAt(system, x, rotationCorrection);
			double meanAbove = GetSystemBrightnessAt(system, x, rotationCorrection - 1);
			double meanBelow = GetSystemBrightnessAt(system, x, rotationCorrection + 1);

			//it is intended that we don't change rotationCorrection when
			//meanAbove and meanBelow are lower than mean but equal
			double darkest;
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
		private List<List<Rect>> GetGroupedLines(IEnumerable<LinePattern> skeletons)
		{
			var groupedLines = new List<List<Rect>>();
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
			//Trace.Assert(mean >= 0F && mean <= 1F);
			return mean;
		}
	}
}