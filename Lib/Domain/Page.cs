using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using PartituraMagica.Lib.Domain;

namespace PartituraMagica
{
	public class Page
	{
		private IDictionary<int, Box> boxes;
		private int selectedBoxId;
		private double pageWith;
		private double pageHeight;
		private bool dirty;
		private static int largestBoxIdTillNow = 0;
		public const double ClickTolerance = 4.5;

		public Page()
		{
			SetDefaults();
			boxes = new Dictionary<int, Box>();
		}

		internal Page(PageMemento p)
				: this()
		{
			foreach (object o in p.Boxes)
			{
				Box b = (Box)o;
				boxes.Add(b.Id, b);
				largestBoxIdTillNow = Math.Max(largestBoxIdTillNow, b.Id);
			}
		}

		internal PageMemento ToMemento()
		{
			dirty = false;
			return new PageMemento(boxes.Values);
		}

		private void SetDefaults()
		{
			selectedBoxId = -1;
			pageWith = 0;
			pageHeight = 0;
			dirty = false;
		}

		private int GetIdAt(Point pos)
		{
			Box b = GetBoxAt(pos);
			return b == null ? 0 : b.Id;
		}

		private Box GetBoxAt(Point pos)
		{
			foreach (Box b in boxes.Values)
			{
				if (b.Contains(pos))
				{
					return b;
				}
			}
			return null;
		}

		/** returns the type of object currently present 
		 * at the given position. This can be used to 
		 * change the Mouse cursor look accordingly.
		 */
		public Shape GetShapeTypeAt(Point pos)
		{
			Box b = GetBoxAt(pos);
			if (b == null)
				return Shape.NothingSpecial;

			return b.GetShapeTypeAt(pos);
		}

		public void AddBox(Rect coords, IDrawingPanel panel, ILabelProvider labelProvider)
		{
			dirty = true;
			int id = ++largestBoxIdTillNow;

			//this guarantess that the box cannot be moved out of the page area:
			coords.OuterRect = new Rect(0, 0, pageWith, pageHeight);

			Box b = new Box(id, coords);
			boxes.Add(id, b);
			PaintPage(panel, labelProvider);
			SelectBoxAt(b.Coords.TopLeft, panel);
		}

		public void RemoveSelectedBox(IDrawingPanel panel, ILabelProvider labelProvider)
		{
			dirty = true;
			double boxY = SelectedBox.Coords.Y;
			SelectedBox.UnPaint(panel);
			boxes.Remove(selectedBoxId);
			selectedBoxId = -1;
			PaintPage(panel, labelProvider);

			//select the nearest remaining box			
			Point nearestBoxPos = default(Point);
			double distance = Double.MaxValue;
			foreach (Box b in boxes.Values)
			{
				double dist = Math.Abs(b.Coords.Y - boxY);
				if (dist < distance)
				{
					distance = dist;
					nearestBoxPos = b.Coords.LeftTop;
				}
			}
			//works also when there are no boxes remaining...
			SelectBoxAt(nearestBoxPos, panel);
		}


		/** returns true exactly if (at least) one item
		 * e.g. a Box is selected.
		 */
		public bool IsBoxSelected()
		{
			return selectedBoxId > -1;
		}

		public bool CanUpdateSelectedBox(Point lastPos, Point newPos)
		{
			if (!IsBoxSelected())
				return false;

			// Create a copy of the Box to try out if updating succeeds.
			Box copy = new Box(SelectedBox);
			if (!copy.Update(lastPos, newPos))
			{
				return false;
			}

			return !Overlaps(copy.Coords, false);
		}

		/** should stay private, cause no Box instances should get out of this class... */
		private Box SelectedBox
		{
			get
			{
				return (Box)boxes[selectedBoxId];
			}
		}

		/// <summary>
		/// Returns true iff the given rectangle overlaps in height with any of the boxes on the page (including the 
		/// selected box only if includeSelectedBox is true).
		/// </summary>
		private bool Overlaps(Rect r, bool includeSelectedBox)
		{
			// [Program code fixed: parameter 'includeSelectedBox' is renamed from 'ignoreSelectedBox' jed 20150118]

			// Make a rectangle which is the given rectangle but over the full page width.
			Rect completeRow =
					new Rect(0, r.Top, pageWith, r.Height).AddAboveAndBelow(Page.ClickTolerance);

			// Loop over all boxes of the page (including the selected box if specified), return true iff any box intersects.
			foreach (Box b in boxes.Values)
			{
				if ((includeSelectedBox || b.Id != selectedBoxId)
						&& b.Coords.IntersectsWith(completeRow))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns true iff the given rectangle overlaps 'in height' with any of the other Boxes in the page. [jed]
		/// </summary>
		public bool Overlaps(Rect r)
		{
			return Overlaps(r, true);
		}

		/** returns ids of all rectangles that need to be updated 
		 * and corresponding new coords. 
		 */
		public void UpdateSelectedBox(Point lastPos, Point newPos, IDrawingPanel panel)
		{
			dirty = true;
			SelectedBox.Update(lastPos, newPos);
			SelectedBox.Paint(panel);
		}

		public void PaintPage(IDrawingPanel panel, ILabelProvider labelProvider)
		{
			foreach (Box b in boxes.Values)
			{
				b.Paint(panel, labelProvider);
			}
		}

		public void PaintPage(IDrawingPanel panel, Point imageSize, ILabelProvider labelProvider)
		{
			PaintPage(panel, labelProvider);
			pageHeight = imageSize.Y;
			pageWith = imageSize.X;
		}

		public bool Dirty
		{
			get
			{
				return dirty;
			}
		}

		[OnSerialized]
		public void Saved(StreamingContext context)
		{
			dirty = false;
		}

		public void SplitSelectedBox(Point pos, IDrawingPanel panel)
		{
			dirty = true;
			SelectedBox.TrySplit(pos);
			SelectedBox.Paint(panel);
		}

		public void SelectBoxAt(Point pos, IDrawingPanel panel)
		{
			Box b = GetBoxAt(pos);
			if (b != null)
			{
				if (IsBoxSelected())
				{
					SelectedBox.Deselect(panel);
				}
				selectedBoxId = b.Id;
				SelectedBox.Select(pos, panel);
			}
		}
		
		/** returns the coords of all Boxes. 
		 * Boxes that contain splitters are returned as several 
		 * boxes with identical Y and Height
		 */
		internal void AppendParts(IList<Part> parts, int currentPageIndex)
		{
			foreach (Box b in SortedBoxes)
			{
				b.AppendParts(parts, currentPageIndex);
			}
		}

		private IList<Box> SortedBoxes
		{
			get
			{
				List<Box> sortedBoxes = new List<Box>(boxes.Values);
				sortedBoxes.Sort(
						(left, right) => (int)(left.Coords.Y - right.Coords.Y));
				return sortedBoxes;
			}
		}


		public void LabelSelection(IDrawingPanel panel, ILabelProvider labelProvider)
		{
			dirty = true;
			SelectedBox.LabelSelection();
			PaintPage(panel, labelProvider);
		}

		internal bool IsSelectionLabelled()
		{
			return SelectedBox.IsSelectionLabelled();
		}

		internal void UnLabelSelection(IDrawingPanel panel, ILabelProvider labelProvider)
		{
			dirty = true;
			SelectedBox.UnLabelSelection();
			PaintPage(panel, labelProvider);
		}

		internal bool IsLeftTopSelected()
		{
			IList<Box> sortedBoxes = SortedBoxes;
			if (sortedBoxes.Count == 0 || selectedBoxId != sortedBoxes[0].Id)
			{
				return false;
			}
			return SelectedBox.IsLeftSelected();
		}
	}
}
