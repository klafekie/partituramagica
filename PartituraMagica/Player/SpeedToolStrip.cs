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
                //				OnSpeedChanged(this.Speed);
            }
        }

        public int BackwardJumpsCount { get; set; }

        private float speed = 1F;
        public float Speed
        {
            get { return this.paused ? 0F : this.speed; }
            set
            {
                if (value == 0)
                {
                    //avoid that speed ever becomes 0, cause we use
                    //multiplication to increase/decrease it
                    this.paused = true;
                }
                else
                {
                    //if (this.speed != value) {
                    //    OnSpeedChanged(this.Speed);//using getter here!
                    //}
                    this.speed = value;
                }
            }
        }

        public SpeedToolStrip()
        {
            InitializeComponent();
            btnSlower.Click += new EventHandler(btnSlower_Click);
            btnPause.Click += new EventHandler(btnPause_Click);
            btnFaster.Click += new EventHandler(btnFaster_Click);
            btnJumpBack.Click += BtnJumpBack_Click;

            this.Paused = true;
        }

        private void BtnJumpBack_Click(object sender, EventArgs e)
        {
            this.BackwardJumpsCount++;
        }

        void btnFaster_Click(object sender, EventArgs e)
        {
            this.speed *= 1.2F;
        }

        void btnPause_Click(object sender, EventArgs e)
        {
            this.Paused = !this.Paused;
        }

        void btnSlower_Click(object sender, EventArgs e)
        {
            this.speed *= 0.8F;
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Oemplus:
                case Keys.Add:
                    if (e.Modifiers == Keys.None)
                    {
                        this.speed *= 1.05F;
                    }
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    if (e.Modifiers == Keys.None)
                    {
                        this.speed *= 0.95F;
                    }
                    break;
                case Keys.Space:
                    this.Paused = true;
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
            }
        }        
    }
}
