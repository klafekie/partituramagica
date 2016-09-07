using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace PartituraMagica
{
    /// <summary>
    /// Boxes are for the 'vertical' splitting of an input page: a 'Box' is a horizontal rectangle on an input page. 
    /// Boxes can be above each other but not horizontally next to each other.
    /// A box has a SplitterList which can divide the box into fragments which are next to each other.
    /// </summary>
    [Serializable]
    internal class Box
    {
        //if this box (or a splitter in it) is labelled, this holds the
        //corresponding box/splitter ids. empty otherwise
        private readonly IList<int> _labelIds;
        private int _selectedId;
        private readonly SplitterList _splitters;

        public Box(int id, Rect coords)
        {
            Id = id;
            _selectedId = -1;
            _labelIds = new List<int>();

            _splitters = new SplitterList();
            Coords = coords;
        }

        /// <summary>
        /// Creates deep copy of the given Box 'rhs'.
        /// Currently only used during editing (in Page.CanUpdateSelectedBox()) [20150118].
        /// </summary>
        public Box(Box rhs)
        {
            Id = rhs.Id;
            Coords = new Rect(rhs.Coords);
            _splitters = new SplitterList(rhs._splitters);

            _selectedId = rhs._selectedId;
            _labelIds = rhs._labelIds;
        }

        public int Id { get; }

        public Rect Coords { get; private set; }

        /** 
         * the minimum distance between to objects (boxes or splitters)
         * on the screen 
         */

        private static double MinItemDistance
        {
            get { return 2.0*Page.ClickTolerance + 1.0; }
        }

        private Size MinBoxSize
        {
            get
            {
                double borderLine = MakeRelative(Coords.X);
                borderLine = Math.Max(borderLine,
                    _splitters.RightmostPos);
                return new Size(borderLine + MinItemDistance, MinItemDistance);
            }
        }

        private Rect MakeAbsolute(Rect r)
        {
            r.OuterRect = Coords.OuterRect;
            return r.Move(Coords.X, Coords.Y);
        }

        private double MakeRelative(double abs)
        {
            return abs - Coords.X;
        }

        public bool Contains(Point pos)
        {
            Rect outer = GetOuterRect(Coords);
            return outer.Contains(pos);
        }

        private static Rect GetOuterRect(Rect r)
        {
            return r.AddInAllDirections(Page.ClickTolerance, null);
        }

        private static Rect GetInnerRect(Rect r)
        {
            return r.AddInAllDirections(-Page.ClickTolerance, new Size(1, 1));
        }

        public Shape GetShapeTypeAt(Point pos)
        {
            Rect inner = GetInnerRect(Coords);
            uint row = 0;
            uint column = 0;
            if (pos.X <= inner.Left)
            {
                column = 0;
            }
            else if (pos.X < inner.Right)
            {
                column = 1;
            }
            else
            {
                column = 2;
            }
            if (pos.Y <= inner.Top)
            {
                row = 0;
            }
            else if (pos.Y < inner.Bottom)
            {
                row = 1;
            }
            else
            {
                row = 2;
            }

            Shape[,] shapeTypeGrid =
            {
                {Shape.BoxLeftTopCorner, Shape.BoxTopMargin, Shape.BoxRightTopCorner},
                {Shape.BoxLeftMargin, Shape.BoxContent, Shape.BoxRightMargin},
                {Shape.BoxLeftBottomCorner, Shape.BoxBottomMargin, Shape.BoxRightBottomCorner}
            };

            Shape result = shapeTypeGrid[row, column];

            //now check for splitters, this can only happen when we are somewere in the middle of the
            //box
            if (result == Shape.BoxContent &&
                FindSplitterAt(pos) != -1)
            {
                return Shape.BoxSplitter;
            }
            return result;
        }

        private int FindSplitterAt(Point pos)
        {
            return _splitters.FindAt(MakeRelative(pos.X));
        }

        public bool Update(Point lastPos, Point newPos)
        {
            bool splitterMoved = false;
            double deltaX = newPos.X - lastPos.X;
            double deltaY = newPos.Y - lastPos.Y;

            Rect result = null;

            switch (GetShapeTypeAt(lastPos))
            {
                case Shape.BoxContent:
                    result = Coords.Move(deltaX, deltaY);
                    break;
                case Shape.BoxLeftTopCorner:
                    result = Coords.AddLeftTop(-deltaX, -deltaY, MinBoxSize);
                    break;
                case Shape.BoxTopMargin:
                    result = Coords.AddAbove(-deltaY, MinBoxSize);
                    break;
                case Shape.BoxRightTopCorner:
                    result = Coords.AddRightTop(deltaX, -deltaY, MinBoxSize);
                    break;
                case Shape.BoxRightMargin:
                    result = Coords.AddRight(deltaX, MinBoxSize);
                    break;
                case Shape.BoxRightBottomCorner:
                    result = Coords.AddRightBottom(deltaX, deltaY, MinBoxSize);
                    break;
                case Shape.BoxBottomMargin:
                    result = Coords.AddBelow(deltaY, MinBoxSize);
                    break;
                case Shape.BoxLeftBottomCorner:
                    result = Coords.AddLeftBottom(-deltaX, deltaY, MinBoxSize);
                    break;
                case Shape.BoxLeftMargin:
                    result = Coords.AddLeft(-deltaX, MinBoxSize);
                    break;
                case Shape.BoxSplitter:
                    splitterMoved = _splitters.MoveSplitter(deltaX, FindSplitterAt(lastPos), Coords.Size);
                    break;
                default:
                    throw new ArgumentException("unexpected shapeType");
            }
            bool boxMoved = result != null && result != Coords;
            if (boxMoved)
            {
                Coords = result;
            }
            return boxMoved || splitterMoved;
        }

        public void TrySplit(Point pos)
        {
            double desiredPos = pos.X;
            if (!Contains(pos))
            {
                desiredPos = Coords.Left + Coords.Width/2.0;
            }
            _splitters.TrySplit(MakeRelative(desiredPos), Id, Coords.Size);
        }

        /** paint without changing the selection or the labelling */

        internal void Paint(IDrawingPanel panel)
        {
            Paint(panel, null);
        }

        /** paint without changing the selection */

        internal void Paint(IDrawingPanel panel, ILabelProvider labelProvider)
        {
            IEnumerable<KeyValuePair<int, Rect>> rectangles = GetAllCoordsWithIds();
            foreach (KeyValuePair<int, Rect> entry in rectangles)
            {
                panel.AddOrUpdateRectangle(entry.Key, entry.Value);
                if (labelProvider != null)
                {
                    panel.LabelRectangle(entry.Key,
                        labelProvider.GetLabel(entry.Key));
                }
            }
            ;
        }

        internal void Select(Point pos, IDrawingPanel panel)
        {
            Paint(panel); //add any missing rectangles
            if (_selectedId != -1)
            {
                panel.DeselectRectangle(_selectedId);
            }
            _selectedId = GetIdAt(pos);
            panel.SelectRectangle(_selectedId);
        }

        private int GetIdAt(Point pos)
        {
            IEnumerable<KeyValuePair<int, Rect>> rectangles = GetAllCoordsWithIds();

            IEnumerator<KeyValuePair<int, Rect>> entries =
                rectangles.Where(
                    e => GetOuterRect(e.Value).Contains(pos)).GetEnumerator();

            //for splitters there can be two rects found at the same postion
            //we simply take the first one

            if (entries.MoveNext())
            {
                return entries.Current.Key;
            }
            return -1;
        }

        internal void Deselect(IDrawingPanel panel)
        {
            if (_selectedId != -1)
            {
                panel.DeselectRectangle(_selectedId);
            }
            _selectedId = -1;
        }

        //or better Hide()?
        internal void UnPaint(IDrawingPanel panel)
        {
            panel.RemoveRectangle(Id);

            foreach (int id in _splitters.Ids)
            {
                panel.RemoveRectangle(id);
            }
        }

        /// <summary>
        /// 'Parses' the rectangles and their labels into the parts list, appending the rectangles of each part to the part's 'clippings' list.
        /// </summary>
        /// <param name="currentPageIndex">The index of the page this box is on (for registering).</param>
        internal void AppendParts(IList<Part> parts, int currentPageIndex)
        {
            // Initialize parts to a single part (because any partitura is at least one part) and make that the 'current' part.
            if (parts.Count == 0)
            {
                parts.Add(new Part());
            }
            // The current part to which found 
            Part currentPart = parts[parts.Count - 1];

            IEnumerable<KeyValuePair<int, Rect>> withId = GetAllCoordsWithIds();

            foreach (KeyValuePair<int, Rect> entry in withId)
            {
                if (_labelIds.Contains(entry.Key)
                    && !currentPart.IsEmpty)
                {
                    //beginning of a new part...
                    //empty parts can happen when we are at the first box on first page					
                    currentPart = new Part();
                    parts.Add(currentPart);
                }
                currentPart.Add(new Clipping(currentPageIndex, entry.Value, entry.Key));
            }
        }

        /// <summary>
        /// Returns coords for the complete box (including splitters).
        /// If this box contains a splitter, we return two Rects, one for the
        /// left part, one for the right part. The dictionary key is the id of 
        /// the box itself for the leftmost rectangle and the id of the splitter
        /// "at the left of the rectangle" for the others.
        /// Note that this is different from the internal representation of Splitters.
        /// </summary>
        internal IEnumerable<KeyValuePair<int, Rect>> GetAllCoordsWithIds()
        {
            IEnumerable<KeyValuePair<int, Rect>> relResults =
                _splitters.GetAllCoordsWithIds(Id, Coords.Size);

            var result = from entry in relResults
                select new KeyValuePair<int, Rect>(entry.Key, MakeAbsolute(entry.Value));
            return result;
        }

        internal void LabelSelection()
        {
            _labelIds.Add(_selectedId);
        }

        internal bool IsSelectionLabelled()
        {
            return _labelIds.Contains(_selectedId);
        }

        internal void UnLabelSelection()
        {
            _labelIds.Remove(_selectedId);
        }

        internal bool IsLeftSelected()
        {
            return _selectedId == Id;
        }

        /// a splitter is a vertical line that splits a box.
        /// a box can contain several splitters.	
        /// splitters are kept sorted by the relative x value
        /// the SplitterList knows nothing about the abs pos of the box
        [Serializable]
        private class SplitterList
        {
            private readonly SortedList<double, int> _splitters;

            internal SplitterList()
            {
                _splitters = new SortedList<double, int>();
            }

            /// <summary>
            /// Creates deep copy of the given SplitterList 'rhs'.
            /// Currently only used for editing (in Page.CanUpdateSelectedBox()) [20150118].
            /// </summary>
            internal SplitterList(SplitterList rhs)
                : this()
            {
                foreach (KeyValuePair<double, int> entry in rhs._splitters)
                {
                    _splitters.Add(entry.Key, entry.Value);
                }
            }

            internal IEnumerable<int> Ids
            {
                get { return _splitters.Values; }
            }

            internal double RightmostPos
            {
                get
                {
                    if (_splitters.Count == 0) return 0D;
                    return _splitters.Last().Key;
                }
            }

            internal void Add(double pos, int id)
            {
                _splitters.Add(pos, id);
            }

            internal int FindAt(double posX)
            {
                var entries = (from entry in _splitters
                    where posX <= entry.Key + Page.ClickTolerance &&
                          posX >= entry.Key - Page.ClickTolerance
                    select entry).ToArray();
                if (entries.Length == 0)
                {
                    return -1;
                }
                Debug.Assert(entries.Length == 1);
                return entries.First().Value;
            }

            internal IEnumerable<KeyValuePair<int, Rect>> GetAllCoordsWithIds(int boxId, Size boxSize)
            {
                List<KeyValuePair<int, Rect>> result = new List<KeyValuePair<int, Rect>>();
                KeyValuePair<double, int> lastSplitter = default(KeyValuePair<double, int>);
                double x = 0;
                double width = 0;
                int id = -1;
                int i = 0;
                foreach (KeyValuePair<double, int> entry in _splitters)
                {
                    if (i == 0)
                    {
//leftmost rect                    
                        x = 0D;
                        width = entry.Key;
                        id = boxId;
                    }
                    else
                    {
                        x = lastSplitter.Key;
                        width = entry.Key - lastSplitter.Key;
                        id = lastSplitter.Value;
                    }

                    result.Add(
                        new KeyValuePair<int, Rect>(id,
                            new Rect(x, 0, width, boxSize.Height)));

                    lastSplitter = entry;
                    i++;
                }

                if (i == 0) //no splitters at all
                {
                    x = 0D;
                    width = boxSize.Width;
                    id = boxId;
                }
                else //rightmost rect
                {
                    x = lastSplitter.Key;
                    width = boxSize.Width - lastSplitter.Key;
                    id = lastSplitter.Value;
                }

                result.Add(
                    new KeyValuePair<int, Rect>(id,
                        new Rect(x, 0D, width, boxSize.Height)));

                return result;
            }

            public bool MoveSplitter(double deltaX, int id, Size boxSize)
            {
                if (deltaX == 0) return false;
                double oldPos = (from entry in _splitters
                        where entry.Value == id
                        select entry.Key).
                    Single();
                double desiredPos = oldPos + deltaX;
                bool wouldWork = EnoughRoomForSplitterAt(desiredPos, id, boxSize);
                if (wouldWork)
                {
                    _splitters.Remove(oldPos);
                    _splitters.Add(desiredPos, id);
                }
                return wouldWork;
            }


            public void TrySplit(double splitterPos, int boxId, Size boxSize)
            {
                if (EnoughRoomForSplitterAt(splitterPos, -1, boxSize))
                {
                    int splitterId = _splitters.Count + 1000*(boxId + 1);
                    _splitters.Add(splitterPos, splitterId);
                }
            }

            private bool EnoughRoomForSplitterAt(double desiredPos, int id, Size boxSize)
            {
                double leftTolerance = desiredPos - Page.ClickTolerance;
                double rightTolerance = desiredPos + Page.ClickTolerance;

                int leftId = FindAt(leftTolerance);
                bool leftOk = (leftId == -1 || leftId == id) &&
                              desiredPos > 2*Page.ClickTolerance;
                int rightId = FindAt(rightTolerance);
                bool rightOk = (rightId == -1 || rightId == id) &&
                               desiredPos < boxSize.Width - 2*Page.ClickTolerance;

                return leftOk && rightOk;
            }
        }
    }
}