namespace PartituraMagica.Player
{
    partial class PlayerViewD3D
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnFirstPart = new System.Windows.Forms.ToolStripButton();
            this.btnPreviousPart = new System.Windows.Forms.ToolStripButton();
            this.btnNextPart = new System.Windows.Forms.ToolStripButton();
            this.btnLastPart = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnJumpBackward = new System.Windows.Forms.ToolStripButton();
            this.btnPauseResume = new System.Windows.Forms.ToolStripButton();
            this.btnJumpForward = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSlower = new System.Windows.Forms.ToolStripButton();
            this.btnFaster = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnWider = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnSmaller = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1006, 227);
            this.panel1.TabIndex = 1;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1006, 227);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1006, 266);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyDown);
            this.toolStripContainer1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFirstPart,
            this.btnPreviousPart,
            this.btnNextPart,
            this.btnLastPart,
            this.toolStripSeparator1,
            this.btnJumpBackward,
            this.btnPauseResume,
            this.btnJumpForward,
            this.toolStripSeparator3,
            this.btnSlower,
            this.btnFaster,
            this.toolStripSeparator2,
            this.btnZoomIn,
            this.btnWider,
            this.btnZoomOut,
            this.btnSmaller});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(579, 39);
            this.toolStrip1.TabIndex = 0;
            // 
            // btnFirstPart
            // 
            this.btnFirstPart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFirstPart.Image = global::PartituraMagica.Properties.Resources.back3;
            this.btnFirstPart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnFirstPart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFirstPart.Name = "btnFirstPart";
            this.btnFirstPart.Size = new System.Drawing.Size(36, 36);
            this.btnFirstPart.Text = "Show First Part";
            this.btnFirstPart.Click += new System.EventHandler(this.btnFirstPart_Click);
            // 
            // btnPreviousPart
            // 
            this.btnPreviousPart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPreviousPart.Image = global::PartituraMagica.Properties.Resources.previous1;
            this.btnPreviousPart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPreviousPart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreviousPart.Name = "btnPreviousPart";
            this.btnPreviousPart.Size = new System.Drawing.Size(36, 36);
            this.btnPreviousPart.Text = "Show Previous Part";
            this.btnPreviousPart.Click += new System.EventHandler(this.btnPreviousPart_Click);
            // 
            // btnNextPart
            // 
            this.btnNextPart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNextPart.Image = global::PartituraMagica.Properties.Resources.next1;
            this.btnNextPart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnNextPart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextPart.Name = "btnNextPart";
            this.btnNextPart.Size = new System.Drawing.Size(36, 36);
            this.btnNextPart.Text = "Show Next Part";
            this.btnNextPart.Click += new System.EventHandler(this.btnNextPart_Click);
            // 
            // btnLastPart
            // 
            this.btnLastPart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLastPart.Image = global::PartituraMagica.Properties.Resources.next_11;
            this.btnLastPart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnLastPart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLastPart.Name = "btnLastPart";
            this.btnLastPart.Size = new System.Drawing.Size(36, 36);
            this.btnLastPart.Text = "Show Last Part";
            this.btnLastPart.Click += new System.EventHandler(this.btnLastPart_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // btnJumpBackward
            // 
            this.btnJumpBackward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnJumpBackward.Image = global::PartituraMagica.Properties.Resources.rewind;
            this.btnJumpBackward.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnJumpBackward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnJumpBackward.Name = "btnJumpBackward";
            this.btnJumpBackward.Size = new System.Drawing.Size(36, 36);
            this.btnJumpBackward.Text = "Jump Back";
            this.btnJumpBackward.Click += new System.EventHandler(this.btnJumpBackward_Click);
            // 
            // btnPauseResume
            // 
            this.btnPauseResume.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPauseResume.Image = global::PartituraMagica.Properties.Resources.play_button;
            this.btnPauseResume.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPauseResume.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPauseResume.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.btnPauseResume.Name = "btnPauseResume";
            this.btnPauseResume.Size = new System.Drawing.Size(36, 36);
            this.btnPauseResume.Text = "Resume";
            this.btnPauseResume.Click += new System.EventHandler(this.btnPauseResume_Click);
            // 
            // btnJumpForward
            // 
            this.btnJumpForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnJumpForward.Image = global::PartituraMagica.Properties.Resources.fast_forward;
            this.btnJumpForward.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnJumpForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnJumpForward.Name = "btnJumpForward";
            this.btnJumpForward.Size = new System.Drawing.Size(36, 36);
            this.btnJumpForward.Text = "Jump Forward";
            this.btnJumpForward.Click += new System.EventHandler(this.btnJumpForward_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // btnSlower
            // 
            this.btnSlower.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSlower.Image = global::PartituraMagica.Properties.Resources.minus;
            this.btnSlower.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSlower.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSlower.Name = "btnSlower";
            this.btnSlower.Size = new System.Drawing.Size(36, 36);
            this.btnSlower.Text = "Slower";
            this.btnSlower.Click += new System.EventHandler(this.btnSlower_Click);
            // 
            // btnFaster
            // 
            this.btnFaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFaster.Image = global::PartituraMagica.Properties.Resources.plus;
            this.btnFaster.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnFaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFaster.Name = "btnFaster";
            this.btnFaster.Size = new System.Drawing.Size(36, 36);
            this.btnFaster.Text = "Faster";
            this.btnFaster.Click += new System.EventHandler(this.btnFaster_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.Image = global::PartituraMagica.Properties.Resources.zoom_in;
            this.btnZoomIn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(36, 36);
            this.btnZoomIn.Text = "Zoom In";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnWider
            // 
            this.btnWider.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnWider.Image = global::PartituraMagica.Properties.Resources.resize;
            this.btnWider.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnWider.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnWider.Name = "btnWider";
            this.btnWider.Size = new System.Drawing.Size(36, 36);
            this.btnWider.Text = "Widen";
            this.btnWider.Click += new System.EventHandler(this.btnWider_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.Image = global::PartituraMagica.Properties.Resources.zoom_out;
            this.btnZoomOut.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(36, 36);
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnSmaller
            // 
            this.btnSmaller.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSmaller.Image = global::PartituraMagica.Properties.Resources.narrow;
            this.btnSmaller.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSmaller.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSmaller.Name = "btnSmaller";
            this.btnSmaller.Size = new System.Drawing.Size(36, 36);
            this.btnSmaller.Text = "Narrow";
            this.btnSmaller.Click += new System.EventHandler(this.btnSmaller_Click);
            // 
            // PlayerViewD3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 266);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "PlayerViewD3D";
            this.Text = "PlayerView";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyUp);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnFirstPart;
        private System.Windows.Forms.ToolStripButton btnPreviousPart;
        private System.Windows.Forms.ToolStripButton btnNextPart;
        private System.Windows.Forms.ToolStripButton btnLastPart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnJumpBackward;
        private System.Windows.Forms.ToolStripButton btnJumpForward;
        private System.Windows.Forms.ToolStripButton btnPauseResume;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnSlower;
        private System.Windows.Forms.ToolStripButton btnFaster;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnSmaller;
        private System.Windows.Forms.ToolStripButton btnWider;
    }
}