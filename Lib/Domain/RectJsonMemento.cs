namespace PartituraMagica.Lib.Domain
{
	public class RectJsonMemento
	{
		public RectJsonMemento OuterRect { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public double? CenterY { get; set; }
	}
}