using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectDraw;
using System.Diagnostics;
using System.Threading;

namespace PartituraMagica
{
	public partial class PlayerViewDX : Form, IDisposable
	{
		private Surface charsurface;
		private Device device;
		private int lastX;
		private int lastY;
        private Surface psurface;
		private bool restore = false;
        private Surface ssurface;
		private int x = 0;
		private int y = 0;

		public PlayerViewDX() {
			InitializeComponent();
			Init();
		}

		public void Cleanup(bool cleanDevice) {
			if (cleanDevice) {
				this.device.Dispose();
			}
			this.psurface.Dispose();
			this.ssurface.Dispose();
			this.charsurface.Dispose();
		}


		private void Init() {
			this.device = new Device();
			this.device.SetCooperativeLevel(this, CooperativeLevelFlags.FullscreenExclusive);
			//this.device.SetDisplayMode(1280, 800, 8, 60, false);			
			
			SurfaceDescription sdesc = new SurfaceDescription();
			Clipper clip = new Clipper(device);
			clip.Window = this;
			sdesc.SurfaceCaps.PrimarySurface = true;
			sdesc.SurfaceCaps.Flip = true;
			sdesc.SurfaceCaps.Complex = true;
			sdesc.BackBufferCount = 1;
			psurface = new Surface(sdesc, device);
			psurface.Clipper = clip;
			sdesc.Clear();
			sdesc.SurfaceCaps.BackBuffer = true;
			ssurface = psurface.GetAttachedSurface(sdesc.SurfaceCaps);
			sdesc.Clear();
			sdesc.SurfaceCaps.OffScreenPlain = true;
			Bitmap b = new Bitmap(@"C:\Dokumente und Einstellungen\2-22-a\Eigene Dateien\SharpDevelop\src\Libraries\DockPanel_Src\WinFormsUI\Docking\Resources\DockIndicator_PaneDiamond_Bottom.bmp");
			//Bitmap b = new Bitmap(@"C:\Dokumente und Einstellungen\2-22-a\Eigene Dateien\PartituraMagica\Examples\aussente-004.bmp");
			charsurface = new Surface(b, sdesc, device);
		}

		private void PlayerViewDX_FormClosed(object sender, FormClosedEventArgs e) {
			this.Cleanup(true);
		}

		//public void ShowSurface() {

		//    try {
		//        ssurface.ColorFill(Color.White);
		//        ssurface.ForeColor = Color.Black;
		//        //ssurface.DrawText(540, 565, "Made By Imran Ahmed Khan", false);
		//        psurface.Flip(ssurface, FlipFlags.Wait);
		//    } catch (SurfaceLostException) {
		//        restore = true;
		//    }
		//}

		public void MoveSurface(int x, int y) {

			try {	
				int deltaX = x - this.lastX;
				int deltaY = y - this.lastY;

				if (deltaX > 0) {
					Rectangle r1 = new Rectangle(this.lastX, this.lastY, deltaX, 100);
					ssurface.ColorFill(r1, Color.White);
				} else if (deltaX < 0) {
					Rectangle r1 = new Rectangle(x + 100, y + 100, -deltaX, 100);
					ssurface.ColorFill(r1, Color.White);
				}
				if (deltaY > 0) {
					Rectangle r1 = new Rectangle(this.lastX, this.lastY, 100, deltaY);
					ssurface.ColorFill(r1, Color.White);
				} else if (deltaY < 0) {
					Rectangle r1 = new Rectangle(x + 100, y + 100, 100, -deltaY);
					ssurface.ColorFill(r1, Color.White);
				}
				//ssurface.ColorFill(Color.White);
				
				this.lastX = x;
				this.lastY = y;
				

				ssurface.Draw(new Rectangle(x, y, 100,100), charsurface, DrawFlags.Wait);
				//ssurface.DrawText(540, 565, "Made By Imran Ahmed Khan", false);
				psurface.Flip(ssurface, FlipFlags.Wait);
				
			} catch (SurfaceLostException) {
				restore = true;				
			}			
		}

		private void PlayerViewDX_Load(object sender, EventArgs e) {
			//ShowSurface();
			ssurface.ColorFill(Color.White);		        
			while (true) {
				for (int i = 0; i < 400; i++) {						
					MoveSurface(i, i);
					Application.DoEvents();
					//Thread.Sleep(5);
				}
				for (int i = 399; i >= 0; i--) {
					MoveSurface(i, i);
					Application.DoEvents();
					//Thread.Sleep(5);
				}				
			}
			//this.timer1.Start();
		}

		private void PlayerViewDX_Activated(object sender, EventArgs e) {
			//TryRestore();
			//ShowSurface();
		}

		private void TryRestore() {
			if (this.restore) {
				try {
					device.RestoreAllSurfaces();
					this.restore = false;
				} catch (WrongModeException ex) {
					Debug.WriteLine(ex);
				}
			}
		}

		private void timer1_Tick(object sender, EventArgs e) {
			//TryRestore();
			//ShowSurface(this.x++, this.y++);
		}

	}
}
