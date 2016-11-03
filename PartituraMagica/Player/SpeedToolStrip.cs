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
        private IPartNavigator _navigator;
        private SpeedManager _speedManager;

        public SpeedToolStrip()
        {
            InitializeComponent();
            btnSlower.Click += btnSlower_Click;
            btnPause.Click += btnPause_Click;
            btnFaster.Click += btnFaster_Click;
            btnJumpBack.Click += BtnJumpBack_Click;
            btnJumpForward.Click += BtnJumpForward_Click;
        }


        private void BtnJumpForward_Click(object sender, EventArgs e)
        {
            _navigator.JumpForwardClicked();
        }

        private void BtnJumpBack_Click(object sender, EventArgs e)
        {
            _navigator.JumpBackwardClicked();
        }

        private void btnFaster_Click(object sender, EventArgs e)
        {
            _speedManager.IncreaseSpeed();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _speedManager.TogglePaused();
        }

        private void btnSlower_Click(object sender, EventArgs e)
        {
            _speedManager.DecreaseSpeed();
        }

        public void RegisterEvents(IPartNavigator navigator, SpeedManager speedManager)
        {
            _navigator = navigator;
            _speedManager = speedManager;
            _speedManager.IsPausedChanged += SpeedManagerOnIsPausedChanged;
        }

        private void SpeedManagerOnIsPausedChanged()
        {
            btnPause.Text = _speedManager.IsPaused ? "Resume" : "Pause";
            btnPause.Checked = _speedManager.IsPaused;
        }
    }
}