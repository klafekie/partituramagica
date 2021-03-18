#region usings

using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Test
{
	internal class FakeImage : IGreyImage
	{
		public int Width => 1000;

		public int Height => 3000;

		public BrightnessGetter Brightness { get; set; } = (x, y) => 0D;


		public double DpiX => 96D;

		public double DpiY => 96D;


		public string Path => "any.jpg";
	}
}