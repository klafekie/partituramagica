using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
        private readonly IntPtr _ownerHandle;
        private readonly IPageFiles _pageFiles;
        private bool _closed = true;

        private Device _device;
        private float _lastZoomX;

        private bool _rendering;
        private IPartNavigator _navigator;

        //position in view specific coordinates		

        public PlayerViewD3D(IPageFiles pageFiles, IImageLoader imageLoader, IntPtr owner, PlayerMemento memento)
        {
            Trace.WriteLine("ctor of PlayerViewD3D");
            InitializeComponent();
            WindowState = FormWindowState.Maximized;

            _pageFiles = pageFiles;
            _imageLoader = imageLoader;
            zoomStrip.XZoom = memento.XZoom;
            zoomStrip.YZoom = memento.YZoom;
            speedStrip.Speed = memento.Speed;
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
                offset += l.Width + (float)lineDistance;
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
            speedStrip.Paused = false;
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
                Speed = speedStrip.Speed,
                XZoom = zoomStrip.XZoom,
                YZoom = zoomStrip.YZoom
            };
        }

        public void RegisterEvents(IPartNavigator navigator)
        {
            _navigator = navigator;
            partsStrip.RegisterEvents(navigator);
            speedStrip.RegisterEvents(navigator);
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
                zoomStrip.GetNextZoom(out _lastZoomX, out zoomY);
                _device.SetTransform(TransformState.World,
                    Matrix.Scaling(_lastZoomX, zoomY, 1F) *
                    Matrix.Translation((float)-XPos * _lastZoomX, 0, 0));
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

            if (!speedStrip.Paused)
            {
                if (!_navigator.IsPartituraFinished())
                {
                    if (!speedStrip.Paused)
                    {
                        XPos += speedStrip.Speed;
                        ViewHasScrolled?.Invoke();
                    }
                }
            }
            //}
        }

        private void PlayerViewD3D_KeyDown(object sender, KeyEventArgs e)
        {
            partsStrip.HandleKeyDown(sender, e);
            speedStrip.HandleKeyDown(sender, e);
            zoomStrip.HandleKeyDown(sender, e);
            switch (e.KeyCode)
            {
                case Keys.F4:
                    if (e.Modifiers == Keys.Alt)
                    {
                        SpecialClose();
                    }
                    break;
                case Keys.Escape:
                    SpecialClose();
                    break;
            }
        }

        /** behave as if window was closed, but it is still in memory */

        private void SpecialClose()
        {
            Hide();
            _closed = true;
            //Trace.WriteLine(SimpleTracer.GetData());
        }

        private void PlayerViewD3D_KeyUp(object sender, KeyEventArgs e)
        {
            speedStrip.HandleKeyUp(sender, e);
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
                    SpecialClose();
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