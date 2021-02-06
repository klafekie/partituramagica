#region usings

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PartituraMagica.Lib.Playing;
using SlimDX.Direct3D9;

#endregion

namespace PartituraMagica.App
{
	public class TextureCache : ITextureCache
	{
		private enum State
		{
			Idle,
			Stopping,
			Stopped
		}

		private readonly ConcurrentDictionary<string, Texture> _textures;

		private Device _device;
		private bool _isLoading;
		private State _state;

		public TextureCache()
		{
			_state = State.Idle;
			_textures = new ConcurrentDictionary<string, Texture>();
			Task.Run(WorkerLoop);
		}

		public void Dispose()
		{
			_state = State.Stopping;
			while (_isLoading || _state != State.Stopped) Thread.Sleep(100);
			ClearAndDisposeCache();
		}

		public void AddTexture(string imageFile)
		{
			_textures.TryAdd(imageFile, null);
		}

		public void ClearAllTextures()
		{
			while (_isLoading) Thread.Sleep(100);
			ClearAndDisposeCache();
		}

		public object GetTexture(string image)
		{
			//Trace.TraceInformation("Cache.Get() looking for: " + image);
			int attempts = 0;
			Texture texture;
			while ((!_textures.TryGetValue(image, out texture) || texture == null) && attempts < 100)
			{
				Trace.TraceInformation("Cache.Get() waiting for: " + image);
				attempts++;
				Thread.Sleep(100);
			}

			//Trace.TraceInformation("Cache.Get() found: " + image);
			return texture;
		}

		public void SetDevice(object device)
		{
			if (_device != null)
			{
				var knownImages = _textures.Keys.ToArray();
				ClearAllTextures();
				foreach (string image in knownImages) AddTexture(image);
			}

			_device = (Device) device;
		}

		private void ClearAndDisposeCache()
		{
			foreach (Texture texture in _textures.Values) texture?.Dispose();
			_textures.Clear();
			_device = null;
		}

		private void WorkerLoop()
		{
			try
			{
				while (_state != State.Stopping)
				{
					string nextImage = _textures
						.OrderBy(pair => pair.Key)
						.FirstOrDefault(pair => pair.Value == null).Key;
					if (nextImage == null || _device == null)
					{
						Thread.Sleep(100);
						continue;
					}

					_isLoading = true;
					Trace.TraceInformation("Cache.Get() loading texture from file: " + nextImage);
					Filter filter = Filter.Linear | Filter.Dither;
					Pool pool = _device is DeviceEx ? Pool.Default : Pool.Managed;
					Texture texture = Texture.FromFile(_device, nextImage, -1, -1, -1, Usage.None, Format.Unknown, pool,
						filter, Filter.None, 0);
					Trace.TraceInformation("success");
					_textures.TryUpdate(nextImage, texture, null);
					_isLoading = false;
				}
			}
			finally
			{
				_isLoading = false;
				_state = State.Stopped;
			}
		}
	}
}