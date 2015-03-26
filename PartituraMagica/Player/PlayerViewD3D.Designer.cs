namespace PartituraMagica
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.speedStrip = new PartituraMagica.SpeedToolStrip();
            this.partsStrip = new PartituraMagica.ChoiceNavigatorStrip();
            this.zoomStrip = new PartituraMagica.ZoomToolStrip();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(773, 223);
            this.panel1.TabIndex = 1;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(773, 223);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(773, 266);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.partsStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.speedStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.zoomStrip);
            this.toolStripContainer1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyDown);
            this.toolStripContainer1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyUp);
            // 
            // speedStrip
            // 
            this.speedStrip.BackwardJumpsCount = 0;
            this.speedStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.speedStrip.Font = new System.Drawing.Font("Lucida Console", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.speedStrip.Location = new System.Drawing.Point(303, 0);
            this.speedStrip.MinimumSize = new System.Drawing.Size(0, 43);
            this.speedStrip.Name = "speedStrip";
            this.speedStrip.Paused = true;
            this.speedStrip.Size = new System.Drawing.Size(432, 43);
            this.speedStrip.Speed = 0F;
            this.speedStrip.TabIndex = 1;
            this.speedStrip.Text = "Speed";
            // 
            // partsStrip
            // 
            this.partsStrip.ChoiceName = "Part";
            this.partsStrip.Choices = new string[] {
        "A",
        "B",
        "C",
        "D"};
            this.partsStrip.Current = 0;
            this.partsStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.partsStrip.Font = new System.Drawing.Font("Lucida Console", 18F, System.Drawing.FontStyle.Bold);
            this.partsStrip.Location = new System.Drawing.Point(3, 0);
            this.partsStrip.Modifiers = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)));
            this.partsStrip.Name = "partsStrip";
            this.partsStrip.Size = new System.Drawing.Size(300, 43);
            this.partsStrip.TabIndex = 2;
            // 
            // zoomStrip
            // 
            this.zoomStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.zoomStrip.Location = new System.Drawing.Point(735, 0);
            this.zoomStrip.MinimumSize = new System.Drawing.Size(0, 43);
            this.zoomStrip.Name = "zoomStrip";
            this.zoomStrip.Size = new System.Drawing.Size(38, 43);
            this.zoomStrip.TabIndex = 0;
            this.zoomStrip.Text = "zoomToolStrip1";
            // 
            // PlayerViewD3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 266);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "PlayerViewD3D";
            this.Text = "PlayerView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayerViewD3D_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyUp);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private SpeedToolStrip speedStrip;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private ChoiceNavigatorStrip partsStrip;
        private ZoomToolStrip zoomStrip;

    }
}