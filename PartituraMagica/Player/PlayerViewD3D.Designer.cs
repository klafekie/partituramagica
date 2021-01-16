namespace PartituraMagica.App.Player
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
			this.toolStrip5 = new System.Windows.Forms.ToolStrip();
			this.btnRecordSnippet = new FontAwesome.Sharp.IconToolStripButton();
			this.splitBtnRepeatAll = new FontAwesome.Sharp.IconSplitButton();
			this.btnRepeatSnippet1 = new FontAwesome.Sharp.IconMenuItem();
			this.comboBtnEditSnippet = new FontAwesome.Sharp.IconDropDownButton();
			this.btnRemoveCurrentSnippet = new FontAwesome.Sharp.IconMenuItem();
			this.btnRemoveAllSnippets = new FontAwesome.Sharp.IconMenuItem();
			this.toolStrip4 = new System.Windows.Forms.ToolStrip();
			this.btnZoomIn = new FontAwesome.Sharp.IconToolStripButton();
			this.btnWider = new FontAwesome.Sharp.IconToolStripButton();
			this.btnZoomOut = new FontAwesome.Sharp.IconToolStripButton();
			this.btnSmaller = new FontAwesome.Sharp.IconToolStripButton();
			this.toolStrip3 = new System.Windows.Forms.ToolStrip();
			this.btnSlower = new FontAwesome.Sharp.IconToolStripButton();
			this.btnFaster = new FontAwesome.Sharp.IconToolStripButton();
			this.toolStrip2 = new System.Windows.Forms.ToolStrip();
			this.btnJumpBackward = new FontAwesome.Sharp.IconToolStripButton();
			this.btnPauseResume = new FontAwesome.Sharp.IconToolStripButton();
			this.btnJumpForward = new FontAwesome.Sharp.IconToolStripButton();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnFirstPart = new FontAwesome.Sharp.IconToolStripButton();
			this.btnPreviousPart = new FontAwesome.Sharp.IconToolStripButton();
			this.btnNextPart = new FontAwesome.Sharp.IconToolStripButton();
			this.btnLastPart = new FontAwesome.Sharp.IconToolStripButton();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.toolStrip5.SuspendLayout();
			this.toolStrip4.SuspendLayout();
			this.toolStrip3.SuspendLayout();
			this.toolStrip2.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1264, 706);
			this.panel1.TabIndex = 1;
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.panel1);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1264, 706);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(1264, 761);
			this.toolStripContainer1.TabIndex = 2;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip5);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip4);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip3);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip2);
			this.toolStripContainer1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyDown);
			this.toolStripContainer1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PlayerViewD3D_KeyUp);
			// 
			// toolStrip5
			// 
			this.toolStrip5.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRecordSnippet,
            this.splitBtnRepeatAll,
            this.comboBtnEditSnippet});
			this.toolStrip5.Location = new System.Drawing.Point(181, 0);
			this.toolStrip5.Name = "toolStrip5";
			this.toolStrip5.Size = new System.Drawing.Size(208, 55);
			this.toolStrip5.TabIndex = 4;
			// 
			// btnRecordSnippet
			// 
			this.btnRecordSnippet.AutoSize = false;
			this.btnRecordSnippet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnRecordSnippet.IconChar = FontAwesome.Sharp.IconChar.Thumbtack;
			this.btnRecordSnippet.IconColor = System.Drawing.Color.Black;
			this.btnRecordSnippet.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnRecordSnippet.IconSize = 40;
			this.btnRecordSnippet.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnRecordSnippet.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnRecordSnippet.Name = "btnRecordSnippet";
			this.btnRecordSnippet.Rotation = 45D;
			this.btnRecordSnippet.Size = new System.Drawing.Size(52, 52);
			this.btnRecordSnippet.Text = "Record Snippet";
			this.btnRecordSnippet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRecordSnippet_MouseDown);
			this.btnRecordSnippet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnRecordSnippet_MouseUp);
			// 
			// splitBtnRepeatAll
			// 
			this.splitBtnRepeatAll.AutoSize = false;
			this.splitBtnRepeatAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.splitBtnRepeatAll.DropDownButtonWidth = 20;
			this.splitBtnRepeatAll.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRepeatSnippet1});
			this.splitBtnRepeatAll.Flip = FontAwesome.Sharp.FlipOrientation.Normal;
			this.splitBtnRepeatAll.Font = new System.Drawing.Font("Segoe UI", 26F);
			this.splitBtnRepeatAll.IconChar = FontAwesome.Sharp.IconChar.SyncAlt;
			this.splitBtnRepeatAll.IconColor = System.Drawing.Color.Black;
			this.splitBtnRepeatAll.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.splitBtnRepeatAll.IconSize = 40;
			this.splitBtnRepeatAll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.splitBtnRepeatAll.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.splitBtnRepeatAll.Name = "splitBtnRepeatAll";
			this.splitBtnRepeatAll.Rotation = 0D;
			this.splitBtnRepeatAll.Size = new System.Drawing.Size(72, 52);
			this.splitBtnRepeatAll.Text = "Repeat All Snippets";
			this.splitBtnRepeatAll.ToolTipText = "Repeat All Snippets";
			this.splitBtnRepeatAll.ButtonClick += new System.EventHandler(this.splitBtnRepeatAll_Click);
			// 
			// btnRepeatSnippet1
			// 
			this.btnRepeatSnippet1.AutoSize = false;
			this.btnRepeatSnippet1.IconChar = FontAwesome.Sharp.IconChar.DiceFive;
			this.btnRepeatSnippet1.IconColor = System.Drawing.Color.Black;
			this.btnRepeatSnippet1.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnRepeatSnippet1.IconSize = 40;
			this.btnRepeatSnippet1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnRepeatSnippet1.Name = "btnRepeatSnippet1";
			this.btnRepeatSnippet1.Size = new System.Drawing.Size(446, 52);
			this.btnRepeatSnippet1.Text = "Repeat 1 only";
			this.btnRepeatSnippet1.ToolTipText = "Repeat Snippet 1";
			// 
			// comboBtnEditSnippet
			// 
			this.comboBtnEditSnippet.AutoSize = false;
			this.comboBtnEditSnippet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.comboBtnEditSnippet.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRemoveCurrentSnippet,
            this.btnRemoveAllSnippets});
			this.comboBtnEditSnippet.Font = new System.Drawing.Font("Segoe UI", 26F);
			this.comboBtnEditSnippet.IconChar = FontAwesome.Sharp.IconChar.TrashAlt;
			this.comboBtnEditSnippet.IconColor = System.Drawing.Color.Black;
			this.comboBtnEditSnippet.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.comboBtnEditSnippet.IconSize = 40;
			this.comboBtnEditSnippet.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.comboBtnEditSnippet.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.comboBtnEditSnippet.Name = "comboBtnEditSnippet";
			this.comboBtnEditSnippet.Size = new System.Drawing.Size(72, 52);
			this.comboBtnEditSnippet.Text = "Delete Snippets";
			// 
			// btnRemoveCurrentSnippet
			// 
			this.btnRemoveCurrentSnippet.IconChar = FontAwesome.Sharp.IconChar.DiceFour;
			this.btnRemoveCurrentSnippet.IconColor = System.Drawing.Color.Black;
			this.btnRemoveCurrentSnippet.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnRemoveCurrentSnippet.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnRemoveCurrentSnippet.Name = "btnRemoveCurrentSnippet";
			this.btnRemoveCurrentSnippet.Size = new System.Drawing.Size(511, 54);
			this.btnRemoveCurrentSnippet.Text = "Remove Current Snippet";
			this.btnRemoveCurrentSnippet.ToolTipText = "Remove Snippet 1";
			this.btnRemoveCurrentSnippet.Click += new System.EventHandler(this.btnRemoveCurrentSnippet_Click);
			// 
			// btnRemoveAllSnippets
			// 
			this.btnRemoveAllSnippets.IconChar = FontAwesome.Sharp.IconChar.Dice;
			this.btnRemoveAllSnippets.IconColor = System.Drawing.Color.Black;
			this.btnRemoveAllSnippets.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnRemoveAllSnippets.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnRemoveAllSnippets.Name = "btnRemoveAllSnippets";
			this.btnRemoveAllSnippets.Size = new System.Drawing.Size(511, 54);
			this.btnRemoveAllSnippets.Text = "Remove All Snippets";
			this.btnRemoveAllSnippets.ToolTipText = "Remove All Snippets";
			this.btnRemoveAllSnippets.Click += new System.EventHandler(this.btnRemoveAllSnippets_Click);
			// 
			// toolStrip4
			// 
			this.toolStrip4.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomIn,
            this.btnWider,
            this.btnZoomOut,
            this.btnSmaller});
			this.toolStrip4.Location = new System.Drawing.Point(389, 0);
			this.toolStrip4.Name = "toolStrip4";
			this.toolStrip4.Size = new System.Drawing.Size(230, 55);
			this.toolStrip4.TabIndex = 3;
			// 
			// btnZoomIn
			// 
			this.btnZoomIn.AutoSize = false;
			this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnZoomIn.IconChar = FontAwesome.Sharp.IconChar.SearchPlus;
			this.btnZoomIn.IconColor = System.Drawing.Color.Black;
			this.btnZoomIn.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnZoomIn.IconSize = 40;
			this.btnZoomIn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnZoomIn.Name = "btnZoomIn";
			this.btnZoomIn.Size = new System.Drawing.Size(52, 52);
			this.btnZoomIn.Text = "Zoom In";
			this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
			// 
			// btnWider
			// 
			this.btnWider.AutoSize = false;
			this.btnWider.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnWider.IconChar = FontAwesome.Sharp.IconChar.ExpandAlt;
			this.btnWider.IconColor = System.Drawing.Color.Black;
			this.btnWider.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnWider.IconSize = 40;
			this.btnWider.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnWider.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnWider.Name = "btnWider";
			this.btnWider.Rotation = 45D;
			this.btnWider.Size = new System.Drawing.Size(52, 52);
			this.btnWider.Text = "Widen";
			this.btnWider.Click += new System.EventHandler(this.btnWider_Click);
			// 
			// btnZoomOut
			// 
			this.btnZoomOut.AutoSize = false;
			this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnZoomOut.IconChar = FontAwesome.Sharp.IconChar.SearchMinus;
			this.btnZoomOut.IconColor = System.Drawing.Color.Black;
			this.btnZoomOut.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnZoomOut.IconSize = 40;
			this.btnZoomOut.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnZoomOut.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
			this.btnZoomOut.Name = "btnZoomOut";
			this.btnZoomOut.Size = new System.Drawing.Size(52, 52);
			this.btnZoomOut.Text = "Zoom Out";
			this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
			// 
			// btnSmaller
			// 
			this.btnSmaller.AutoSize = false;
			this.btnSmaller.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnSmaller.IconChar = FontAwesome.Sharp.IconChar.CompressAlt;
			this.btnSmaller.IconColor = System.Drawing.Color.Black;
			this.btnSmaller.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnSmaller.IconSize = 40;
			this.btnSmaller.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnSmaller.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSmaller.Name = "btnSmaller";
			this.btnSmaller.Rotation = 45D;
			this.btnSmaller.Size = new System.Drawing.Size(52, 52);
			this.btnSmaller.Text = "Narrow";
			this.btnSmaller.Click += new System.EventHandler(this.btnSmaller_Click);
			// 
			// toolStrip3
			// 
			this.toolStrip3.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSlower,
            this.btnFaster});
			this.toolStrip3.Location = new System.Drawing.Point(619, 0);
			this.toolStrip3.Name = "toolStrip3";
			this.toolStrip3.Size = new System.Drawing.Size(116, 55);
			this.toolStrip3.TabIndex = 2;
			// 
			// btnSlower
			// 
			this.btnSlower.AutoSize = false;
			this.btnSlower.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnSlower.IconChar = FontAwesome.Sharp.IconChar.Hiking;
			this.btnSlower.IconColor = System.Drawing.Color.Black;
			this.btnSlower.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnSlower.IconSize = 40;
			this.btnSlower.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnSlower.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSlower.Name = "btnSlower";
			this.btnSlower.Size = new System.Drawing.Size(52, 52);
			this.btnSlower.Text = "Slower";
			this.btnSlower.Click += new System.EventHandler(this.btnSlower_Click);
			// 
			// btnFaster
			// 
			this.btnFaster.AutoSize = false;
			this.btnFaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFaster.IconChar = FontAwesome.Sharp.IconChar.Running;
			this.btnFaster.IconColor = System.Drawing.Color.Black;
			this.btnFaster.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnFaster.IconSize = 40;
			this.btnFaster.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnFaster.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFaster.Name = "btnFaster";
			this.btnFaster.Size = new System.Drawing.Size(52, 52);
			this.btnFaster.Text = "Faster";
			this.btnFaster.Click += new System.EventHandler(this.btnFaster_Click);
			// 
			// toolStrip2
			// 
			this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnJumpBackward,
            this.btnPauseResume,
            this.btnJumpForward});
			this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.toolStrip2.Location = new System.Drawing.Point(3, 0);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.Size = new System.Drawing.Size(178, 55);
			this.toolStrip2.TabIndex = 1;
			// 
			// btnJumpBackward
			// 
			this.btnJumpBackward.AutoSize = false;
			this.btnJumpBackward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnJumpBackward.IconChar = FontAwesome.Sharp.IconChar.Backward;
			this.btnJumpBackward.IconColor = System.Drawing.Color.Black;
			this.btnJumpBackward.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnJumpBackward.IconSize = 40;
			this.btnJumpBackward.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnJumpBackward.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnJumpBackward.Name = "btnJumpBackward";
			this.btnJumpBackward.Size = new System.Drawing.Size(52, 52);
			this.btnJumpBackward.Text = "Jump Back";
			this.btnJumpBackward.Click += new System.EventHandler(this.btnJumpBackward_Click);
			// 
			// btnPauseResume
			// 
			this.btnPauseResume.AutoSize = false;
			this.btnPauseResume.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnPauseResume.IconChar = FontAwesome.Sharp.IconChar.Play;
			this.btnPauseResume.IconColor = System.Drawing.Color.Black;
			this.btnPauseResume.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnPauseResume.IconSize = 40;
			this.btnPauseResume.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnPauseResume.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnPauseResume.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
			this.btnPauseResume.Name = "btnPauseResume";
			this.btnPauseResume.Size = new System.Drawing.Size(52, 52);
			this.btnPauseResume.Text = "Resume";
			this.btnPauseResume.Click += new System.EventHandler(this.btnPauseResume_Click);
			// 
			// btnJumpForward
			// 
			this.btnJumpForward.AutoSize = false;
			this.btnJumpForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnJumpForward.IconChar = FontAwesome.Sharp.IconChar.Forward;
			this.btnJumpForward.IconColor = System.Drawing.Color.Black;
			this.btnJumpForward.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnJumpForward.IconSize = 40;
			this.btnJumpForward.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnJumpForward.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnJumpForward.Name = "btnJumpForward";
			this.btnJumpForward.Size = new System.Drawing.Size(52, 52);
			this.btnJumpForward.Text = "Jump Forward";
			this.btnJumpForward.Click += new System.EventHandler(this.btnJumpForward_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFirstPart,
            this.btnPreviousPart,
            this.btnNextPart,
            this.btnLastPart});
			this.toolStrip1.Location = new System.Drawing.Point(735, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(251, 55);
			this.toolStrip1.TabIndex = 0;
			// 
			// btnFirstPart
			// 
			this.btnFirstPart.AutoSize = false;
			this.btnFirstPart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFirstPart.IconChar = FontAwesome.Sharp.IconChar.FastBackward;
			this.btnFirstPart.IconColor = System.Drawing.Color.Black;
			this.btnFirstPart.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnFirstPart.IconSize = 40;
			this.btnFirstPart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnFirstPart.Name = "btnFirstPart";
			this.btnFirstPart.Size = new System.Drawing.Size(52, 52);
			this.btnFirstPart.Text = "Show First Part";
			this.btnFirstPart.Click += new System.EventHandler(this.btnFirstPart_Click);
			// 
			// btnPreviousPart
			// 
			this.btnPreviousPart.AutoSize = false;
			this.btnPreviousPart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnPreviousPart.IconChar = FontAwesome.Sharp.IconChar.StepBackward;
			this.btnPreviousPart.IconColor = System.Drawing.Color.Black;
			this.btnPreviousPart.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnPreviousPart.IconSize = 40;
			this.btnPreviousPart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnPreviousPart.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnPreviousPart.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
			this.btnPreviousPart.Name = "btnPreviousPart";
			this.btnPreviousPart.Size = new System.Drawing.Size(52, 52);
			this.btnPreviousPart.Text = "Show Next Part";
			this.btnPreviousPart.Click += new System.EventHandler(this.btnPreviousPart_Click);
			// 
			// btnNextPart
			// 
			this.btnNextPart.AutoSize = false;
			this.btnNextPart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnNextPart.IconChar = FontAwesome.Sharp.IconChar.StepForward;
			this.btnNextPart.IconColor = System.Drawing.Color.Black;
			this.btnNextPart.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnNextPart.IconSize = 40;
			this.btnNextPart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnNextPart.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnNextPart.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
			this.btnNextPart.Name = "btnNextPart";
			this.btnNextPart.Size = new System.Drawing.Size(52, 52);
			this.btnNextPart.Text = "Show Next Part";
			this.btnNextPart.Click += new System.EventHandler(this.btnNextPart_Click);
			// 
			// btnLastPart
			// 
			this.btnLastPart.AutoSize = false;
			this.btnLastPart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnLastPart.IconChar = FontAwesome.Sharp.IconChar.FastForward;
			this.btnLastPart.IconColor = System.Drawing.Color.Black;
			this.btnLastPart.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.btnLastPart.IconSize = 40;
			this.btnLastPart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnLastPart.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnLastPart.Name = "btnLastPart";
			this.btnLastPart.Size = new System.Drawing.Size(52, 52);
			this.btnLastPart.Text = "Show Last Part";
			this.btnLastPart.Click += new System.EventHandler(this.btnLastPart_Click);
			// 
			// PlayerViewD3D
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1264, 761);
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
			this.toolStrip5.ResumeLayout(false);
			this.toolStrip5.PerformLayout();
			this.toolStrip4.ResumeLayout(false);
			this.toolStrip4.PerformLayout();
			this.toolStrip3.ResumeLayout(false);
			this.toolStrip3.PerformLayout();
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
		private FontAwesome.Sharp.IconToolStripButton btnFirstPart;
		private FontAwesome.Sharp.IconToolStripButton btnNextPart;
		private FontAwesome.Sharp.IconToolStripButton btnLastPart;
		private FontAwesome.Sharp.IconToolStripButton btnPreviousPart;
		private System.Windows.Forms.ToolStrip toolStrip2;
		private FontAwesome.Sharp.IconToolStripButton btnJumpBackward;
		private FontAwesome.Sharp.IconToolStripButton btnPauseResume;
		private FontAwesome.Sharp.IconToolStripButton btnJumpForward;
		private System.Windows.Forms.ToolStrip toolStrip3;
		private FontAwesome.Sharp.IconToolStripButton btnSlower;
		private FontAwesome.Sharp.IconToolStripButton btnFaster;
		private System.Windows.Forms.ToolStrip toolStrip4;
		private FontAwesome.Sharp.IconToolStripButton btnZoomIn;
		private FontAwesome.Sharp.IconToolStripButton btnWider;
		private FontAwesome.Sharp.IconToolStripButton btnZoomOut;
		private FontAwesome.Sharp.IconToolStripButton btnSmaller;
		private System.Windows.Forms.ToolStrip toolStrip5;
		private FontAwesome.Sharp.IconSplitButton splitBtnRepeatAll;
		private FontAwesome.Sharp.IconMenuItem btnRepeatSnippet1;
		private FontAwesome.Sharp.IconToolStripButton btnRecordSnippet;
		private FontAwesome.Sharp.IconDropDownButton comboBtnEditSnippet;
		private FontAwesome.Sharp.IconMenuItem btnRemoveCurrentSnippet;
		private FontAwesome.Sharp.IconMenuItem btnRemoveAllSnippets;
	}
}