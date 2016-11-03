using System.Windows.Forms;

namespace PartituraMagica.Player
{
    public class KeyboardMap
    {
        private readonly SpeedManager _speedManager;
        private readonly ZoomManager _zoomManager;
        private readonly IPartNavigator _navigator;
        private readonly IPlayerView _playerView;

        public KeyboardMap(SpeedManager speedManager, ZoomManager zoomManager, IPartNavigator navigator, IPlayerView playerView)
        {
            _speedManager = speedManager;
            _zoomManager = zoomManager;
            _navigator = navigator;
            _playerView = playerView;
        }
        
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F4:
                    if (e.Modifiers == Keys.Alt)
                    {
                        _playerView.Close();
                    }
                    break;
                case Keys.Escape:
                    _playerView.Close();
                    break;
                case Keys.Oemplus:
                case Keys.Add:
                    if (e.Modifiers == Keys.None)
                    {
                        _speedManager.IncreaseSpeed();
                    }
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    if (e.Modifiers == Keys.None)
                    {
                        _speedManager.DecreaseSpeed();
                    }
                    break;
                case Keys.Space:
                    _speedManager.IsPaused = true;
                    break;
            }
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        _zoomManager.ZoomOut(e.Shift);
                        break;
                    case Keys.Oemplus:
                    case Keys.Add:
                        _zoomManager.ZoomIn(e.Shift);
                        break;
                    case Keys.Left:
                        _navigator.PreviousPartClicked();
                        break;
                    case Keys.Right:
                        _navigator.NextPartClicked();
                        break;
                    case Keys.Home:
                        _navigator.FirstPartClicked();
                        break;
                    case Keys.End:
                        _navigator.LastPartClicked();
                        break;
                }
            }
        }

        public void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (_navigator.IsPartituraFinished())
                    {
                        _navigator.FirstPartClicked();
                    }
                    _speedManager.IsPaused = false; //use property setter
                    break;
                case Keys.Left:
                    if (e.Modifiers == Keys.None)
                    {
                        _navigator.JumpBackwardClicked();
                    }
                    break;
                case Keys.Right:
                    if (e.Modifiers == Keys.None)
                    {
                        _navigator.JumpForwardClicked();
                    }
                    break;
            }
        }
    }
}