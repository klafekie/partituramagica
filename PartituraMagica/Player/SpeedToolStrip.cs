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
        private bool _paused;
        private IPartNavigator _navigator;

        public SpeedToolStrip()
        {
            InitializeComponent();
            btnSlower.Click += btnSlower_Click;
            btnPause.Click += btnPause_Click;
            btnFaster.Click += btnFaster_Click;
            btnJumpBack.Click += BtnJumpBack_Click;
            btnJumpForward.Click += BtnJumpForward_Click;

            Paused = true;
        }

        

        public bool Paused
        {
            get { return _paused; }
            set
            {
                btnPause.Text = value ? "Resume" : "Pause";
                btnPause.Checked = value;
                _paused = value;
            }
        }

        public float Speed { get; set; }

        private void BtnJumpForward_Click(object sender, EventArgs e)
        {
            _navigator?.JumpForwardClicked();
        }

        private void BtnJumpBack_Click(object sender, EventArgs e)
        {
            _navigator?.JumpBackwardClicked();
        }

        private void btnFaster_Click(object sender, EventArgs e)
        {
            Speed *= 1.2F;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Paused = !Paused;
        }

        private void btnSlower_Click(object sender, EventArgs e)
        {
            Speed *= 0.8F;
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
                    _paused = true;
                    break;
            }
        }

        public void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    Paused = false; //use property setter
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

        public void RegisterEvents(IPartNavigator navigator)
        {
            _navigator = navigator;
        }
    }
}