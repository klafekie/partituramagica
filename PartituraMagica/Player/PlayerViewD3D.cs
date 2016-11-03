using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PartituraMagica.Player;
using SlimDX;
using SlimDX.Direct3D9;

namespace PartituraMagica
{
    /**
     * Uses Direct3D to implement a smooth scrolling view.
     * Contains custom controls for navigating, zooming, speed control
     * 
     * For performance reasons the view is not disposed when the user
     * closes it. Instead it is just hidden.
     */

    public partial class PlayerViewD3D : Form, IPlayerView
    {
        private readonly List<ClippingD3D> _clippingD3Ds = new List<ClippingD3D>();

        private readonly ApplicationConfiguration _configuration = new ApplicationConfiguration();
        private readonly Direct3D _d3D;
        private readonly IImageLoader _imageLoader;
        private readonly KeyboardMap _keyboardMap;
        private readonly IntPtr _ownerHandle;
        private readonly IPageFiles _pageFiles;
        private readonly SpeedManager _speedManager;
        private readonly ZoomManager _zoomManager;
        private bool _closed = true;

        private Device _device;
        private float _lastZoomX;
        private IPartNavigator _navigator;

        private bool _rendering;

        //position in view specific coordinates		

        public PlayerViewD3D(IPageFiles pageFiles, IImageLoader imageLoader, IntPtr owner, PlayerMemento memento)
        {
            Trace.WriteLine("ctor of PlayerViewD3D");
            InitializeComponent();
            WindowState = FormWindowState.Maximized;

            _speedManager = new SpeedManager(memento.Speed);
            _zoomManager = new ZoomManager(memento.XZoom, memento.YZoom);
            _keyboardMap = new KeyboardMap(_speedManager, _zoomManager);
            _pageFiles = pageFiles;
            _imageLoader = imageLoader;
            _ownerHandle = owner;

            _d3D = new Direct3D();
            CreateDevice();
        }

        public double XPos { get; set; }

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
            partsStrip.Parts = parts;

            XPos = 0;
        }

        public void StartScrolling()
        {
            IWin32Window w = new Win32Window(_ownerHandle);
            _rendering = true;
            _speedManager.IsPaused = false;
            _closed = false;
            Enabled = true;
            Show(w);

            do
            {
                Application.DoEvents();
                Render();
            } while (!_closed);
            Enabled = false;
        }

        public event Action ViewHasScrolled;

        public PlayerMemento ToMemento()
        {
            return new PlayerMemento
            {
                Speed = _speedManager.Speed,
                XZoom = _zoomManager.XZoom,
                YZoom = _zoomManager.YZoom
            };
        }

        public void RegisterEvents(IPartNavigator navigator)
        {
            _navigator = navigator;
            _keyboardMap.RegisterEvents(navigator, this);
            partsStrip.RegisterEvents(navigator);
            speedStrip.RegisterEvents(navigator, _speedManager);
            zoomStrip.RegisterEvents(_zoomManager);
        }

        /** behave as if window was closed, but it is still in memory */

        public new void Hide()
        {
            base.Hide();
            _closed = true;
        }

        protected override void Dispose(bool disposing)
        {
            _rendering = false;
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);

            _device?.Dispose();
            foreach (ClippingD3D l in _clippingD3Ds)
            {
                l.Dispose();
            }
            ClippingD3D.Cleanup();
            _d3D?.Dispose();
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
                DeviceType.Hardware, panel1.Handle, CreateFlags.HardwareVertexProcessing,
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

            if (!_rendering)
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
                    Matrix.Translation((float) -XPos * _lastZoomX, 0, 0));
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
                    if (!_speedManager.IsPaused)
                    {
                        XPos += _speedManager.Speed;
                        ViewHasScrolled?.Invoke();
                    }
                }
            }
            //}
        }

        private void PlayerViewD3D_KeyDown(object sender, KeyEventArgs e)
        {
            _keyboardMap.HandleKeyDown(sender, e);
        }

        private void PlayerViewD3D_KeyUp(object sender, KeyEventArgs e)
        {
            _keyboardMap.HandleKeyUp(sender, e);
        }

        private void PlayerViewD3D_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closed = true;
            switch (e.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                case CloseReason.FormOwnerClosing:
                case CloseReason.MdiFormClosing:
                case CloseReason.None:
                case CloseReason.TaskManagerClosing:
                case CloseReason.WindowsShutDown:
                    break; //no cancel
                case CloseReason.UserClosing:
                    e.Cancel = true; //window is not really closed.
                    Hide();
                    break;
            }
        }

        private class Win32Window : IWin32Window
        {
            public Win32Window(IntPtr w)
            {
                Handle = w;
            }

            public IntPtr Handle { get; }
        }
    }
}