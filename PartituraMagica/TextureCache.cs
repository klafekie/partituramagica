#region usings

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using PartituraMagica.Lib.Playing;
using Vortice.Direct3D9;

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

		private readonly IDirect3D9 _d3D;
		private readonly ConcurrentQueue<(IDirect3DDevice9, IDirect3DTexture9[])> _disposables;
		private readonly ConcurrentDictionary<string, (bool isRequested, IDirect3DTexture9 texture)> _textures;

		private IDirect3DDevice9 _device;
		private State _state;

		public TextureCache()
		{
			_state = State.Idle;
			_textures = new ConcurrentDictionary<string, (bool, IDirect3DTexture9)>();
			_disposables = new ConcurrentQueue<(IDirect3DDevice9, IDirect3DTexture9[])>();
			_d3D = D3D9.Direct3DCreate9();

			Task.Run(WorkerLoop);
		}

		public void Dispose()
		{
			Trace.TraceInformation("disposing TextureCache");
			_state = State.Stopping;
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
				var knownImages = _textures
					.ToArray()
					.OrderByDescending(pair => pair.Value.isRequested)
					.ThenBy(pair => pair.Key)
					.Select(pair => pair.Key)
					.ToArray();

				_textures.Clear();
				foreach (string image in knownImages) AddImage(image);
			}

			int bbWidth = 0, bbHeight = 0;
			for (uint i = 0; i < _d3D.AdapterCount; i++)
			{
				var mode = _d3D.GetAdapterDisplayMode(i);
				bbWidth = Math.Max(bbWidth, (int)mode.Width);
				bbHeight = Math.Max(bbHeight, (int)mode.Height);
			}

			var pp = new PresentParameters
			{
				BackBufferWidth = (uint)bbWidth,
				BackBufferHeight = (uint)bbHeight,
				PresentationInterval = PresentInterval.One,
				MultiSampleType = MultisampleType.None,
				Windowed = true,
				SwapEffect = SwapEffect.Discard
			};
			_device = _d3D.CreateDevice(0U, DeviceType.Hardware, (IntPtr)handle,
				CreateFlags.HardwareVertexProcessing, pp);

			_device.SetRenderState(RenderState.CullMode, Cull.None);
			_device.SetRenderState(RenderState.Lighting, false);
			_device.SetRenderState(RenderState.Clipping, true);
			_device.SetRenderState(RenderState.AlphaBlendEnable, false);
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

			return null;
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
						var loadingDevice = _device;
						Pool pool = loadingDevice is IDirect3DDevice9Ex ? Pool.Default : Pool.Managed;
						IDirect3DTexture9 texture = LoadTexture(loadingDevice, nextImage, pool);
						Trace.TraceInformation("successfully created texture");

						if (_device == loadingDevice && _state == State.Idle)
						{
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
				foreach (IDirect3DTexture9 t in _textures.Values.Select(v => v.texture)) t?.Dispose();
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
				while (_disposables.TryDequeue(out (IDirect3DDevice9, IDirect3DTexture9[]) result))
				{
					Trace.TraceInformation($"Disposing {result.Item2.Count(t => t != null)} textures");
					foreach (IDirect3DTexture9 t in result.Item2) t?.Dispose();

					if (result.Item1 != null)
					{
						Trace.TraceInformation("Disposing device");
						result.Item1.Dispose();
					}
				}
			}
		}

		private static IDirect3DTexture9 LoadTexture(IDirect3DDevice9 device, string path, Pool pool)
		{
			using var bitmap = new System.Drawing.Bitmap(path);
			int w = bitmap.Width, h = bitmap.Height;
			var bmpData = bitmap.LockBits(
				new System.Drawing.Rectangle(0, 0, w, h),
				System.Drawing.Imaging.ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			// D3DFMT_A8R8G8B8 and PixelFormat.Format32bppArgb are both BGRA in memory on x86.
			var texture = device.CreateTexture((uint)w, (uint)h, 1, Usage.None, Format.A8R8G8B8, pool);
			var locked = texture.LockRect(0, LockFlags.None);

			int srcStride = Math.Abs(bmpData.Stride);
			var row = new byte[srcStride];
			for (int y = 0; y < h; y++)
			{
				Marshal.Copy(bmpData.Scan0 + y * bmpData.Stride, row, 0, srcStride);
				Marshal.Copy(row, 0, locked.DataPointer + y * locked.Pitch, srcStride);
			}

			texture.UnlockRect(0);
			bitmap.UnlockBits(bmpData);
			return texture;
		}
	}
}
