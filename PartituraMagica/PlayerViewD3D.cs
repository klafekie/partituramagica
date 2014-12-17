using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PartituraMagica.Playing;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Diagnostics;

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
		private const float LINE_DISTANCE = 10F;
		private IPageFiles context;
		private IImageLoader imageLoader;
        private IntPtr ownerHandle;

		private Device device = null;

		private bool rendering = false;
		private bool closed = true;

		//position in view specific coordinates		
		private double viewXPos;
		private float lastZoomX; 
		private int currentLine;
		private List<ClippingD3D> lines = new List<ClippingD3D>();

		public PlayerViewD3D(IPageFiles context, IImageLoader imageLoader, IntPtr owner) {
			Trace.WriteLine("ctor of PlayerViewD3D");
			InitializeComponent();
			this.ClientSize = new System.Drawing.Size(640, 480);
			this.WindowState = FormWindowState.Maximized;
			
			this.context = context;
			this.imageLoader = imageLoader;
			this.zoomStrip.XZoom = 0.004F;
			this.zoomStrip.YZoom = 0.01F;
			this.ownerHandle = owner;

			CreateDevice();
		}

		protected override void Dispose(bool disposing) {
			this.rendering = false;
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		
			if (this.device != null) {
				this.device.Dispose();
			}
			foreach (ClippingD3D l in this.lines) {
				l.Dispose();
			}
			ClippingD3D.Cleanup();
		}

		private void CreateDevice() {
			//using (SimpleTracer.Start("OnShown()")) {
				Caps caps;
				int adapter = Manager.Adapters.Default.Adapter;
				try {
					caps = Manager.GetDeviceCaps(adapter, DeviceType.Hardware);
				} catch (Exception ex) {
					Trace.TraceWarning("Ignoring Exception when getting dev caps for hardware acceleration:");
					Trace.TraceWarning(ex.ToString());
					caps = Manager.GetDeviceCaps(adapter, DeviceType.Reference);
				}


				CreateFlags createFlags = caps.DeviceCaps.SupportsHardwareTransformAndLight ?
					CreateFlags.HardwareVertexProcessing :
					CreateFlags.MixedVertexProcessing;				

				PresentParameters presentParams1 = new PresentParameters();
				presentParams1.PresentationInterval = PresentInterval.One;
				presentParams1.MultiSample = MultiSampleType.None;
				presentParams1.Windowed = true;
				presentParams1.SwapEffect = SwapEffect.Discard;

				device = new Device(adapter, caps.DeviceType, this.panel1, createFlags, presentParams1);
				device.DeviceResizing += new System.ComponentModel.CancelEventHandler(device_DeviceResizing);
				device.RenderState.CullMode = Cull.None;
				device.RenderState.Lighting = false;
				device.RenderState.Clipping = false;
				device.RenderState.AlphaBlendEnable = false;
				device.RenderState.MultiSampleAntiAlias = false;         
			//}
		}

		public void Init(string partPattern, IEnumerable<Clipping> clips) {			
			this.lines.Clear();
            foreach (Clipping c in clips)
            {
                string path = this.context.GetImage(c.PageIndex);
                IGreyImage image = imageLoader.GetImage(path);
                ClippingD3D line = new ClippingD3D(image, c.Coords);
                this.lines.Add(line);
            }

            float offset = 0F;
            foreach (ClippingD3D l in this.lines)
            {
                l.Init(device, offset);
                offset += l.Width + LINE_DISTANCE;
            }

            string[] choices = new string[partPattern.Length];
            for (int i = 0; i < partPattern.Length; i++)
            {
                choices[i] = partPattern[i].ToString();
            }
            this.partsStrip.Choices = choices;
            this.partsStrip.Current = 0;
            this.viewXPos = 0;
			this.currentLine = 0;
        }
		
		void device_DeviceResizing(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = true;
		}

		private class Win32Window : IWin32Window
		{
			IntPtr handle;
			public Win32Window(IntPtr w) {
				this.handle = w;
			}

			public IntPtr Handle {
				get { return handle; }
			}
		}

		private void Render() {
			//using (TraceEntry e = SimpleTracer.Start("Render()")) {
				//	int tick = Environment.TickCount;
				if (device == null)
					return;

				if (!this.rendering) {
					//Debug.Write(".");
					return;
				}

				device.Clear(ClearFlags.Target, System.Drawing.Color.Black, 1.0f, 0);
				float zoomY;
				try {
					device.BeginScene();					
					this.zoomStrip.GetNextZoom(out lastZoomX, out zoomY);
					device.Transform.World =
						Matrix.Scaling(lastZoomX, zoomY, 1F) *
						Matrix.Translation((float)-viewXPos * lastZoomX, 0, 0);
					for (int i = 0; i < lines.Count; i++) {
						lines[i].Render(device);
					}
				} finally {
					device.EndScene();
				}

				device.Present();
				if (!this.speedStrip.Paused) {
					if (viewXPos < this.lines.Last().Right) {
						viewXPos += this.speedStrip.Speed;						
						if (viewXPos > this.lines[this.currentLine].Right) {
							if (!OnLineEndReached()) {
								this.currentLine++;
							}							
						}
					}
				}
			//}
		}		
				
		public void GotoLine(int lineIndex) {
		    this.currentLine = lineIndex;
			this.viewXPos = this.lines[lineIndex].XPos;
		}

		public void StartScrolling() {
			System.Windows.Forms.IWin32Window w = new Win32Window(this.ownerHandle);			
			this.rendering = true;
			this.speedStrip.Paused = false;
            this.closed = false;
			this.Enabled = true;
			this.Show(w);      
            
			do {
                Application.DoEvents();
				Render();
			} while (!this.closed);
			this.Enabled = false;
		}			

		public double XPos {
			get {
				Debug.Assert(this.lastZoomX > 0F);
				return (this.viewXPos - (double)this.currentLine * LINE_DISTANCE); 
			}
		}
				
		
		private void PlayerViewDX_Resize(object sender, EventArgs e) {
			this.rendering = (this.WindowState != FormWindowState.Minimized &&
				this.Visible);
			//Debug.WriteLine("Resize(): Rendering=" + this.rendering);
		}

		private void panel1_Paint(object sender, PaintEventArgs e) {
			this.Render();
		}

		private void PlayerViewDX_Activated(object sender, EventArgs e) {
			this.rendering = true;
		}

		private void PlayerViewDX_Deactivate(object sender, EventArgs e) {
			this.rendering = false;
			//Debug.WriteLine("Deactivate() Rendering = false");
		}

		public event LineEndEvent LineEndReached;
		protected bool OnLineEndReached() {
			bool cancel = false;
			if (LineEndReached != null) {
				LineEndReached(this.currentLine, ref cancel);
			}
			return cancel;
		}
		
		public IChoiceNavigator PartNavigator {
			get { return this.partsStrip; }
		}		

		private void PlayerViewD3D_KeyDown(object sender, KeyEventArgs e) {
			this.partsStrip.HandleKeyDown(sender, e);
			this.speedStrip.HandleKeyDown(sender, e);
			this.zoomStrip.HandleKeyDown(sender, e);
			switch (e.KeyCode) { 
				case Keys.F4:
					if (e.Modifiers == Keys.Alt) {
						SpecialClose(); 
					}
					break;
				case Keys.Escape:
					SpecialClose(); break;
			}
		}

		/** behave as if window was closed, but it is still in memory */
		private void SpecialClose() {
			this.Hide();
			this.closed = true;
			//Trace.WriteLine(SimpleTracer.GetData());
		}

		private void PlayerViewD3D_KeyUp(object sender, KeyEventArgs e) {
			this.speedStrip.HandleKeyUp(sender, e);
		}

		private void PlayerViewD3D_FormClosing(object sender, FormClosingEventArgs e) {
			this.closed = true;
			switch (e.CloseReason) {
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

		
	}
}
