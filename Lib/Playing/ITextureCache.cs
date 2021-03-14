#region usings

using System;

#endregion

namespace PartituraMagica.Lib.Playing
{
	public interface ITextureCache : IDisposable
	{
		/// <summary>
		///   block until the needed texture is loaded
		/// </summary>
		/// <param name="imagePath"></param>
		/// <returns>A Direct3D Texture</returns>
		object GetTexture(string imagePath);

		/// <summary>
		///   create a Direct3D Device.
		///   dispose any previously created devices and textures accosiated with the old device
		/// </summary>
		/// <param name="handle">ControlHandle for the device</param>
		/// <returns>A Direct3D.Device</returns>
		object CreateDevice(object handle);

		/// <summary>
		///   add given image and start preloading. none blocking call
		/// </summary>
		/// <param name="imagePath"></param>
		void AddImage(string imagePath);

		/// <summary>
		///   Clear all existing images and dispose any associated textures.
		///   The device is not disposed.
		/// </summary>
		void ClearAllImages();
	}
}