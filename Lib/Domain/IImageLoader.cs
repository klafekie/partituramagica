namespace PartituraMagica.Domain
{
	public interface IImageLoader
	{
		IGreyImage GetImage(string path);
	}
}