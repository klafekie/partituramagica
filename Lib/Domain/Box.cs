using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PartituraMagica.Lib.Domain;

namespace PartituraMagica
{
	/// <summary>
	/// Boxes are for the 'vertical' splitting of an input page: a 'Box' is a horizontal rectangle on an input page. 
	/// Boxes can be above each other but not horizontally next to each other.
	/// A box has a SplitterList which can divide the box into fragments which are next to each other.
	/// </summary>
	[Serializable]
	public class Box
	{

		/// a splitter is a vertical line that splits a box.
		/// a box can contain several splitters.	
		/// splitters are kept sorted by the relative x value
		/// the SplitterList knows nothing about the abs pos of the box
		[Serializable]
		public class SplitterList
		{
			private SortedList<double, int> splitters;

			internal SplitterList()
			{
				this.splitters = new SortedList<double, int>();
			}

			internal SplitterList(ICollection<KeyValuePair<double, int>> boxSplitters) : this()
			{
				foreach (var splitter in boxSplitters)
				{
					this.splitters.Add(splitter.Key, splitter.Value);
				}
			}

			/// <summary>
			/// Creates deep copy of the given SplitterList 'rhs'.
			/// Currently only used for editing (in Page.CanUpdateSelectedBox()) [20150118].
			/// </summary>
			internal SplitterList(SplitterList rhs)
					: this()
			{
				foreach (KeyValuePair<double, int> entry in rhs.splitters)
				{
					this.splitters.Add(entry.Key, entry.Value);
				}
			}

			internal IEnumerable<int> Ids
			{
				get
				{
					return this.splitters.Values;
				}
			}

			internal void Add(double pos, int id)
			{
				this.splitters.Add(pos, id);
			}

			internal double RightmostPos
			{
				get
				{
					if (this.splitters.Count == 0) return 0D;
					return this.splitters.Last().Key;
				}
			}

			//just for serialization
			public SortedList<double, int> Splitters => splitters;

			internal int FindAt(double posX)
			{
				var entries = (from entry in this.splitters
											 where posX <= entry.Key + Page.ClickTolerance &&
														posX >= entry.Key - Page.ClickTolerance
											 select entry).ToArray();
				if (entries.Length == 0)
				{
					return -1;
				}
				else
				{
					Debug.Assert(entries.Length == 1);
					return entries.First().Value;
				}
			}

			internal IEnumerable<KeyValuePair<int, Rect>> GetAllCoordsWithIds(int boxId, Size boxSize)
			{
				List<KeyValuePair<int, Rect>> result = new List<KeyValuePair<int, Rect>>();
				KeyValuePair<double, int> lastSplitter = default(KeyValuePair<double, int>);
				double x = 0;
				double width = 0;
				int id = -1;
				int i = 0;
				foreach (KeyValuePair<double, int> entry in this.splitters)
				{
					if (i == 0)
					{//leftmost rect                    
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
				double oldPos = (from entry in this.splitters
												 where entry.Value == id
												 select entry.Key).
												Single();
				double desiredPos = oldPos + deltaX;
				bool wouldWork = EnoughRoomForSplitterAt(desiredPos, id, boxSize);
				if (wouldWork)
				{
					this.splitters.Remove(oldPos);
					this.splitters.Add(desiredPos, id);
				}
				return wouldWork;
			}


			public void TrySplit(double splitterPos, int boxId, Size boxSize)
			{
				if (EnoughRoomForSplitterAt(splitterPos, -1, boxSize))
				{
					int splitterId = this.splitters.Count + 1000 * (boxId + 1);
					splitters.Add(splitterPos, splitterId);
				}
			}

			private bool EnoughRoomForSplitterAt(double desiredPos, int id, Size boxSize)
			{
				double leftTolerance = desiredPos - Page.ClickTolerance;
				double rightTolerance = desiredPos + Page.ClickTolerance;

				int leftId = FindAt(leftTolerance);
				bool leftOk = (leftId == -1 || leftId == id) &&
						desiredPos > 2 * Page.ClickTolerance;
				int rightId = FindAt(rightTolerance);
				bool rightOk = (rightId == -1 || rightId == id) &&
						desiredPos < boxSize.Width - 2 * Page.ClickTolerance;

				return leftOk && rightOk;
			}
		}

		private int id;
		private Rect coords;
		private SplitterList splitters;

		//just for json serialization!
		public IList<KeyValuePair<double, int>> Splitters => splitters.Splitters.ToList();
		public IList<int> LabelIds => labelIds;

		private int selectedId;
		//if this box (or a splitter in it) is labelled, this holds the
		//corresponding box/splitter ids. empty otherwise
		private IList<int> labelIds;

		public Box(int id, Rect coords)
		{
			this.id = id;
			this.selectedId = -1;
			this.labelIds = new List<int>();

			this.splitters = new SplitterList();
			this.coords = coords;
		}

		/// <summary>
		/// Creates deep copy of the given Box 'rhs'.
		/// Currently only used during editing (in Page.CanUpdateSelectedBox()) [20150118].
		/// </summary>
		public Box(Box rhs)
		{
			this.id = rhs.id;
			this.coords = new Rect(rhs.coords);
			this.splitters = new SplitterList(rhs.splitters);

			this.selectedId = rhs.selectedId;
			this.labelIds = rhs.labelIds;
		}

		public Box(BoxJsonMemento box)
		{
			this.id = box.Id;
			this.coords = new Rect(box.Coords);
			if (box.Splitters != null)
				this.splitters = new SplitterList(box.Splitters);
			else
			{
				this.splitters = new SplitterList();
			}
			this.selectedId = -1;
			if (box.LabelIds != null)
			{
				this.labelIds = new List<int>(box.LabelIds);
			}
			else
			{
				this.labelIds = new List<int>();
			}

		}

		public int Id
		{
			get
			{
				return id;
			}
		}

		public Rect Coords
		{
			get
			{
				return coords;
			}
		}

		/** 
		 * the minimum distance between to objects (boxes or splitters)
		 * on the screen 
		 */
		private static double MinItemDistance
		{
			get
			{
				return 2.0 * Page.ClickTolerance + 1.0;
			}
		}

		private Rect MakeAbsolute(Rect r)
		{
			r.OuterRect = this.coords.OuterRect;
			return r.Move(this.coords.X, this.coords.Y);
		}

		private double MakeRelative(double abs)
		{
			return abs - this.coords.X;
		}

		private Size MinBoxSize
		{
			get
			{
				double borderLine = MakeRelative(this.coords.X);
				borderLine = Math.Max(borderLine,
						this.splitters.RightmostPos);
				return new Size(borderLine + MinItemDistance, MinItemDistance);
			}
		}

		public bool Contains(Point pos)
		{
			Rect outer = GetOuterRect(this.coords);
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
			Rect inner = GetInnerRect(this.coords);
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

			Shape[,] shapeTypeGrid = {
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
			return this.splitters.FindAt(MakeRelative(pos.X));
		}

		public bool Update(Point lastPos, Point newPos, Shape lastShape)
		{
			bool splitterMoved = false;
			double deltaX = newPos.X - lastPos.X;
			double deltaY = newPos.Y - lastPos.Y;

			Rect result = null;

			switch (lastShape)
			{
				case Shape.BoxContent:
					result = this.coords.Move(deltaX, deltaY);
					break;
				case Shape.BoxLeftTopCorner:
					result = this.coords.AddLeftTop(-deltaX, -deltaY, this.MinBoxSize);
					break;
				case Shape.BoxTopMargin:
					result = this.coords.AddAbove(-deltaY, this.MinBoxSize);
					break;
				case Shape.BoxRightTopCorner:
					result = this.coords.AddRightTop(deltaX, -deltaY, this.MinBoxSize);
					break;
				case Shape.BoxRightMargin:
					result = this.coords.AddRight(deltaX, this.MinBoxSize);
					break;
				case Shape.BoxRightBottomCorner:
					result = this.coords.AddRightBottom(deltaX, deltaY, this.MinBoxSize);
					break;
				case Shape.BoxBottomMargin:
					result = this.coords.AddBelow(deltaY, this.MinBoxSize);
					break;
				case Shape.BoxLeftBottomCorner:
					result = this.coords.AddLeftBottom(-deltaX, deltaY, this.MinBoxSize);
					break;
				case Shape.BoxLeftMargin:
					result = this.coords.AddLeft(-deltaX, this.MinBoxSize);
					break;
				case Shape.BoxSplitter:
					int splitterId = FindSplitterAt(lastPos);
					if (splitterId >= 0)
						splitterMoved = this.splitters.MoveSplitter(deltaX, splitterId, this.coords.Size);
					break;
				default:
					throw new ArgumentException("unexpected shapeType");
			}
			bool boxMoved = result != null && result != this.coords;
			if (boxMoved)
			{
				this.coords = result;
			}
			return boxMoved || splitterMoved;
		}

		public void TrySplit(Point pos)
		{
			double desiredPos = pos.X;
			if (!this.Contains(pos))
			{
				desiredPos = this.coords.Left + (this.coords.Width / 2.0);
			}
			this.splitters.TrySplit(MakeRelative(desiredPos), this.id, this.coords.Size);
		}

		/** paint without changing the selection or the labelling */
		internal void Paint(IDrawingPanel panel)
		{
			this.Paint(panel, null);
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
			};
		}

		internal void Select(Point pos, IDrawingPanel panel)
		{
			Paint(panel);//add any missing rectangles
			if (this.selectedId != -1)
			{
				panel.DeselectRectangle(this.selectedId);
			}
			this.selectedId = GetIdAt(pos);
			panel.SelectRectangle(this.selectedId);
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
			else
			{
				return -1;
			}
		}

		internal void Deselect(IDrawingPanel panel)
		{
			if (this.selectedId != -1)
			{
				panel.DeselectRectangle(this.selectedId);
			}
			this.selectedId = -1;
		}

		//or better Hide()?
		internal void UnPaint(IDrawingPanel panel)
		{
			panel.RemoveRectangle(this.id);

			foreach (int id in this.splitters.Ids)
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
				if (this.labelIds.Contains(entry.Key)
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
					this.splitters.GetAllCoordsWithIds(this.Id, this.coords.Size);

			var result = from entry in relResults
									 select new KeyValuePair<int, Rect>(entry.Key, MakeAbsolute(entry.Value));
			return result;
		}

		internal void LabelSelection()
		{
			this.labelIds.Add(this.selectedId);
		}

		internal bool IsSelectionLabelled()
		{
			return this.labelIds.Contains(this.selectedId);
		}

		internal void UnLabelSelection()
		{
			this.labelIds.Remove(this.selectedId);
		}

		internal bool IsLeftSelected()
		{
			return this.selectedId == this.id;
		}

		public BoxJsonMemento ToMemento()
		{
			return new BoxJsonMemento()
			{
				Id = Id,
				Coords = Coords.ToMemento(),
				LabelIds = LabelIds,
				Splitters = Splitters
			};
		}
	}
}
