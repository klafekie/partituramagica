#region usings

using System;
using System.Collections.Generic;
using System.Drawing;
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
		private readonly List<ClippingD3D> _clippingD3Ds = new List<ClippingD3D>();
		private readonly ApplicationConfiguration _configuration = new ApplicationConfiguration();
		private readonly IImageLoader _imageLoader;
		private readonly IPlayerController _playerController;

		private Direct3D _d3D;

		private Device _device;

		private bool _isRendering;

		public ScrollingPanel(IImageLoader imageLoader, IPlayerController playerController)
		{
			_imageLoader = imageLoader;
			_playerController = playerController;
		}

		public void Dispose()
		{
			_isRendering = false;
			_device?.Dispose();
			foreach (ClippingD3D l in _clippingD3Ds) l.Dispose();
			_clippingD3Ds.Clear();
			ClippingD3D.Cleanup();
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
		}

		public void StartScrolling(IPlayedPartitura partitura)
		{
			_clippingD3Ds.Clear();
			var paths = partitura.ImagePaths;
			foreach (Clipping c in _playerController.GetClippings())
			{
				string path = paths[c.PageIndex];
				IGreyImage image = _imageLoader.GetImage(path);
				ClippingD3D line = new ClippingD3D(image, c.Coords, _configuration.InvertColors, _configuration.Color);
				_clippingD3Ds.Add(line);
			}

			float offset = 0F;
			foreach (ClippingD3D l in _clippingD3Ds)
			{
				l.Init(_device, offset);
				offset += l.Width;
			}

			string[] parts = new string[partitura.PartPattern.Length];
			for (int i = 0; i < partitura.PartPattern.Length; i++) parts[i] = partitura.PartPattern[i].ToString();

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

			_device.Clear(ClearFlags.Target, _configuration.InvertColors ? Color.Black : _configuration.Color, 1.0f, 0);
			try
			{
				_device.BeginScene();

				_playerController.GetNextZoom(out float zoomX, out float zoomY);

				_device.SetTransform(TransformState.World,
					Matrix.Scaling(zoomX, zoomY, 1F) *
					Matrix.Translation((float) -_playerController.XPos * zoomX, 0, 0));

				foreach (ClippingD3D clipping in _clippingD3Ds)
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