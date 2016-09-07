namespace PartituraMagica
{
    public delegate double BrightnessGetter(int x, int y);

    /**
     * An abstraction for a pixel based grey bitmap
     */

    public interface IGreyImage
    {
        int Width { get; }
        int Height { get; }
        BrightnessGetter Brightness { get; }
        double DpiX { get; }
        double DpiY { get; }

        string Path { get; }
    }
}