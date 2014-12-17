using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace PartituraMagica
{
	//@todo p1 allow to scroll backwards
	/**
	 * A custom control that allows to increase/decrease speed 
	 * and to pause/resume playing
	 */
	public partial class SpeedToolStrip : ToolStrip
	{
		private bool paused;
		public bool Paused {
			get { return paused; }
			set {
				btnPause.Text = value ? "Resume" : "Pause";
				btnPause.Image = this.imageList1.Images[value ? "resume" : "pause"];
				btnPause.Checked = value;
				paused = value;
//				OnSpeedChanged(this.Speed);
			}
		}

		private float speed = 1F;
		public float Speed {
			get { return this.paused ? 0F : this.speed;  }
			set {
				if (value == 0) {
					//avoid that speed ever becomes 0, cause we use
					//multiplication to increase/decrease it
					this.paused = true;
				} else {					
					//if (this.speed != value) {
					//    OnSpeedChanged(this.Speed);//using getter here!
					//}
					this.speed = value;
				}
			}
		}

		public SpeedToolStrip() {
			InitializeComponent();
			btnSlower.Click += new EventHandler(btnSlower_Click);
			btnSlower.Image = this.imageList1.Images["decreaseSpeed"];			
			btnPause.Click += new EventHandler(btnPause_Click);			
			btnFaster.Click += new EventHandler(btnFaster_Click);
			btnFaster.Image = this.imageList1.Images["increaseSpeed"];

			foreach (ToolStripItem i in this.Items) {
				ToolStripButton b = i as ToolStripButton;
				if (b != null) {
					b.ImageScaling = ToolStripItemImageScaling.SizeToFit;
					b.AutoSize = false;
					b.Size = new Size(36, 36);
				}
			}
			this.Paused = true;
		}

		void btnFaster_Click(object sender, EventArgs e) {
			this.speed *= 1.2F;
		}

		void btnPause_Click(object sender, EventArgs e) {
			this.Paused = !this.Paused;			
		}

		void btnSlower_Click(object sender, EventArgs e) {
			this.speed *= 0.8F;
		}

		public void HandleKeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Oemplus:
				case Keys.Add:
					if (e.Modifiers == Keys.None) {
						this.speed *= 1.05F;
					} 
					break;
				case Keys.OemMinus:
				case Keys.Subtract:
					if (e.Modifiers == Keys.None) {
						this.speed *= 0.95F; 
					}
					break;
				case Keys.Space:
					this.Paused = true;
					break;
			}

		}

		public void HandleKeyUp(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Space:
					this.Paused = false;//use property setter
					break;
			}
		}

		//public event Action<float> SpeedChanged;
		//protected void OnSpeedChanged(float value) {
		//    if (this.SpeedChanged != null) {
		//        this.SpeedChanged(value);
		//    }
		//}
	}
}
