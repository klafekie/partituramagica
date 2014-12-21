using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PartituraMagica.Playing
{
	//@todo p2 since we use InternalsVisibleTo("UnitTests")
	//many classes can be made "internal"

	/** 
	 * Implements navigation inside the parts, pages, lines
	 * of a partitura. This is not trivial, because when going to a
	 * certain page it is not clear how to change the "current part"
	 */

	public class Navigator : IDisposable
	{
		private class LinePartInfo
		{
			public readonly int partIndex; //index of the parts, 0 corresponds to part "A"
			public readonly int clippingIndex; //index of the clipping relative to the corresponding part
			public readonly int patternIndex; //the position inside the part pattern, 
			public readonly int lineIndex; //the position inside the Navigator.lines list
			public readonly double xPos; //the absolute xPosition of the line on the view

			public LinePartInfo(int partIndex, int clippingIndex, int patternIndex, int lineIndex, double xPos)
			{
				this.partIndex = partIndex;
				this.clippingIndex = clippingIndex;
				this.patternIndex = patternIndex;
				this.lineIndex = lineIndex;
				this.xPos = xPos;
			}
		}

		private static readonly double MIN_PART_SCROLL = 50D;
		private readonly string partPattern;
		//the original parts as configured by the user
		private readonly IList<Part> parts;
		private List<LinePartInfo> lines;
		private readonly IPlayerView view;

		public Navigator(string partPattern, IList<Part> parts, IPlayerView view)
		{
			this.view = view;
			//note that we allow that not all labelled parts occur in the pattern.
			if (String.IsNullOrEmpty(partPattern))
			{
				partPattern = "A";
			}
			partPattern.Trim();
			this.partPattern = partPattern;
			this.parts = parts;
			CreateLines();
			SetupEvents(true);
		}

		private int GetPartIndex(int i)
		{
			return partPattern[i] - 'A';
		}

		private void CreateLines()
		{
			lines = new List<LinePartInfo>();
			double currentX = 0D;
			int lineIndex = 0;
			for (int patternIndex = 0; patternIndex < partPattern.Length; patternIndex++)
			{
				Part p = parts[GetPartIndex(patternIndex)];
				for (int clippingIndex = 0; clippingIndex < p.Count; clippingIndex++)
				{
					lines.Add(
						new LinePartInfo(GetPartIndex(patternIndex),
							clippingIndex, patternIndex, lineIndex, currentX));
					currentX += p[clippingIndex].Coords.Width;
					lineIndex++;
				}
			}
		}

		public void Start()
		{
			List<Clipping> clips = new List<Clipping>();

			foreach (LinePartInfo i in lines)
			{
				clips.Add(new Clipping(parts[i.partIndex][i.clippingIndex]));
			}

			//make the clippings a bit taller, cause the lines might
			//be very near to each other
			foreach (Clipping c in clips)
			{
				//this is safe, cause the outerrect is already set to page coordinates.
				Debug.Assert(c.Coords.OuterRect != null);
				c.AddAboveAndBelow(Page.ClickTolerance);
			}

			view.Init(partPattern, clips);
			view.StartScrolling();
		}

		private void PartClicked(int partIndex)
		{
			view.GotoLine(PatternIndexToLineInfo(partIndex).lineIndex);
		}

		private LinePartInfo PatternIndexToLineInfo(int patternIndex)
		{
			return (from l in lines
				where l.patternIndex == patternIndex
				select l).First();
		}

		private void PreviousPartClicked(ref bool cancel)
		{
			int patternIndex = view.PartNavigator.Current;
			double currentPartStartPos = PatternIndexToLineInfo(patternIndex).xPos;

			//if we have scrolled only a bit, go back to previous part, otherwise restart current part
			cancel |= (view.XPos > currentPartStartPos + MIN_PART_SCROLL);
		}

		private void LineEndReached(int line, ref bool cancel)
		{
			int patternIndex = 0;
			if (line + 1 < lines.Count)
			{
				patternIndex = lines[line + 1].patternIndex;
			}
			else
			{
				cancel = true;
			}

			if (!cancel)
			{
				view.PartNavigator.Current = patternIndex;
			}
		}

		public void Dispose()
		{
			SetupEvents(false);
		}

		private void SetupEvents(bool add)
		{
			if (add)
			{
				view.PartNavigator.ChoiceClicked += PartClicked;
				view.PartNavigator.PreviousChoiceClicked += PreviousPartClicked;
				view.LineEndReached += LineEndReached;
			}
			else
			{
				view.PartNavigator.ChoiceClicked -= PartClicked;
				view.PartNavigator.PreviousChoiceClicked -= PreviousPartClicked;
				view.LineEndReached -= LineEndReached;
			}
		}
	}
}