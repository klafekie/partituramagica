namespace PartituraMagica.Lib.Domain
{
	public interface IImageLoader
	{
		IGreyImage LoadImage(string path);
	}
}