#region usings

using JetBrains.Annotations;

#endregion

namespace PartituraMagica.Lib.Domain
{
	/// <summary>
	///   Represents a rectangular area on a certain input page.
	///   Related ClippingD3D will be generated for the rendering.
	/// </summary>
	public class Clipping
	{
		public int PageIndex { get; set; }

		public Rect Coords { get; private set; }

		/// <summary>
		///   ID of this object, used for the connection with the ClippingD3D
		///   and the editing objects on the drawing panel (Boxes?).
		/// </summary>
		public int Id { get; }

		public Clipping(int pageIndex, Rect coords, int id)
		{
			PageIndex = pageIndex;
			Coords = coords;
			Id = id;
		}

		public Clipping(Clipping rhs)
		{
			PageIndex = rhs.PageIndex;
			Coords = rhs.Coords;
			Id = rhs.Id;
		}

		//c'tor for testing
		[UsedImplicitly]
		public Clipping(int pageIndex, double x, double y, double width, double height)
		{
			PageIndex = pageIndex;
			Coords = new Rect(x, y, width, height);
			Id = -1;
		}

		public void AddAboveAndBelow(double deltaY)
		{
			Coords = Coords.AddAboveAndBelow(deltaY);
		}

		public void MoveTo(double xPos, double yPos)
		{
			Coords = Coords.MoveTo(xPos, yPos);
		}

		public override string ToString()
		{
			return Coords.ToString();
		}
	}
}