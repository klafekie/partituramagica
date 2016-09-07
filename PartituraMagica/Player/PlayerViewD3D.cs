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
        private const float LineDistance = 10F;

        private readonly List<ClippingD3D> _clippingD3Ds = new List<ClippingD3D>();
            // [Renamed from 'lines' jed 20150118]

        private readonly ApplicationConfiguration _configuration = new ApplicationConfiguration();
        private readonly Direct3D _d3D;
        private readonly IImageLoader _imageLoader;
        private readonly IntPtr _ownerHandle;
        private readonly IPageFiles _pageFiles;
        private bool _closed = true;
        private int _currentLine;

        private Device _device;
        private float _lastZoomX;

        private bool _rendering;

        //position in view specific coordinates		
        private double _viewXPos;

        public PlayerViewD3D(IPageFiles pageFiles, IImageLoader imageLoader, IntPtr owner, PlayerMemento memento)
        {
            Trace.WriteLine("ctor of PlayerViewD3D");
            InitializeComponent();
            WindowState = FormWindowState.Maximized;

            this._pageFiles = pageFiles;
            this._imageLoader = imageLoader;
            zoomStrip.XZoom = memento.XZoom;
            zoomStrip.YZoom = memento.YZoom;
            speedStrip.Speed = memento.Speed;
            _ownerHandle = owner;

            _d3D = new Direct3D();
            CreateDevice();
        }

        public void Init(string partPattern, IEnumerable<Clipping> clips)
        {
            _clippingD3Ds.Clear();
            foreach (Clipping c in clips)
            {
                string path = _pageFiles.GetImage(c.PageIndex);
                IGreyImage image = _imageLoader.GetImage(path);
                ClippingD3D line = new ClippingD3D(image, c.Coords, _configuration.InvertColors, _configuration.Color);
                _clippingD3Ds.Add(line);
            }

            float offset = 0F;
            foreach (ClippingD3D l in _clippingD3Ds)
            {
                l.Init(_device, offset);
                offset += l.Width + LineDistance;
            }

            string[] parts = new string[partPattern.Length];
            for (int i = 0; i < partPattern.Length; i++)
            {
                parts[i] = partPattern[i].ToString();
            }
            partsStrip.Parts = parts;
            partsStrip.Current = 0;
            _viewXPos = 0;
            _currentLine = 0;
        }

        public void GotoLine(int lineIndex)
        {
            _currentLine = lineIndex;
            _viewXPos = _clippingD3Ds[lineIndex].XPos;
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

        public double XPos
        {
            get
            {
                Debug.Assert(_lastZoomX > 0F);
                return _viewXPos - (double) _currentLine*LineDistance;
            }
        }

        public event LineEndEvent LineEndReached;

        public PlayerMemento ToMemento()
        {
            return new PlayerMemento
            {
                Speed = speedStrip.Speed,
                XZoom = zoomStrip.XZoom,
                YZoom = zoomStrip.YZoom
            };
        }

        public IPartNavigator PartNavigator
        {
            get { return partsStrip; }
        }

        protected override void Dispose(bool disposing)
        {
            _rendering = false;
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            if (_device != null)
            {
                _device.Dispose();
            }
            foreach (ClippingD3D l in _clippingD3Ds)
            {
                l.Dispose();
            }
            ClippingD3D.Cleanup();
            if (_d3D != null)
            {
                _d3D.Dispose();
            }
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
                    Matrix.Scaling(_lastZoomX, zoomY, 1F)*
                    Matrix.Translation((float) -_viewXPos*_lastZoomX, 0, 0));
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

            if (speedStrip.JumpsCount != 0)
            {
                _viewXPos = Math.Max(0, _viewXPos - speedStrip.JumpsCount*250);
                speedStrip.JumpsCount = 0;
            }

            if (!speedStrip.Paused)
            {
                if (_viewXPos < _clippingD3Ds.Last().Right)
                {
                    if (!speedStrip.Paused)
                    {
                        _viewXPos += speedStrip.Speed;
                    }
                    if (_viewXPos > _clippingD3Ds[_currentLine].Right)
                    {
                        if (!OnLineEndReached())
                        {
                            _currentLine++;
                        }
                    }
                }
            }
            //}
        }

        protected bool OnLineEndReached()
        {
            bool cancel = false;
            LineEndReached?.Invoke(_currentLine, ref cancel);
            return cancel;
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