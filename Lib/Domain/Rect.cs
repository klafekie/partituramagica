using System;
using System.Collections.Generic;
using global::System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using JetBrains.Annotations;
using PartituraMagica.Lib.Domain;

namespace PartituraMagica
{
	/**
	 * A class similar to the Rect structure. We prefer to have
	 * a class, cause we often need to check against null.
	 *
	 * A rect has a CenterY property which can be used to represent the position of the originally
	 * detected musical system inside a "box"
	 *
	 * Also here we have convenience methods for changing rectangles
	 * in a way that the coordinates never leave a certain "OuterRectangle".
	 * this class is immutable.
	 */
	[Serializable]
	public class Rect
	{
		private System.Windows.Rect r;

		/** represents the position that should be vertically centered in player view.
		 * the value is relative to current Y possition
		 */
		public double CenterY { get; private set; }

		public Rect OuterRect { get; private set; }

		public Rect(Rect rhs, double? centerY = null, Rect outerRect = null)
			: this(rhs.X, rhs.Y, rhs.Width, rhs.Height, centerY ?? rhs.CenterY, outerRect ?? rhs.OuterRect) { }

		public Rect(Rect rhs, double x, double y, double width, double height, double? centerY = null, Rect outerRect = null)
			: this(x, y, width, height, centerY ?? rhs.CenterY, outerRect ?? rhs.OuterRect) { }

		public Rect(double x, double y, double width, double height)
			: this(x, y, width, height, height / 2d, null) { }

		private Rect(double x, double y, double width, double height, double centerY, Rect outerRect)
		{
			r = new System.Windows.Rect(x, y, width, height);
			OuterRect = outerRect;
			CenterY = centerY;
		}

		public Rect(RectJsonMemento memento)
			: this(memento.X, memento.Y, memento.Width, memento.Height, memento.CenterY ?? memento.Height / 2D, null)
		{
			if (memento.OuterRect != null)
			{
				OuterRect = new Rect(memento.OuterRect);
			}
		}

		public double X => r.X;
		[JsonIgnore]
		public int XInt => (int)Math.Round(this.X);

		public double Y => r.Y;
		[JsonIgnore]
		public int YInt => (int)Math.Round(this.Y);

		public double Width => r.Width;
		[JsonIgnore]
		public int WidthInt => (int)Math.Round(this.Width);

		public double Height => r.Height;
		[JsonIgnore]
		public int HeightInt => (int)Math.Round(Height);

		[JsonIgnore]
		public double Left => this.r.Left;
		[JsonIgnore]
		public int LeftInt => (int)Math.Round(this.Left);

		[JsonIgnore]
		public double Right => this.r.Right;
		[JsonIgnore]
		public int RightInt => (int)Math.Round(this.Right);

		[JsonIgnore]
		public double Top => this.r.Top;

		[JsonIgnore]
		public double Bottom => this.r.Bottom;

		[JsonIgnore]
		public Size Size => this.r.Size;

		[JsonIgnore]
		public Point TopLeft => this.r.TopLeft;

		public bool Contains(Point point) => this.r.Contains(point.X, point.Y);

		//we intentionally dont check the outerRect/minSize here
		public Rect Scale(double xFactor, double yFactor)
		{
			return new Rect(this.X * xFactor, this.Y * yFactor, this.Width * xFactor, this.Height * yFactor, this.CenterY * yFactor, this.OuterRect);
		}

		public Rect AddAbove(double delta)
		{
			return AddAbove(delta, null);
		}

		public Rect AddAbove(double delta, Size? minSize)
		{
			return SafeCreateRect(this.X, this.Y - delta, this.Width, this.Height + delta, minSize);
		}

		private Rect SafeCreateKeepSize(double x, double y)
		{
			double width = this.Width;
			double height = this.Height;
			if (OuterRect != null)
			{
				if (x < OuterRect.X)
				{
					x = OuterRect.X;
				}
				if (y < OuterRect.Y)
				{
					y = OuterRect.Y;
				}
				if (x + width > OuterRect.Right)
				{
					x = OuterRect.Right - width;
				}
				if (y + height > OuterRect.Bottom)
				{
					y = OuterRect.Bottom - height;
				}
			}

			return new Rect(this, x, y, width, height);
		}

		private Rect SafeCreateRect(double x, double y, double width, double height, Size? minSize)
		{
			//the order of the various checks is important
			if (minSize != null)
			{
				if (width < minSize.Value.Width || height < minSize.Value.Height)
					return this;
			}
			if (OuterRect != null)
			{
				if (x < OuterRect.X)
				{
					width -= OuterRect.X - x;
					x = OuterRect.X;
				}
				if (y < OuterRect.Y)
				{
					height -= OuterRect.Y - y;
					y = OuterRect.Y;
				}
				if (x + width > OuterRect.Right) width = OuterRect.Right - x;
				if (y + height > OuterRect.Bottom) height = OuterRect.Bottom - y;
			}

			//adjust centerY until min height is reached
			double deltaY = y - this.Y;
			double deltaHeight = height - this.Height;
			double centerY = this.CenterY;
			if (this.Y + deltaY < this.Y + this.CenterY - 2D * Page.ClickTolerance)
			{
				//centerY line is "pushed" from above
				centerY = this.CenterY - deltaY;
			}
			if (centerY + Page.ClickTolerance >= height)
			{
				//centerY is pushed from below
				centerY = height - Page.ClickTolerance;
			}

			return new Rect(this, x, y, width, height, centerY);
		}

		public Rect AdjustCenterY(double deltaY)
		{
			double newCenterY = this.CenterY + deltaY;
			if (newCenterY > 2d * Page.ClickTolerance && newCenterY < this.Height - Page.ClickTolerance)
			{
				return new Rect(this, centerY: newCenterY);
			}

			return this;
		}

		public Rect AddBelow(double delta)
		{
			return AddBelow(delta, null);
		}

		public Rect AddBelow(double delta, Size? minSize)
		{
			return SafeCreateRect(this.X, this.Y, this.Width, this.Height + delta, minSize);
		}

		public Rect AddRight(double delta, Size? minSize)
		{
			return SafeCreateRect(this.X, this.Y, this.Width + delta, this.Height, minSize);
		}

		public Rect AddLeftBottom(double deltaX, double deltaY, Size minSize)
		{
			return SafeCreateRect(this.X - deltaX, this.Y, this.Width + deltaX, this.Height + deltaY, minSize);
		}

		public Rect AddLeft(double delta, Size? minSize)
		{
			return SafeCreateRect(this.X - delta, this.Y, this.Width + delta, this.Height, minSize);
		}

		public Rect AddRightBottom(double deltaX, double deltaY, Size? minSize, bool adjustCenterY)
		{
			Rect safeCreateRect = SafeCreateRect(this.X, this.Y, this.Width + deltaX, this.Height + deltaY, minSize);
			if (adjustCenterY)
				safeCreateRect = new Rect(safeCreateRect, centerY: safeCreateRect.Height / 2D);
			return safeCreateRect;
		}

		public Rect AddLeftTop(double deltaX, double deltaY, Size? minSize)
		{
			return SafeCreateRect(this.X - deltaX, this.Y - deltaY, this.Width + deltaX, this.Height + deltaY, minSize);
		}

		public Rect AddRightTop(double deltaX, double deltaY, Size? minSize)
		{
			return SafeCreateRect(this.X, this.Y - deltaY, this.Width + deltaX, this.Height + deltaY, minSize);
		}

		public Rect AddAboveAndBelow(double deltaY)
		{
			return AddAboveAndBelow(deltaY, null);
		}

		public Rect AddAboveAndBelow(double deltaY, Size? minSize)
		{
			return AddAbove(deltaY, minSize).AddBelow(deltaY, minSize);
		}

		public Rect AddLeftAndRight(double deltaX, Size? minSize)
		{
			return AddLeft(deltaX, minSize).AddRight(deltaX, minSize);
		}

		public Rect AddInAllDirections(double delta, Size? minSize)
		{
			return AddAbove(delta, minSize).AddBelow(delta, minSize).AddLeft(delta, minSize).AddRight(delta, minSize);
		}

		public Rect MoveTo(double xPos, double yPos)
		{
			return SafeCreateKeepSize(xPos, yPos);
		}

		public Rect Move(double deltaX, double deltaY)
		{
			return SafeCreateKeepSize(this.X + deltaX, this.Y + deltaY);
		}

		public bool IntersectsWith(Rect rect)
		{
			return this.r.IntersectsWith(rect.r);
		}

		public override bool Equals(object o)
		{
			return this.Equals(o as Rect);
		}

		public bool Equals(Rect value)
		{
			if (value == null) return false;
			return this.r.Equals(value.r) && this.CenterY.Equals(value.CenterY);
		}

		public static bool operator !=(Rect rect1, Rect rect2)
		{
			return !(rect1 == rect2);
		}

		public static bool operator ==(Rect rect1, Rect rect2)
		{
			if (Object.ReferenceEquals(rect1, null)) return Object.ReferenceEquals(rect2, null);
			return rect1.Equals(rect2);
		}

		public override int GetHashCode()
		{
			return this.r.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("({0:0.0},{1:0.0},{2:0.0},{3:0.0}) centerY: {4:0.0} outerRect: {5}", Left, Top, Right, Bottom, CenterY, OuterRect);
		}

		public RectJsonMemento ToMemento()
		{
			var result = new RectJsonMemento() { X = X, Y = Y, Width = Width, Height = Height, CenterY = CenterY };
			if (OuterRect != null)
			{
				result.OuterRect = OuterRect.ToMemento();
			}

			return result;
		}


	}
}
