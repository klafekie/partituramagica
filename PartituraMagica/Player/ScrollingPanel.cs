#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;
using SlimDX;
using SlimDX.Direct3D9;

#endregion

namespace PartituraMagica.App.Player
{
	internal class ScrollingPanel : IScrollingPanel
	{
		private const double CLIPPING_LOAD_BUFFER = 2000;
		private readonly List<ClippingD3D> _clippingD3Ds = new List<ClippingD3D>();
		private readonly ApplicationConfiguration _configuration = new ApplicationConfiguration();
		private readonly IImageLoader _imageLoader;
		private readonly IPlayerController _playerController;
		private readonly ITextureCache _textureCache;

		private Direct3D _d3D;

		private Device _device;

		private bool _isRendering;

		public ScrollingPanel(IImageLoader imageLoader, IPlayerController playerController, ITextureCache textureCache)
		{
			_imageLoader = imageLoader;
			_playerController = playerController;
			_textureCache = textureCache;
		}

		public void Dispose()
		{
			_isRendering = false;
			foreach (ClippingD3D l in _clippingD3Ds) l.Dispose();
			_textureCache.SetDevice(null);
			_device?.Dispose();
			_clippingD3Ds.Clear();
			_d3D?.Dispose();
		}

		public void Init(IntPtr handle)
		{
			_d3D = new Direct3D();
			/* Experiment jed 20141227, setting BackBuffer size explicitly. Without that, seen that device.ViewPort size was 640 * 480 pixels.
			 * Setting the size to the monitor resolution improved quality a lot. But maybe it should be the exact #pixels in the player panel?
			 * Values: Laptop jed: 1600*900. Monitor jed: 1680*1050.
			 */
			const int bbWidth = 1600;
			const int bbHeight = 900;

			_device = new Device(_d3D, _d3D.Adapters.DefaultAdapter.Adapter,
				DeviceType.Hardware, handle, CreateFlags.HardwareVertexProcessing,
				new PresentParameters
				{
					// Added BackBufferWidth and BackBufferHeight as experiment for improving vision quality [jed])
					BackBufferWidth = bbWidth,
					BackBufferHeight = bbHeight,
					PresentationInterval = PresentInterval.One,
					Multisample = MultisampleType.None,
					// Options are: None, NonMaskable, and NSamples where N in 2..16.
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
			_device.Clear(ClearFlags.Target, _configuration.InvertColors ? Color.Black : _configuration.Color, 1.0f, 0);

			_textureCache.SetDevice(_device);
		}

		public void StartScrolling(IPlayedPartitura partitura)
		{
			Trace.TraceInformation("PartituraMagica.App.Player.ScrollingPanel.StartScrolling()");
			_clippingD3Ds.Clear();
			var paths = partitura.ImagePaths;
			foreach (Clipping c in _playerController.GetClippings())
			{
				string path = paths[c.PageIndex];
				IGreyImage image = _imageLoader.GetImage(path);
				ClippingD3D line = new ClippingD3D(image, c.Coords, _configuration, _textureCache);
				_clippingD3Ds.Add(line);
			}

			Trace.TraceInformation("PartituraMagica.App.Player.ScrollingPanel.StartScrolling.beforeInit clippings");

			float offset = 0F;
			foreach (ClippingD3D l in _clippingD3Ds)
			{
				l.Init(_device, offset);
				offset += l.Width;
			}

			string[] parts = new string[partitura.PartPattern.Length];
			for (int i = 0; i < partitura.PartPattern.Length; i++) parts[i] = partitura.PartPattern[i].ToString();

			Trace.TraceInformation("PartituraMagica.App.Player.ScrollingPanel.StartScrolling.isRendering");

			_isRendering = true;

			do
			{
				Application.DoEvents();
				Render();
			} while (_isRendering);
		}

		private void Render()
		{
			if (_device == null) return;

			if (!_isRendering)
				return;

			try
			{
				_device.Clear(ClearFlags.Target, _configuration.InvertColors ? Color.Black : _configuration.Color, 1.0f, 0);

				_device.BeginScene();

				_playerController.GetNextZoom(out float zoomX, out float zoomY);

				_device.SetTransform(TransformState.World,
					Matrix.Scaling(zoomX, zoomY, 1F) *
					Matrix.Translation((float) -_playerController.XPos * zoomX, 0, 0));

				var visibleClippings = _clippingD3Ds.Where(clip =>
					clip.XPos >= _playerController.XPos - CLIPPING_LOAD_BUFFER &&
					clip.XPos <= _playerController.XPos + CLIPPING_LOAD_BUFFER);
				foreach (ClippingD3D clipping in visibleClippings)
					clipping.Render(_device);
			}
			finally
			{
				_device.EndScene();
			}

			_device.Present();

			_playerController.ScrollForward();
		}
	}
}