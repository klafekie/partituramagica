namespace PartituraMagica
{
	partial class SpeedToolStrip
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
		    if (disposing && (components != null)) {
		        components.Dispose();
		    }
		    base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpeedToolStrip));
			this.btnSlower = new System.Windows.Forms.ToolStripButton();
			this.btnPause = new System.Windows.Forms.ToolStripButton();
			this.btnFaster = new System.Windows.Forms.ToolStripButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// btnSlower
			// 
			this.btnSlower.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnSlower.Image = ((System.Drawing.Image)(resources.GetObject("btnSlower.Image")));
			this.btnSlower.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSlower.Name = "btnSlower";
			this.btnSlower.Size = new System.Drawing.Size(23, 22);
			this.btnSlower.Text = "Slow Down";
			// 
			// btnPause
			// 
			this.btnPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
			this.btnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnPause.Name = "btnPause";
			this.btnPause.Size = new System.Drawing.Size(23, 22);
			this.btnPause.Text = "Pause";
			// 
			// btnFaster
			// 
			this.btnFaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFaster.Image = ((System.Drawing.Image)(resources.GetObject("btnFaster.Image")));
			this.btnFaster.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFaster.Name = "btnFaster";
			this.btnFaster.Size = new System.Drawing.Size(23, 22);
			this.btnFaster.Text = "Go Faster";
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "resume");
			this.imageList1.Images.SetKeyName(1, "pause");
			this.imageList1.Images.SetKeyName(2, "increaseSpeed");
			this.imageList1.Images.SetKeyName(3, "decreaseSpeed");
			// 
			// SpeedToolStrip
			// 
			this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSlower,
            this.btnPause,
            this.btnFaster});
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleKeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HandleKeyUp);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripButton btnSlower;
		private System.Windows.Forms.ToolStripButton btnPause;
		private System.Windows.Forms.ToolStripButton btnFaster;
		private System.Windows.Forms.ImageList imageList1;
	}
}
