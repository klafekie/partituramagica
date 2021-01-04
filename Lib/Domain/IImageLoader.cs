namespace PartituraMagica.Lib.Domain
{
	public interface IImageLoader
	{
		IGreyImage GetImage(string path);
	}
}