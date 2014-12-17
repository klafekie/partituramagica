using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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
			public int partIndex;//index of the parts, 0 corresponds to part "A"
			public int clippingIndex;//index of the clipping relative to the corresponding part
			public int patternIndex;//the position inside the part pattern, 
			public int lineIndex;//the position inside the Navigator.lines list
			public double xPos;//the absolute xPosition of the line on the view

			public LinePartInfo(int partIndex, int clippingIndex, int patternIndex, int lineIndex, double xPos) {
				this.partIndex = partIndex;
				this.clippingIndex = clippingIndex;
				this.patternIndex = patternIndex;
				this.lineIndex = lineIndex;
				this.xPos = xPos;
			}			
		}

        private static readonly double MIN_PART_SCROLL = 50D;
        private string partPattern;
        //the original parts as configured by the user
        private IList<Part> parts;
        private List<LinePartInfo> lines;
        private IPlayerView view;

        public Navigator(string partPattern, IList<Part> parts, IPlayerView view) {
            this.view = view;
            //note that we allow that not all labelled parts occur in the pattern.
            if (String.IsNullOrEmpty(partPattern)) {
                partPattern = "A";
            }
            partPattern.Trim();
            this.partPattern = partPattern;
            this.parts = parts;
            CreateLines();
            SetupEvents(true);
        }

        private int GetPartIndex(int i) {
            return (int)(this.partPattern[i] - 'A');
        }

		private void CreateLines() {
			this.lines = new List<LinePartInfo>();
			double currentX = 0D;
			int lineIndex = 0;
			for (int patternIndex = 0; patternIndex < this.partPattern.Length; patternIndex++) {
				Part p = this.parts[GetPartIndex(patternIndex)];
				for (int clippingIndex = 0; clippingIndex < p.Count; clippingIndex++) {
					this.lines.Add(
						new LinePartInfo(GetPartIndex(patternIndex), 
							clippingIndex, patternIndex, lineIndex, currentX));
					currentX += p[clippingIndex].Coords.Width;
					lineIndex++;
				}				
			}
		}

        public void Start() {

			List<Clipping> clips = new List<Clipping>();

			foreach (LinePartInfo i in this.lines) {
				clips.Add(new Clipping(this.parts[i.partIndex][i.clippingIndex]));
			}			

			//make the clippings a bit taller, cause the lines might
			//be very near to each other
			foreach (Clipping c in clips) {
				//this is safe, cause the outerrect is already set to page coordinates.
				Debug.Assert(c.Coords.OuterRect != null);
				c.AddAboveAndBelow(Page.ClickTolerance);
			}                

            this.view.Init(this.partPattern, clips);
            this.view.StartScrolling();
        }

        private void PartClicked(int partIndex) {
            this.view.GotoLine(PatternIndexToLineInfo(partIndex).lineIndex);
        }

        private LinePartInfo PatternIndexToLineInfo(int patternIndex) {
			return (from l in this.lines
					where l.patternIndex == patternIndex
					select l).First();
        }

        private void PreviousPartClicked(ref bool cancel) {

            int patternIndex = this.view.PartNavigator.Current;
			double currentPartStartPos = PatternIndexToLineInfo(patternIndex).xPos;

            //if we have scrolled only a bit, go back to previous part, otherwise restart current part
            cancel |= (view.XPos > currentPartStartPos + MIN_PART_SCROLL);
        }

        private void LineEndReached(int line, ref bool cancel) {
			int patternIndex = 0;
			if (line + 1 < this.lines.Count) {
				patternIndex = this.lines[line + 1].patternIndex;
			} else {
				cancel = true;
			}

            if (!cancel) {
                this.view.PartNavigator.Current = patternIndex;
            }
        }

        public void Dispose() {
            SetupEvents(false);
        }

        private void SetupEvents(bool add) {
            if (add) {
                this.view.PartNavigator.ChoiceClicked += this.PartClicked;
                this.view.PartNavigator.PreviousChoiceClicked += this.PreviousPartClicked;
                this.view.LineEndReached += this.LineEndReached;
            }
            else {
                this.view.PartNavigator.ChoiceClicked -= this.PartClicked;
                this.view.PartNavigator.PreviousChoiceClicked -= this.PreviousPartClicked;
                this.view.LineEndReached -= this.LineEndReached;
            }
        }
    }
}
