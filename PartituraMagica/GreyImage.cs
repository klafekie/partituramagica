using System.Windows.Media.Imaging;

namespace PartituraMagica
{
    internal class GreyImage : IGreyImage
    {
        private byte[] pixels;

        public GreyImage(string path, BitmapImage bm)
        {
            RawImage = bm;
            Path = path;
        }

        public BitmapImage RawImage { get; }

        public int Width
        {
            get { return RawImage.PixelWidth; }
        }

        public int Height
        {
            get { return RawImage.PixelHeight; }
        }

        public BrightnessGetter Brightness
        {
            get
            {
                if (pixels == null)
                {
                    // Quick fix for crash on color image input. TODO p3: should similar corrections needed elsewhere?
                    int bytesPerPixel = RawImage.Format.BitsPerPixel/8;
                        // BitsPerPixel is normally 1 for grayscale; seen 32 for color image input [jed]
                    pixels = new byte[Width*Height*bytesPerPixel];
                    int stride = (RawImage.PixelWidth*RawImage.Format.BitsPerPixel + 7)/8;
                    RawImage.CopyPixels(pixels, stride, 0);
                }
                return GetBrightness;
            }
        }


        public double DpiX
        {
            get { return RawImage.DpiX; }
        }

        public double DpiY
        {
            get { return RawImage.DpiY; }
        }


        public string Path { get; }

        private double GetBrightness(int x, int y)
        {
            return (double) pixels[y*Width + x]/256F;
        }
    }
}