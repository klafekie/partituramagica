using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace PartituraMagica
{
	class GreyImage : IGreyImage
	{
		private BitmapImage bm;
		private byte[] pixels = null;
		private string path;
		public GreyImage(string path, BitmapImage bm) {
			this.bm = bm;
			this.path = path;
		}

		public int Width {
			get { return this.bm.PixelWidth; }
		}

		public int Height {
			get { return this.bm.PixelHeight; }
		}

		private double GetBrightness(int x, int y) {
			return (double)(this.pixels[y * this.Width + x]) / 256F;
		}

		public BrightnessGetter Brightness {
			get {
				if (this.pixels == null) {
					this.pixels = new byte[this.Width * this.Height];
					int stride = (bm.PixelWidth * bm.Format.BitsPerPixel + 7) / 8;
					this.bm.CopyPixels(this.pixels, stride, 0);
				}
				return GetBrightness;
			}
		}


		public double DpiX {
			get { return this.bm.DpiX; }
		}

		public double DpiY {
			get { return this.bm.DpiY; }
		}


		public string Path {
			get { return this.path; }
		}

		public BitmapImage RawImage {
			get { return bm;  }
		}
	}
}
