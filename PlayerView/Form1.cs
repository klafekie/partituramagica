using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectDraw;

namespace PlayerView
{
	public partial class Form1 : Form
	{
		Device device = null;
		Surface primeSurface;
		Surface secondSurface;
		Clipper clip = null;

		public Form1() {
			InitializeComponent();			
			
			device = new Device(); // Creating Device Object
			device.SetCooperativeLevel(this, CooperativeLevelFlags.FullscreenExclusive);
			this.Load += new EventHandler(Form1_Load);
			InitInterfaces();

			GameLoop();
		}

		void Form1_GotFocus(object sender, EventArgs e) {
			ShowSurface();
		}

		private void InitInterfaces() {
			SurfaceDescription sdesc = new SurfaceDescription();
			Clipper clip = new Clipper(device);
			clip.Window = this;
			sdesc.SurfaceCaps.PrimarySurface = true;
			sdesc.SurfaceCaps.Flip = true;
			sdesc.SurfaceCaps.Complex = true;
			sdesc.BackBufferCount = 1;
			primeSurface = new Surface(sdesc, device);
			primeSurface.Clipper = clip;
			sdesc.Clear();
			sdesc.SurfaceCaps.BackBuffer = true;
			secondSurface = primeSurface.GetAttachedSurface(sdesc.SurfaceCaps);
		}

		private void GameLoop() {
			do {
				if (!this.Focused)
					Application.DoEvents();
				else
					ShowSurface();
				Application.DoEvents();
			} while (this.Created);
			Cleanup();
		}

		private void ShowSurface() {
			try {
				secondSurface.ColorFill(Color.White); //fill the backbuffer ( secondary Surface ) with a color
				primeSurface.Flip(secondSurface, FlipFlags.Wait); //flip the backbuffer to the front
			} catch (GraphicsException) {
				primeSurface.Dispose();
				secondSurface.Dispose();
				InitInterfaces();
				GameLoop();
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e) {
			Cleanup(); 
			base.OnFormClosed(e);		
		}

		private void Cleanup() {
			device.Dispose();
			primeSurface.Dispose();
			secondSurface.Dispose();
		}

		private void timer1_Tick(object sender, EventArgs e) {
			//this.flowLayoutPanel1.HorizontalScroll.Value += 5;
			this.Invalidate(true);
		}
				
		private void Form1_Load(object sender, EventArgs e) {
			Cleanup();
			Form1 newform = new Form1();
			newform.Show();
		}
				
		private void Form1_Paint(object sender, PaintEventArgs e) {
			//Debug.WriteLine(DateTime.Now);
			//Graphics g = e.Graphics;
			//g.TranslateTransform(4, 0);
			//g.DrawImage(this.pictureBox1.Image, x++, 0);
		}

		private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {
			//Form1_Paint(sender, e);
		}
	}
}
