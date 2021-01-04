#region usings

using System.Windows.Forms;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.App.Player
{
	public class KeyboardMap
	{
		private readonly IPlayerController _playerController;
		private readonly IPlayerView _playerView;

		public KeyboardMap(IPlayerController playerController, IPlayerView playerView)
		{
			_playerController = playerController;
			_playerView = playerView;
		}

		public void HandleKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F4:
					if (e.Modifiers == Keys.Alt) _playerView.Close();
					break;
				case Keys.Escape:
					_playerView.Close();
					break;
				case Keys.Oemplus:
				case Keys.Add:
					if (e.Modifiers == Keys.None) _playerController.IncreaseSpeed();
					break;
				case Keys.OemMinus:
				case Keys.Subtract:
					if (e.Modifiers == Keys.None) _playerController.DecreaseSpeed();
					break;
				case Keys.Space:
					_playerController.Pause();
					break;
			}

			if (e.Control)
				switch (e.KeyCode)
				{
					case Keys.OemMinus:
					case Keys.Subtract:
						_playerController.ZoomOut(e.Shift);
						break;
					case Keys.Oemplus:
					case Keys.Add:
						_playerController.ZoomIn(e.Shift);
						break;
					case Keys.Left:
						_playerController.GoToPreviousPart();
						break;
					case Keys.Right:
						_playerController.GoToNextPart();
						break;
					case Keys.Home:
						_playerController.GoToFirstPart();
						break;
					case Keys.End:
						_playerController.GoToLastPart();
						break;
				}
		}

		public void HandleKeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Space:
					if (_playerController.IsPartituraFinished()) _playerController.GoToFirstPart();
					_playerController.Resume();
					break;
				case Keys.Left:
					if (e.Modifiers == Keys.None) _playerController.JumpBackward();
					break;
				case Keys.Right:
					if (e.Modifiers == Keys.None) _playerController.JumpForward();
					break;
			}
		}
	}
}