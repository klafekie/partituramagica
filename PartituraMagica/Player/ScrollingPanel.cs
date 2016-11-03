using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;

namespace PartituraMagica.Player
{
    internal class ScrollingPanel : IDisposable
    {
        private readonly List<ClippingD3D> _clippingD3Ds = new List<ClippingD3D>();
        private readonly ApplicationConfiguration _configuration = new ApplicationConfiguration();

        private readonly Direct3D _d3D;
        private readonly IImageLoader _imageLoader;
        private readonly IPartNavigator _navigator;
        private readonly IntPtr _ownerHandle;
        private readonly IPageFiles _pageFiles;

        private readonly IntPtr _panelHandle;
        private readonly PlayerPositionHolder _positionHolder;
        private readonly SpeedManager _speedManager;
        private readonly ZoomManager _zoomManager;

        private Device _device;

        private bool _isRendering;
        private float _lastZoomX;

        public ScrollingPanel(IPageFiles pageFiles, IImageLoader imageLoader, IntPtr owner, IntPtr panelHandle,
            PlayerPositionHolder positionHolder, IPartNavigator navigator, SpeedManager speedManager,
            ZoomManager zoomManager)
        {
            _pageFiles = pageFiles;
            _imageLoader = imageLoader;
            _ownerHandle = owner;
            _panelHandle = panelHandle;
            _positionHolder = positionHolder;
            _navigator = navigator;
            _speedManager = speedManager;
            _zoomManager = zoomManager;

            _d3D = new Direct3D();
            CreateDevice();
        }

        public void Dispose()
        {
            _isRendering = false;
            _device?.Dispose();
            foreach (ClippingD3D l in _clippingD3Ds)
            {
                l.Dispose();
            }
            ClippingD3D.Cleanup();
            _d3D?.Dispose();
        }

        public void Init(string partPattern, IEnumerable<Clipping> clips, double lineDistance)
        {
            _clippingD3Ds.Clear();
            foreach (Clipping c in clips)
            {
                string path = _pageFiles.GetImagePath(c.PageIndex);
                IGreyImage image = _imageLoader.GetImage(path);
                ClippingD3D line = new ClippingD3D(image, c.Coords, _configuration.InvertColors, _configuration.Color);
                _clippingD3Ds.Add(line);
            }

            float offset = 0F;
            foreach (ClippingD3D l in _clippingD3Ds)
            {
                l.Init(_device, offset);
                offset += l.Width + (float) lineDistance;
            }

            string[] parts = new string[partPattern.Length];
            for (int i = 0; i < partPattern.Length; i++)
            {
                parts[i] = partPattern[i].ToString();
            }

            _positionHolder.XPos = 0;
        }

        public void StartScrolling()
        {
            _isRendering = true;

            do
            {
                Application.DoEvents();
                Render();
            } while (_isRendering);
        }

        private void CreateDevice()
        {
            /* Experiment jed 20141227, setting BackBuffer size explicitly. Without that, seen that device.ViewPort size was 640 * 480 pixels.
             * Setting the size to the monitor resolution improved quality a lot. But maybe it should be the exact #pixels in the player panel?
             * Values: Laptop jed: 1600*900. Monitor jed: 1680*1050.
             */
            const int bbWidth = 1600;
            const int bbHeight = 900;

            _device = new Device(_d3D, _d3D.Adapters.DefaultAdapter.Adapter,
                DeviceType.Hardware, _panelHandle, CreateFlags.HardwareVertexProcessing,
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

        private void Render()
        {
            //using (TraceEntry e = SimpleTracer.Start("Render()")) {
            //	int tick = Environment.TickCount;
            if (_device == null)
            {
                return;
            }

            if (!_isRendering)
            {
                //Debug.Write(".");
                return;
            }

            _device.Clear(ClearFlags.Target, _configuration.InvertColors ? Color.Black : _configuration.Color, 1.0f, 0);
            try
            {
                _device.BeginScene();
                float zoomY;
                _zoomManager.GetNextZoom(out _lastZoomX, out zoomY);
                _device.SetTransform(TransformState.World,
                    Matrix.Scaling(_lastZoomX, zoomY, 1F) *
                    Matrix.Translation((float) -_positionHolder.XPos * _lastZoomX, 0, 0));
                for (int i = 0; i < _clippingD3Ds.Count; i++)
                {
                    _clippingD3Ds[i].Render(_device);
                }
            }
            finally
            {
                _device.EndScene();
            }

            _device.Present();

            if (!_speedManager.IsPaused)
            {
                if (!_navigator.IsPartituraFinished())
                {
                    _positionHolder.ScrollForwardBy(_speedManager.Speed);
                }
            }
            //}
        }
    }
}