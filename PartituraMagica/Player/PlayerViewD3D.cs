#region usings

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using FontAwesome.Sharp;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;
using IWin32Window = System.Windows.Forms.IWin32Window;

#endregion

namespace PartituraMagica.App.Player
{
	/**
     * Uses Direct3D to implement a smooth scrolling view.
     * Contains custom controls for navigating, zooming, speed control
     */
	public partial class PlayerViewD3D : Form, IPlayerView
	{
		private class Win32Window : IWin32Window
		{
			public Win32Window(IntPtr w)
			{
				Handle = w;
			}

			public IntPtr Handle { get; }
		}

		private const int PART_A_BUTTON_INDEX = 2;

		private readonly IntPtr _ownerHandle;
		private readonly IScrollingPanel _scrollingPanel;

		private KeyboardMap _keyboardMap;

		private IPlayerController _playerController;

		public PlayerViewD3D(MainWindow mainWindow, IScrollingPanel scrollingPanel)
		{
			InitializeComponent();
			WindowState = FormWindowState.Maximized;

			_ownerHandle = new WindowInteropHelper(mainWindow).Handle;

			_scrollingPanel = scrollingPanel;
			_scrollingPanel.Init(panel1.Handle);
		}

		public void StartScrolling(IPlayedPartitura partitura, IPlayerController playerController)
		{
			_playerController = playerController;
			_keyboardMap = new KeyboardMap(_playerController, this);

			CreatePartButtons(partitura.PartPattern);

			IWin32Window w = new Win32Window(_ownerHandle);

			Enabled = true;
			Show(w);

			_playerController.PlayerShown();
			_scrollingPanel.StartScrolling(partitura);

			//we return here only if window is closed
			_playerController.PlayerClosed();
			Enabled = false;
		}

		public void MarkActivePart(int index)
		{
			int buttonCount = toolStrip1.Items.Count - 2 * PART_A_BUTTON_INDEX;
			if (buttonCount < index + 1)
				//this check is required. When you play to second part, then close and reopen player the Items list is empty sometimes.
				return;

			for (int i = 0; i < buttonCount; i++)
			{
				var button = (ToolStripButton) toolStrip1.Items[i + PART_A_BUTTON_INDEX];
				button.Checked = i == index;
			}
		}

		public void MarkPaused(bool isPaused)
		{
			btnPauseResume.Checked = isPaused;
			btnPauseResume.IconChar = isPaused
				? IconChar.Play
				: IconChar.Pause;
		}

		private void CreatePartButtons(string partPattern)
		{
			string[] partStrings = new string[partPattern.Length];
			for (int i = 0; i < partPattern.Length; i++) partStrings[i] = partPattern[i].ToString();
			int partIndex = partStrings.Length - 1;
			foreach (var partString in partStrings.Reverse())
			{
				var button = new IconToolStripButton
				{
					Name = partString,
					Text = partString,
					AutoSize = false,
					DisplayStyle = ToolStripItemDisplayStyle.Text,
					Font = new Font("Segoe UI", 26F, FontStyle.Bold),
					IconChar = IconChar.StepBackward,
					IconColor = Color.Black,
					IconFont = IconFont.Auto,
					IconSize = 40,
					ImageScaling = ToolStripItemImageScaling.None,
					ImageTransparentColor = Color.Magenta,
					Margin = new Padding(0),
					//intentionally bigger than buttons from designer to avoid vertical misalign issue
					Size = new Size(52, 60),
					TextAlign = ContentAlignment.TopCenter
				};
				toolStrip1.Items.Insert(PART_A_BUTTON_INDEX, button);

				int index = partIndex;
				button.Click += (sender, args) => { _playerController.GoToPart(index); };
				partIndex--;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) components?.Dispose();
			base.Dispose(disposing);

			_scrollingPanel?.Dispose();
		}

		private void PlayerViewD3D_KeyDown(object sender, KeyEventArgs e)
		{
			_keyboardMap.HandleKeyDown(sender, e);
		}

		private void PlayerViewD3D_KeyUp(object sender, KeyEventArgs e)
		{
			_keyboardMap.HandleKeyUp(sender, e);
		}

		private void btnFirstPart_Click(object sender, EventArgs e)
		{
			_playerController.GoToFirstPart();
		}

		private void btnPreviousPart_Click(object sender, EventArgs e)
		{
			_playerController.GoToPreviousPart();
		}

		private void btnNextPart_Click(object sender, EventArgs e)
		{
			_playerController.GoToNextPart();
		}

		private void btnLastPart_Click(object sender, EventArgs e)
		{
			_playerController.GoToLastPart();
		}

		private void btnJumpBackward_Click(object sender, EventArgs e)
		{
			_playerController.JumpBackward();
		}

		private void btnPauseResume_Click(object sender, EventArgs e)
		{
			_playerController.TogglePause();
		}

		private void btnJumpForward_Click(object sender, EventArgs e)
		{
			_playerController.JumpForward();
		}

		private void btnSlower_Click(object sender, EventArgs e)
		{
			_playerController.DecreaseSpeed();
		}

		private void btnFaster_Click(object sender, EventArgs e)
		{
			_playerController.IncreaseSpeed();
		}

		private void btnZoomIn_Click(object sender, EventArgs e)
		{
			_playerController.ZoomIn(false);
		}

		private void btnWider_Click(object sender, EventArgs e)
		{
			_playerController.ZoomIn(true);
		}

		private void btnZoomOut_Click(object sender, EventArgs e)
		{
			_playerController.ZoomOut(false);
		}

		private void btnSmaller_Click(object sender, EventArgs e)
		{
			_playerController.ZoomOut(true);
		}

		private void btnRemoveCurrentSnippet_Click(object sender, EventArgs e)
		{
		}

		private void btnRecordSnippet_MouseDown(object sender, MouseEventArgs e)
		{
			_playerController.StartSnippetRecording();
		}

		private void btnRecordSnippet_MouseUp(object sender, MouseEventArgs e)
		{
			_playerController.StopSnippetRecording();
		}

		private void splitBtnRepeatAll_Click(object sender, EventArgs e)
		{
			_playerController.RepeatAllSnippets();
		}
	}
}