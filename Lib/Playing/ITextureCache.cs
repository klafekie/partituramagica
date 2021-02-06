#region usings

using System;

#endregion

namespace PartituraMagica.Lib.Playing
{
	public interface ITextureCache : IDisposable
	{
		//block until the needed texture is loaded
		object GetTexture(string imagePath);

		//set current device. Do not dispose device but all associated textures
		//start preloading all currently known images for new device
		//none blocking call
		void SetDevice(object device);

		//add given image and start preloading. none blocking call
		void AddTexture(string imagePath);
		void ClearAllTextures();
	}
}