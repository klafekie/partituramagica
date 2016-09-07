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
        private static readonly double MinPartScroll = 50D;
        private readonly string _partPattern;
        //the original parts as configured by the user
        private readonly IList<Part> _parts;
        private readonly IPlayerView _view;
        private List<LinePartInfo> _lines;

        public Navigator(string partPattern, IList<Part> parts, IPlayerView view)
        {
            this._view = view;
            //note that we allow that not all labelled parts occur in the pattern.
            if (string.IsNullOrEmpty(partPattern))
            {
                partPattern = "A";
            }
            partPattern.Trim();
            this._partPattern = partPattern;
            this._parts = parts;
            CreateLines();
            SetupEvents(true);
        }

        public void Dispose()
        {
            SetupEvents(false);
        }

        private int GetPartIndex(int i)
        {
            return _partPattern[i] - 'A';
        }

        private void CreateLines()
        {
            _lines = new List<LinePartInfo>();
            double currentX = 0D;
            int lineIndex = 0;
            for (int patternIndex = 0; patternIndex < _partPattern.Length; patternIndex++)
            {
                Part p = _parts[GetPartIndex(patternIndex)];
                for (int clippingIndex = 0; clippingIndex < p.Count; clippingIndex++)
                {
                    _lines.Add(
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

            foreach (LinePartInfo i in _lines)
            {
                clips.Add(new Clipping(_parts[i.PartIndex][i.ClippingIndex]));
            }

            //make the clippings a bit taller, cause the lines might
            //be very near to each other
            foreach (Clipping c in clips)
            {
                //this is safe, cause the outerrect is already set to page coordinates.
                Debug.Assert(c.Coords.OuterRect != null);
                c.AddAboveAndBelow(Page.ClickTolerance);
            }

            _view.Init(_partPattern, clips);
            _view.StartScrolling();
        }

        private void PartClicked(int partIndex)
        {
            _view.GotoLine(PatternIndexToLineInfo(partIndex).LineIndex);
        }

        private LinePartInfo PatternIndexToLineInfo(int patternIndex)
        {
            return (from l in _lines
                where l.PatternIndex == patternIndex
                select l).First();
        }

        private void PreviousPartClicked(ref bool cancel)
        {
            int patternIndex = _view.PartNavigator.Current;
            double currentPartStartPos = PatternIndexToLineInfo(patternIndex).XPos;

            //if we have scrolled only a bit, go back to previous part, otherwise restart current part
            cancel |= _view.XPos > currentPartStartPos + MinPartScroll;
        }

        private void LineEndReached(int line, ref bool cancel)
        {
            int patternIndex = 0;
            if (line + 1 < _lines.Count)
            {
                patternIndex = _lines[line + 1].PatternIndex;
            }
            else
            {
                cancel = true;
            }

            if (!cancel)
            {
                _view.PartNavigator.Current = patternIndex;
            }
        }

        private void SetupEvents(bool add)
        {
            if (add)
            {
                _view.PartNavigator.ChoiceClicked += PartClicked;
                _view.PartNavigator.PreviousChoiceClicked += PreviousPartClicked;
                _view.LineEndReached += LineEndReached;
            }
            else
            {
                _view.PartNavigator.ChoiceClicked -= PartClicked;
                _view.PartNavigator.PreviousChoiceClicked -= PreviousPartClicked;
                _view.LineEndReached -= LineEndReached;
            }
        }

        private class LinePartInfo
        {
            public readonly int ClippingIndex; //index of the clipping relative to the corresponding part
            public readonly int LineIndex; //the position inside the Navigator.lines list
            public readonly int PartIndex; //index of the parts, 0 corresponds to part "A"
            public readonly int PatternIndex; //the position inside the part pattern, 
            public readonly double XPos; //the absolute xPosition of the line on the view

            public LinePartInfo(int partIndex, int clippingIndex, int patternIndex, int lineIndex, double xPos)
            {
                this.PartIndex = partIndex;
                this.ClippingIndex = clippingIndex;
                this.PatternIndex = patternIndex;
                this.LineIndex = lineIndex;
                this.XPos = xPos;
            }
        }
    }
}