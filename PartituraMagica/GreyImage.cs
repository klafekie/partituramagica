#region usings

using System.Windows.Media.Imaging;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica
{
	internal class GreyImage : IGreyImage
	{
		private byte[] _pixels;

		public BitmapImage RawImage { get; }

		public GreyImage(string path, BitmapImage bm)
		{
			RawImage = bm;
			Path = path;
		}

		public int Width => RawImage.PixelWidth;

		public int Height => RawImage.PixelHeight;

		public BrightnessGetter Brightness
		{
			get
			{
				if (_pixels == null)
				{
					int bytesPerPixel = RawImage.Format.BitsPerPixel / 8;
					// BitsPerPixel is normally 1 for grayscale; seen 32 for color image input
					_pixels = new byte[Width * Height * bytesPerPixel];
					int stride = (RawImage.PixelWidth * RawImage.Format.BitsPerPixel + 7) / 8;
					RawImage.CopyPixels(_pixels, stride, 0);
				}

				return GetBrightness;
			}
		}


		public double DpiX => RawImage.DpiX;

		public double DpiY => RawImage.DpiY;


		public string Path { get; }

		private double GetBrightness(int x, int y)
		{
			return (double) _pixels[y * Width + x] / 256F;
		}
	}
}