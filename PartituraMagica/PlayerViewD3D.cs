using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		private const float LINE_DISTANCE = 10F;
		private readonly IPageFiles context;
		private readonly IImageLoader imageLoader;
		private readonly IntPtr ownerHandle;

		private Device device;

		private bool rendering;
		private bool closed = true;

		//position in view specific coordinates		
		private double viewXPos;
		private float lastZoomX;
		private int currentLine;
		private readonly List<ClippingD3D> lines = new List<ClippingD3D>();
		private readonly Direct3D d3d;

		public PlayerViewD3D(IPageFiles context, IImageLoader imageLoader, IntPtr owner)
		{
			Trace.WriteLine("ctor of PlayerViewD3D");
			InitializeComponent();
			ClientSize = new Size(640, 480);
			WindowState = FormWindowState.Maximized;

			this.context = context;
			this.imageLoader = imageLoader;
			zoomStrip.XZoom = 0.004F;
			zoomStrip.YZoom = 0.01F;
			ownerHandle = owner;

			d3d = new Direct3D();
			CreateDevice();
		}

		protected override void Dispose(bool disposing)
		{
			rendering = false;
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);

			if (device != null)
			{
				device.Dispose();
			}
			foreach (ClippingD3D l in lines)
			{
				l.Dispose();
			}
			ClippingD3D.Cleanup();
			if (d3d != null)
			{
				d3d.Dispose();
			}
		}

		private void CreateDevice()
		{
			//using (SimpleTracer.Start("OnShown()")) {
			Capabilities caps;
			int adapter = d3d.Adapters.DefaultAdapter.Adapter;
			try
			{
				caps = d3d.GetDeviceCaps(adapter, DeviceType.Hardware);
			}
			catch (Exception ex)
			{
				Trace.TraceWarning("Ignoring Exception when getting dev caps for hardware acceleration:");
				Trace.TraceWarning(ex.ToString());
				caps = d3d.GetDeviceCaps(adapter, DeviceType.Reference);
			}

			//CreateFlags createFlags = caps.DeviceCaps.SupportsHardwareTransformAndLight ?
			//  CreateFlags.HardwareVertexProcessing :
			//  CreateFlags.MixedVertexProcessing;

			device = new Device(d3d, adapter, DeviceType.Hardware, panel1.Handle, CreateFlags.HardwareVertexProcessing,
				new PresentParameters
				{
					PresentationInterval = PresentInterval.One,
					Multisample = MultisampleType.None,
					Windowed = true,
					SwapEffect = SwapEffect.Discard
				});
			//device.DeviceResizing += new System.ComponentModel.CancelEventHandler(device_DeviceResizing);
			device.SetRenderState(RenderState.CullMode, Cull.None);
			device.SetRenderState(RenderState.Lighting, false);
			device.SetRenderState(RenderState.Clipping, false);
			device.SetRenderState(RenderState.AlphaBlendEnable, false);
			device.SetRenderState(RenderState.MultisampleAntialias, false);

			//}
		}

		public void Init(string partPattern, IEnumerable<Clipping> clips)
		{
			lines.Clear();
			foreach (Clipping c in clips)
			{
				string path = context.GetImage(c.PageIndex);
				IGreyImage image = imageLoader.GetImage(path);
				ClippingD3D line = new ClippingD3D(image, c.Coords);
				lines.Add(line);
			}

			float offset = 0F;
			foreach (ClippingD3D l in lines)
			{
				l.Init(device, offset);
				offset += l.Width + LINE_DISTANCE;
			}

			string[] choices = new string[partPattern.Length];
			for (int i = 0; i < partPattern.Length; i++)
			{
				choices[i] = partPattern[i].ToString();
			}
			partsStrip.Choices = choices;
			partsStrip.Current = 0;
			viewXPos = 0;
			currentLine = 0;
		}

		private void device_DeviceResizing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
		}

		private class Win32Window : IWin32Window
		{
			private readonly IntPtr handle;

			public Win32Window(IntPtr w)
			{
				handle = w;
			}

			public IntPtr Handle { get { return handle; } }
		}

		private void Render()
		{
			//using (TraceEntry e = SimpleTracer.Start("Render()")) {
			//	int tick = Environment.TickCount;
			if (device == null)
			{
				return;
			}

			if (!rendering)
			{
				//Debug.Write(".");
				return;
			}

			device.Clear(ClearFlags.Target, Color.Orange, 1.0f, 0);
			try
			{
				device.BeginScene();
				float zoomY;
				zoomStrip.GetNextZoom(out lastZoomX, out zoomY);
				device.SetTransform(TransformState.World,
					Matrix.Scaling(lastZoomX, zoomY, 1F) *
					Matrix.Translation((float) -viewXPos * lastZoomX, 0, 0));
				for (int i = 0; i < lines.Count; i++)
				{
					lines[i].Render(device);
				}
			}
			finally
			{
				device.EndScene();
			}

			device.Present();
			if (!speedStrip.Paused)
			{
				if (viewXPos < lines.Last().Right)
				{
					viewXPos += speedStrip.Speed;
					if (viewXPos > lines[currentLine].Right)
					{
						if (!OnLineEndReached())
						{
							currentLine++;
						}
					}
				}
			}
			//}
		}

		public void GotoLine(int lineIndex)
		{
			this.currentLine = lineIndex;
			this.viewXPos = this.lines[lineIndex].XPos;
		}

		public void StartScrolling()
		{
			IWin32Window w = new Win32Window(ownerHandle);
			rendering = true;
			speedStrip.Paused = false;
			closed = false;
			Enabled = true;
			Show(w);

			do
			{
				Application.DoEvents();
				Render();
			} while (!closed);
			Enabled = false;
		}

		public double XPos
		{
			get
			{
				Debug.Assert(lastZoomX > 0F);
				return (viewXPos - (double) currentLine * LINE_DISTANCE);
			}
		}

		private void PlayerViewDX_Resize(object sender, EventArgs e)
		{
			rendering = (WindowState != FormWindowState.Minimized &&
									 Visible);
			//Debug.WriteLine("Resize(): Rendering=" + this.rendering);
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			Render();
		}

		private void PlayerViewDX_Activated(object sender, EventArgs e)
		{
			rendering = true;
		}

		private void PlayerViewDX_Deactivate(object sender, EventArgs e)
		{
			rendering = false;
			//Debug.WriteLine("Deactivate() Rendering = false");
		}

		public event LineEndEvent LineEndReached;

		protected bool OnLineEndReached()
		{
			bool cancel = false;
			if (LineEndReached != null)
			{
				LineEndReached(currentLine, ref cancel);
			}
			return cancel;
		}

		public IChoiceNavigator PartNavigator { get { return partsStrip; } }

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
			closed = true;
			//Trace.WriteLine(SimpleTracer.GetData());
		}

		private void PlayerViewD3D_KeyUp(object sender, KeyEventArgs e)
		{
			speedStrip.HandleKeyUp(sender, e);
		}

		private void PlayerViewD3D_FormClosing(object sender, FormClosingEventArgs e)
		{
			closed = true;
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
	}
}