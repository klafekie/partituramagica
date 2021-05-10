#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PartituraMagica.Lib;
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
		private readonly IApplicationConfiguration _configuration;
		private readonly IImageLoader _imageLoader;
		private readonly IPlayerController _playerController;
		private readonly ITextureCache _textureCache;

		private Device _device;

		private bool _isRendering;

		public ScrollingPanel(IImageLoader imageLoader, IPlayerController playerController, ITextureCache textureCache,
			IApplicationConfiguration configuration)
		{
			_imageLoader = imageLoader;
			_playerController = playerController;
			_textureCache = textureCache;
			_configuration = configuration;
		}

		public void Dispose()
		{
			_isRendering = false;
			foreach (ClippingD3D l in _clippingD3Ds) l.Dispose();
			_clippingD3Ds.Clear();
		}

		public void Init(IntPtr handle)
		{
			_device = (Device)_textureCache.CreateDevice(handle);
			_device.Clear(ClearFlags.Target, _configuration.InvertColors ? Color.Black : Color.FromArgb(_configuration.Color),
				1.0f, 0);
		}

		public void StartScrolling(IPlayedPartitura partitura)
		{
			Trace.TraceInformation("PartituraMagica.App.Player.ScrollingPanel.StartScrolling()");
			_clippingD3Ds.Clear();
			var paths = partitura.ImagePaths;
			foreach (Clipping c in _playerController.GetClippings())
			{
				string path = paths[c.PageIndex];
				IGreyImage image = _imageLoader.LoadImage(path);
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
				_device.Clear(ClearFlags.Target,
					_configuration.InvertColors ? Color.Black : Color.FromArgb(_configuration.Color), 1.0f, 0);

				_device.BeginScene();

				_playerController.GetNextZoomAndYPosition(out float zoomX, out float zoomY, out float yPosition);

				_device.SetTransform(TransformState.World,
					Matrix.Scaling(zoomX, zoomY, 1F) *
					Matrix.Translation((float)-_playerController.XPos * zoomX, yPosition, 0));

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