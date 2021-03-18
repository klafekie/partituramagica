#region usings

using System;
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

		private readonly Direct3D _d3D;
		private readonly ConcurrentQueue<(Device, Texture[])> _disposables;

		private readonly ConcurrentDictionary<string, (bool isRequested, Texture texture)> _textures;


		private Device _device;
		private State _state;

		public TextureCache()
		{
			_state = State.Idle;
			_textures = new ConcurrentDictionary<string, (bool, Texture)>();
			_disposables = new ConcurrentQueue<(Device, Texture[])>();
			_d3D = new Direct3D();

			Task.Run(WorkerLoop);
		}

		public void Dispose()
		{
			Trace.TraceInformation("disposing TextureCache");
			_state = State.Stopping;
			//disposing is done inside the worker loop to make sure that a device is not disposed while a 
			//texture is being loaded.
			while (_state != State.Stopped) Thread.Sleep(100);
			Trace.TraceInformation("finished disposing TextureCache");
		}

		public void AddImage(string imageFile)
		{
			Trace.TraceInformation("Adding image file: " + imageFile);
			_textures.TryAdd(imageFile, (isRequested: false, texture: null));
		}

		public void ClearAllImages()
		{
			Trace.TraceInformation("clear existing images");
			//only enqueue the textures for disposing
			_disposables.Enqueue((null, _textures.Values.Select(v => v.texture).ToArray()));
			_textures.Clear();
		}

		public object CreateDevice(object handle)
		{
			Trace.TraceInformation("Creating device");
			if (_device != null)
			{
				Trace.TraceInformation("enqueuing disposable device");
				_disposables.Enqueue((_device, _textures.Values.Select(v => v.texture).ToArray()));
				//make a copy before ordering to avoid concurrency issues
				var knownImages = _textures
					.ToArray()
					.OrderByDescending(pair => pair.Value.isRequested)
					.ThenBy(pair => pair.Key)
					.Select(pair => pair.Key)
					.ToArray();

				_textures.Clear();
				foreach (string image in knownImages) AddImage(image);
			}

			/*
			 * back buffer should have at least the same dimensions as the used display adapter.
			 */
			int bbWidth = _d3D.Adapters.Select(a => a.CurrentDisplayMode.Width).Max();
			int bbHeight = _d3D.Adapters.Select(a => a.CurrentDisplayMode.Height).Max();

			_device = new Device(_d3D, _d3D.Adapters.DefaultAdapter.Adapter,
				DeviceType.Hardware, (IntPtr) handle, CreateFlags.HardwareVertexProcessing,
				new PresentParameters
				{
					BackBufferWidth = bbWidth,
					BackBufferHeight = bbHeight,
					PresentationInterval = PresentInterval.One,
					Multisample = MultisampleType.None,
					Windowed = true,
					SwapEffect = SwapEffect.Discard
				});

			_device.SetRenderState(RenderState.CullMode, Cull.None);
			_device.SetRenderState(RenderState.Lighting, false);
			_device.SetRenderState(RenderState.Clipping, false);
			_device.SetRenderState(RenderState.AlphaBlendEnable, false);
			// Antialiasing: see http://msdn.microsoft.com/en-us/library/windows/desktop/bb173422%28v=vs.85%29.aspx
			// seems that it is not doing anything when working with images, only for shapes
			_device.SetRenderState(RenderState.MultisampleAntialias, false);

			return _device;
		}

		public object GetTexture(string image)
		{
			if (!_textures.TryGetValue(image, out var tuple))
			{
				Trace.TraceError("Call AddImage() before trying to get the corresponding texture");
				return null;
			}

			if (tuple.texture != null) return tuple.texture;

			Trace.TraceInformation("Requesting priority for: " + image);
			//this will update the value in dictionary only if it is still (false, null).
			//so this doesn't harm if in the meantime the worker thread has set the texture.
			_textures.TryUpdate(image, (true, null), (false, null));

			int attempts = 0;
			do
			{
				tuple = _textures[image];
				if (tuple.texture != null) return tuple.texture;
				Trace.TraceInformation("GetTexture() waiting for: " + image);
				attempts++;
				Thread.Sleep(100);
			} while (attempts < 150);

			return null; //give up
		}

		private void WorkerLoop()
		{
			try
			{
				while (_state != State.Stopping)
				{
					var keyValuePairs = _textures
						.ToArray()
						.OrderByDescending(pair => pair.Value.isRequested)
						.ThenBy(pair => pair.Key);
					string nextImage = keyValuePairs
						.FirstOrDefault(pair => pair.Value.texture == null).Key;

					if (nextImage != null && _device != null)
					{
						Trace.TraceInformation("Cache.Get() loading texture from file: " + nextImage);
						Filter filter = Filter.Linear | Filter.Dither;
						Pool pool = _device is DeviceEx ? Pool.Default : Pool.Managed;
						Texture texture = Texture.FromFile(_device, nextImage, -1, -1, -1, Usage.None, Format.Unknown, pool,
							filter, Filter.None, 0);
						Trace.TraceInformation("successfully created texture");

						if (_device == texture.Device && _state == State.Idle)
						{
							//only use the texture if current device hasn't changed and we are not about to dispose the texture cache
							Trace.TraceInformation("Using Texture");
							_textures[nextImage] = (false, texture);
						}
						else
						{
							Trace.TraceInformation("Disposing Texture");
							texture.Dispose();
						}
					}

					DisposeOldStuff();
					Thread.Sleep(10);
				}

				Trace.TraceInformation("left workerloop.");
				Trace.TraceInformation(
					$"disposing {_textures.Values.Count(t => t.texture != null)} textures and current device");
				foreach (Texture texture in _textures.Values.Select(v => v.texture)) texture?.Dispose();
				DisposeOldStuff();
				_textures.Clear();
				_device?.Dispose();
				_d3D.Dispose();
			}
			finally
			{
				_state = State.Stopped;
			}

			void DisposeOldStuff()
			{
				while (_disposables.TryDequeue(out (Device, Texture[]) result))
				{
					Trace.TraceInformation($"Disposing {result.Item2.Count(t => t != null)} textures");
					foreach (Texture texture1 in result.Item2) texture1?.Dispose();

					if (result.Item1 != null)
					{
						Trace.TraceInformation("Disposing device");
						result.Item1.Dispose();
					}
				}
			}
		}
	}
}