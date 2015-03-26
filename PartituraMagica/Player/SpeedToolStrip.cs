using System;
using System.Windows.Forms;

namespace PartituraMagica
{
    /**
     * A custom control that allows to increase/decrease speed 
     * and to pause/resume playing
     */
    public partial class SpeedToolStrip : ToolStrip
    {
        private bool paused;
        public bool Paused
        {
            get { return paused; }
            set
            {
                btnPause.Text = value ? "Resume" : "Pause";
                btnPause.Checked = value;
                paused = value;
            }
        }

        public int JumpsCount { get; set; }

        public float Speed { get; set; }

        public SpeedToolStrip()
        {
            InitializeComponent();
            btnSlower.Click += new EventHandler(btnSlower_Click);
            btnPause.Click += new EventHandler(btnPause_Click);
            btnFaster.Click += new EventHandler(btnFaster_Click);
            btnJumpBack.Click += BtnJumpBack_Click;
            btnJumpForward.Click += BtnJumpForward_Click;

            this.Paused = true;
        }

        private void BtnJumpForward_Click(object sender, EventArgs e)
        {
            this.JumpsCount--;
        }

        private void BtnJumpBack_Click(object sender, EventArgs e)
        {
            this.JumpsCount++;
        }

        void btnFaster_Click(object sender, EventArgs e)
        {
            this.Speed *= 1.2F;
        }

        void btnPause_Click(object sender, EventArgs e)
        {
            this.Paused = !this.Paused;
        }

        void btnSlower_Click(object sender, EventArgs e)
        {
            this.Speed *= 0.8F;
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Oemplus:
                case Keys.Add:
                    if (e.Modifiers == Keys.None)
                    {
                        btnFaster.PerformClick();
                    }
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    if (e.Modifiers == Keys.None)
                    {
                        btnSlower.PerformClick();
                    }
                    break;
                case Keys.Space:
                    this.paused = true;
                    break;
            }

        }

        public void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    this.Paused = false;//use property setter
                    break;
                case Keys.Left:
                    if (e.Modifiers == Keys.None)
                    {
                        btnJumpBack.PerformClick();
                    }
                    break;
                case Keys.Right:
                    if (e.Modifiers == Keys.None)
                    {
                        btnJumpForward.PerformClick();
                    }
                    break;
            }
        }
    }
}
