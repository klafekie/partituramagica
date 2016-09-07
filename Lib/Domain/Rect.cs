using System;
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
        private Rect outerRect;
        private System.Windows.Rect r;

        public Rect(Rect rhs)
            : this(rhs.X, rhs.Y, rhs.Width, rhs.Height, rhs.outerRect)
        {
        }

        public Rect(double x, double y, double width, double height)
            : this(x, y, width, height, null)
        {
        }

        private Rect(double x, double y, double width, double height, Rect outerRect)
        {
            r = new System.Windows.Rect(x, y, width, height);
            this.outerRect = outerRect;
        }

        public Rect OuterRect
        {
            get { return outerRect; }
            set { outerRect = value; }
        }

        public double X
        {
            get { return r.X; }
        }

        public int XInt
        {
            get { return (int) Math.Round(X); }
        }

        public double Y
        {
            get { return r.Y; }
        }

        public int YInt
        {
            get { return (int) Math.Round(Y); }
        }

        public double Width
        {
            get { return r.Width; }
        }

        public int WidthInt
        {
            get { return (int) Math.Round(Width); }
        }

        public double Height
        {
            get { return r.Height; }
        }

        public int HeightInt
        {
            get { return (int) Math.Round(Height); }
        }

        public double Left
        {
            get { return r.Left; }
        }

        public int LeftInt
        {
            get { return (int) Math.Round(Left); }
        }

        public double Right
        {
            get { return r.Right; }
        }

        public int RightInt
        {
            get { return (int) Math.Round(Right); }
        }

        public double Top
        {
            get { return r.Top; }
        }

        public double Bottom
        {
            get { return r.Bottom; }
        }

        public Point LeftTop
        {
            get { return r.Location; }
        }

        public Size Size
        {
            get { return r.Size; }
        }

        public Point TopLeft
        {
            get { return r.TopLeft; }
        }

        public bool Contains(Point point)
        {
            return r.Contains(point.X, point.Y);
        }

        //we intentionally dont check the outerRect/minSize here
        public Rect Scale(double xFactor, double yFactor)
        {
            return new Rect(X*xFactor, Y*yFactor, Width*xFactor, Height*yFactor);
        }

        public Rect AddAbove(double delta)
        {
            return AddAbove(delta, null);
        }

        public Rect AddAbove(double delta, Size? minSize)
        {
            return SafeCreateRect(X, Y - delta, Width, Height + delta, minSize);
        }

        private Rect SafeCreateKeepSize(double x, double y)
        {
            double width = Width;
            double height = Height;
            if (outerRect != null)
            {
                if (x < outerRect.X)
                {
                    x = outerRect.X;
                }
                if (y < outerRect.Y)
                {
                    y = outerRect.Y;
                }
                if (x + width > outerRect.Right)
                {
                    x = outerRect.Right - width;
                }
                if (y + height > outerRect.Bottom)
                {
                    y = outerRect.Bottom - height;
                }
            }
            return new Rect(x, y, width, height, outerRect);
        }

        private Rect SafeCreateRect(double x, double y, double width, double height, Size? minSize)
        {
            //the order of the various checks is important
            if (minSize != null)
            {
                if (width < minSize.Value.Width || height < minSize.Value.Height)
                    return this;
            }
            if (outerRect != null)
            {
                if (x < outerRect.X)
                {
                    width -= outerRect.X - x;
                    x = outerRect.X;
                }
                if (y < outerRect.Y)
                {
                    height -= outerRect.Y - y;
                    y = outerRect.Y;
                }
                if (x + width > outerRect.Right) width = outerRect.Right - x;
                if (y + height > outerRect.Bottom) height = outerRect.Bottom - y;
            }
            return new Rect(x, y, width, height, outerRect);
        }

        public Rect AddBelow(double delta)
        {
            return AddBelow(delta, null);
        }

        public Rect AddBelow(double delta, Size? minSize)
        {
            return SafeCreateRect(X, Y, Width, Height + delta, minSize);
        }

        public Rect AddRight(double delta, Size? minSize)
        {
            return SafeCreateRect(X, Y, Width + delta, Height, minSize);
        }

        public Rect AddLeftBottom(double deltaX, double deltaY, Size minSize)
        {
            return SafeCreateRect(X - deltaX, Y, Width + deltaX, Height + deltaY, minSize);
        }

        public Rect AddLeft(double delta, Size? minSize)
        {
            return SafeCreateRect(X - delta, Y, Width + delta, Height, minSize);
        }

        public Rect AddRightBottom(double deltaX, double deltaY, Size? minSize)
        {
            return SafeCreateRect(X, Y, Width + deltaX, Height + deltaY, minSize);
        }

        public Rect AddLeftTop(double deltaX, double deltaY, Size? minSize)
        {
            return SafeCreateRect(X - deltaX, Y - deltaY, Width + deltaX, Height + deltaY, minSize);
        }

        public Rect AddRightTop(double deltaX, double deltaY, Size? minSize)
        {
            return SafeCreateRect(X, Y - deltaY, Width + deltaX, Height + deltaY, minSize);
        }

        public Rect AddAboveAndBelow(double deltaY)
        {
            return AddAboveAndBelow(deltaY, null);
        }

        public Rect AddAboveAndBelow(double deltaY, Size? minSize)
        {
            return SafeCreateRect(X, Y - deltaY, Width, Height + 2D*deltaY, minSize);
        }

        public Rect AddLeftAndRight(double deltaX, Size? minSize)
        {
            return SafeCreateRect(X - deltaX, Y, Width + 2D*deltaX, Height, minSize);
        }

        public Rect AddInAllDirections(double delta, Size? minSize)
        {
            return SafeCreateRect(X - delta, Y - delta, Width + 2D*delta, Height + 2D*delta, minSize);
        }

        public Rect MoveTo(double xPos, double yPos)
        {
            return SafeCreateKeepSize(xPos, yPos);
        }

        public Rect Move(double deltaX, double deltaY)
        {
            return SafeCreateKeepSize(X + deltaX, Y + deltaY);
        }

        public bool IntersectsWith(Rect rect)
        {
            return r.IntersectsWith(rect.r);
        }

        public override bool Equals(object o)
        {
            return Equals(o as Rect);
        }

        public bool Equals(Rect value)
        {
            if (value == null) return false;
            return r.Equals(value.r);
        }

        public static bool operator !=(Rect rect1, Rect rect2)
        {
            return !(rect1 == rect2);
        }

        public static bool operator ==(Rect rect1, Rect rect2)
        {
            if (ReferenceEquals(rect1, null)) return ReferenceEquals(rect2, null);
            return rect1.Equals(rect2);
        }

        public override int GetHashCode()
        {
            return r.GetHashCode();
        }

        public override string ToString()
        {
            return r.ToString();
        }
    }
}