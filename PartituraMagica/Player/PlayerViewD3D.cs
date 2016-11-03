using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PartituraMagica.Player;
using SlimDX;
using SlimDX.Direct3D9;

namespace PartituraMagica
{
    /**
     * Uses Direct3D to implement a smooth scrolling view.
     * Contains custom controls for navigating, zooming, speed control
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

        private Device _device;
        private float _lastZoomX;
        private IPartNavigator _navigator;

        private bool _rendering;
        private const int PartAButtonIndex = 2;

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
                offset += l.Width + (float)lineDistance;
            }

            string[] parts = new string[partPattern.Length];
            for (int i = 0; i < partPattern.Length; i++)
            {
                parts[i] = partPattern[i].ToString();
            }
            CreatePartButtons(parts);

            XPos = 0;
        }

        private void CreatePartButtons(string[] partStrings)
        {
            int partIndex = partStrings.Length - 1;
            foreach (var partString in partStrings.Reverse())
            {
                var button = new ToolStripButton();
                button.DisplayStyle = ToolStripItemDisplayStyle.Text;
                button.Font = new System.Drawing.Font("Segoe UI", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
                button.ImageTransparentColor = Color.Magenta;
                button.Name = partString;
                button.Size = new Size(49, 51);
                button.Text = partString;
                toolStrip1.Items.Insert(PartAButtonIndex, button);

                int index = partIndex;
                button.Click += (sender, args) =>
                {
                    _navigator.PartClicked(index);
                };
                partIndex--;
            }
        }

        public void StartScrolling()
        {
            IWin32Window w = new Win32Window(_ownerHandle);
            _rendering = true;
            _speedManager.IsPaused = false;

            Enabled = true;
            Show(w);

            do
            {
                Application.DoEvents();
                Render();
            } while (_rendering);
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
            _navigator.CurrentPartIndexChanged += () =>
            {
                for (int i = 0; i < _navigator.PartCount; i++)
                {
                    var button = (ToolStripButton) toolStrip1.Items[i + PartAButtonIndex];
                    button.Checked = i == _navigator.CurrentPartIndex;
                }
            };
            _speedManager.IsPausedChanged += () =>
            {
                btnPauseResume.Checked = _speedManager.IsPaused;
                btnPauseResume.Image = _speedManager.IsPaused
                    ? Properties.Resources.play_button
                    : Properties.Resources.pause;
            };
            _keyboardMap.RegisterEvents(navigator, this);
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


        private class Win32Window : IWin32Window
        {
            public Win32Window(IntPtr w)
            {
                Handle = w;
            }

            public IntPtr Handle { get; }
        }

        private void btnFirstPart_Click(object sender, EventArgs e)
        {
            _navigator.FirstPartClicked();
        }

        private void btnPreviousPart_Click(object sender, EventArgs e)
        {
            _navigator.PreviousPartClicked();
        }

        private void btnNextPart_Click(object sender, EventArgs e)
        {
            _navigator.NextPartClicked();
        }

        private void btnLastPart_Click(object sender, EventArgs e)
        {
            _navigator.LastPartClicked();
        }

        private void btnJumpBackward_Click(object sender, EventArgs e)
        {
            _navigator.JumpBackwardClicked();
        }

        private void btnPauseResume_Click(object sender, EventArgs e)
        {
            _speedManager.TogglePaused();
        }

        private void btnJumpForward_Click(object sender, EventArgs e)
        {
            _navigator.JumpForwardClicked();
        }

        private void btnSlower_Click(object sender, EventArgs e)
        {
            _speedManager.DecreaseSpeed();
        }

        private void btnFaster_Click(object sender, EventArgs e)
        {
            _speedManager.IncreaseSpeed();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            _zoomManager.ZoomIn(false);
        }

        private void btnWider_Click(object sender, EventArgs e)
        {
            _zoomManager.ZoomIn(true);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            _zoomManager.ZoomOut(false);
        }

        private void btnSmaller_Click(object sender, EventArgs e)
        {
            _zoomManager.ZoomOut(true);
        }
    }
}