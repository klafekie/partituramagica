using System;

namespace PartituraMagica
{
	/// <summary>
	/// A platform independent 2D point (replacement for System.Windows.Point).
	/// </summary>
	[Serializable]
	public readonly struct Point : IEquatable<Point>
	{
		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X { get; }
		public double Y { get; }

		public bool Equals(Point other) => X.Equals(other.X) && Y.Equals(other.Y);

		public override bool Equals(object obj) => obj is Point other && Equals(other);

		public override int GetHashCode() => unchecked(X.GetHashCode() * 397) ^ Y.GetHashCode();

		public static bool operator ==(Point left, Point right) => left.Equals(right);

		public static bool operator !=(Point left, Point right) => !left.Equals(right);

		public override string ToString() => $"({X},{Y})";
	}
}
