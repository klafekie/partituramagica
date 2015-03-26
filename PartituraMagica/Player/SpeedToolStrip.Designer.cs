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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpeedToolStrip));
            this.btnSlower = new System.Windows.Forms.ToolStripButton();
            this.btnPause = new System.Windows.Forms.ToolStripButton();
            this.btnFaster = new System.Windows.Forms.ToolStripButton();
            this.btnJumpBack = new System.Windows.Forms.ToolStripButton();
            this.btnJumpForward = new System.Windows.Forms.ToolStripButton();
            this.SuspendLayout();
            // 
            // btnSlower
            // 
            this.btnSlower.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSlower.Image = ((System.Drawing.Image)(resources.GetObject("btnSlower.Image")));
            this.btnSlower.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSlower.Name = "btnSlower";
            this.btnSlower.Size = new System.Drawing.Size(98, 28);
            this.btnSlower.Text = "Slower";
            // 
            // btnPause
            // 
            this.btnPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(84, 28);
            this.btnPause.Text = "Pause";
            // 
            // btnFaster
            // 
            this.btnFaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFaster.Image = ((System.Drawing.Image)(resources.GetObject("btnFaster.Image")));
            this.btnFaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFaster.Name = "btnFaster";
            this.btnFaster.Size = new System.Drawing.Size(98, 28);
            this.btnFaster.Text = "Faster";
            // 
            // btnJumpBack
            // 
            this.btnJumpBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnJumpBack.Image = ((System.Drawing.Image)(resources.GetObject("btnJumpBack.Image")));
            this.btnJumpBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnJumpBack.Name = "btnJumpBack";
            this.btnJumpBack.Size = new System.Drawing.Size(126, 28);
            this.btnJumpBack.Text = "Backward";
            // 
            // btnJumpForward
            // 
            this.btnJumpForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnJumpForward.Image = ((System.Drawing.Image)(resources.GetObject("btnJumpForward.Image")));
            this.btnJumpForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnJumpForward.Name = "btnJumpForward";
            this.btnJumpForward.Size = new System.Drawing.Size(23, 23);
            this.btnJumpForward.Text = "Forward";
            // 
            // SpeedToolStrip
            // 
            this.Font = new System.Drawing.Font("Lucida Console", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSlower,
            this.btnPause,
            this.btnFaster,
            this.btnJumpBack,
            this.btnJumpForward});
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HandleKeyUp);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripButton btnSlower;
		private System.Windows.Forms.ToolStripButton btnPause;
		private System.Windows.Forms.ToolStripButton btnFaster;
        private System.Windows.Forms.ToolStripButton btnJumpBack;
        private System.Windows.Forms.ToolStripButton btnJumpForward;
    }
}
