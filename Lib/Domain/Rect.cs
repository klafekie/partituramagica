using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PartituraMagica
{
    /**
     * A class similar to the Rect structure. We prefer to have
     * a class, cause we often need to check against null.
     * Also here we have convenience methods for changing rectangles
     * in a way that the coordinates never leave a certain "OuterRectangle".
     * this class is immutable.
     */
    [Serializable]
    public class Rect
    {
        private System.Windows.Rect r;
        private Rect outerRect;
        public Rect OuterRect
        {
            get
            {
                return outerRect;
            }
            set
            {
                outerRect = value;
            }
        }

        public Rect(Rect rhs)
            : this(rhs.X, rhs.Y, rhs.Width, rhs.Height, rhs.outerRect) { }

        public Rect(double x, double y, double width, double height)
            : this(x, y, width, height, null) { }

        private Rect(double x, double y, double width, double height, Rect outerRect)
        {
            this.r = new System.Windows.Rect(x, y, width, height);
            this.outerRect = outerRect;
        }

        public double X { get { return this.r.X; } }
        public int XInt { get { return (int)Math.Round(this.X); } }
        public double Y { get { return this.r.Y; } }
        public int YInt { get { return (int)Math.Round(this.Y); } }
        public double Width { get { return this.r.Width; } }
        public int WidthInt { get { return (int)Math.Round(this.Width); } }
        public double Height { get { return this.r.Height; } }
        public int HeightInt { get { return (int)Math.Round(this.Height); } }

        public double Left { get { return this.r.Left; } }
        public int LeftInt { get { return (int)Math.Round(this.Left); } }
        public double Right { get { return this.r.Right; } }
        public int RightInt { get { return (int)Math.Round(this.Right); } }
        public double Top { get { return this.r.Top; } }
        public double Bottom { get { return this.r.Bottom; } }

        public Point LeftTop
        {
            get
            {
                return this.r.Location;
            }
        }
        public Size Size { get { return this.r.Size; } }
        public Point TopLeft { get { return this.r.TopLeft; } }

        public bool Contains(Point point) { return this.r.Contains(point.X, point.Y); }

        //we intentionally dont check the outerRect/minSize here
        public Rect Scale(double xFactor, double yFactor)
        {
            return new Rect(this.X * xFactor, this.Y * yFactor, this.Width * xFactor, this.Height * yFactor);
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
            if (this.outerRect != null)
            {
                if (x < this.outerRect.X)
                {
                    x = this.outerRect.X;
                }
                if (y < this.outerRect.Y)
                {
                    y = this.outerRect.Y;
                }
                if (x + width > this.outerRect.Right)
                {
                    x = this.outerRect.Right - width;
                }
                if (y + height > this.outerRect.Bottom)
                {
                    y = this.outerRect.Bottom - height;
                }
            }
            return new Rect(x, y, width, height, this.outerRect);
        }

        private Rect SafeCreateRect(double x, double y, double width, double height, Size? minSize)
        {
            //the order of the various checks is important
            if (minSize != null)
            {
                if (width < minSize.Value.Width || height < minSize.Value.Height)
                    return this;
            }
            if (this.outerRect != null)
            {
                if (x < this.outerRect.X)
                {
                    width -= this.outerRect.X - x;
                    x = this.outerRect.X;
                }
                if (y < this.outerRect.Y)
                {
                    height -= this.outerRect.Y - y;
                    y = this.outerRect.Y;
                }
                if (x + width > this.outerRect.Right) width = this.outerRect.Right - x;
                if (y + height > this.outerRect.Bottom) height = this.outerRect.Bottom - y;
            }
            return new Rect(x, y, width, height, this.outerRect);
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

        public Rect AddRightBottom(double deltaX, double deltaY, Size? minSize)
        {
            return SafeCreateRect(this.X, this.Y, this.Width + deltaX, this.Height + deltaY, minSize);
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
            return SafeCreateRect(this.X, this.Y - deltaY, this.Width, this.Height + 2D * deltaY, minSize);
        }

        public Rect AddLeftAndRight(double deltaX, Size? minSize)
        {
            return SafeCreateRect(this.X - deltaX, this.Y, this.Width + 2D * deltaX, this.Height, minSize);
        }

        public Rect AddInAllDirections(double delta, Size? minSize)
        {
            return SafeCreateRect(this.X - delta, this.Y - delta, this.Width + 2D * delta, this.Height + 2D * delta, minSize);
        }

        public Rect MoveTo(double xPos, double yPos)
        {
            return SafeCreateKeepSize(xPos, yPos);
        }

        public Rect Move(double deltaX, double deltaY)
        {
            return SafeCreateKeepSize(this.X + deltaX, this.Y + deltaY);
        }

        public bool IntersectsWith(Rect rect) { return this.r.IntersectsWith(rect.r); }

        public override bool Equals(object o)
        {
            return this.Equals(o as Rect);
        }

        public bool Equals(Rect value)
        {
            if (value == null) return false;
            return this.r.Equals(value.r);
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

        public override int GetHashCode() { return this.r.GetHashCode(); }

        public override string ToString() { return this.r.ToString(); }
    }
}
