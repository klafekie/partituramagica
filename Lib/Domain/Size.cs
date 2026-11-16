using System;

namespace PartituraMagica
{
	/// <summary>
	/// A platform independent 2D size (replacement for System.Windows.Size).
	/// </summary>
	[Serializable]
	public readonly struct Size : IEquatable<Size>
	{
		public Size(double width, double height)
		{
			Width = width;
			Height = height;
		}

		public double Width { get; }
		public double Height { get; }

		public bool Equals(Size other) => Width.Equals(other.Width) && Height.Equals(other.Height);

		public override bool Equals(object obj) => obj is Size other && Equals(other);

		public override int GetHashCode() => unchecked(Width.GetHashCode() * 397) ^ Height.GetHashCode();

		public static bool operator ==(Size left, Size right) => left.Equals(right);

		public static bool operator !=(Size left, Size right) => !left.Equals(right);

		public override string ToString() => $"{Width}x{Height}";
	}
}
