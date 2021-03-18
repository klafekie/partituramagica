#region usings

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Interop;
using FontAwesome.Sharp;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;
using SlimDX;
using DialogResult = System.Windows.Forms.DialogResult;
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
		}

		public void Init()
		{
			_scrollingPanel.Init(panel1.Handle);
		}

		public bool ShowConfirmDialog(string message)
		{
			var result = MessageBox.Show(message, "Please confirm", MessageBoxButtons.OKCancel);
			return result == DialogResult.OK;
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
			try
			{
				_scrollingPanel.StartScrolling(partitura);
			}
			finally
			{
				Close();
				_playerController.PlayerClosed();
				Enabled = false;
			}
		}

		public void ExecuteDelayed(int milliseconds, Action action)
		{
			var timer = new Timer
			{
				Enabled = true,
				Interval = milliseconds
			};
			timer.Tick += (sender, args) =>
			{
				action();
				timer.Stop();
			};
			timer.Start();
		}

		public void MarkActivePart(int index)
		{
			if (index >= dropdownAllParts.DropDown.Items.Count) return;

			var button = (IconToolStripButton) dropdownAllParts.DropDown.Items[index];
			button.Select();
			dropdownAllParts.Text = button.Text;
		}

		public void MarkPaused(bool isPaused)
		{
			btnPauseResume.Checked = isPaused;
			btnPauseResume.IconChar = isPaused
				? IconChar.Play
				: IconChar.Pause;
		}

		public void UpdateAvailableSnippets(int snippetsCount)
		{
			splitBtnRepeatAll.DropDownItems.Clear();

			for (int i = 0; i < snippetsCount; i++)
			{
				var btn = new IconMenuItem
				{
					AutoSize = false,
					IconChar = GetIconChar(i),
					IconColor = Color.Black,
					IconFont = IconFont.Auto,
					IconSize = 40,
					ImageScaling = ToolStripItemImageScaling.None,
					Name = "btnRepeatSnippet" + i,
					Size = new Size(446, 52),
					Text = $"Go to snippet {i + 1}",
					ToolTipText = $"Go to snippet {i + 1}"
				};
				int i1 = i;
				btn.Click += (sender, args) => _playerController.GoToSnippet(i1);

				splitBtnRepeatAll.DropDownItems.Add(btn);
			}

			var endBtn = new IconMenuItem
			{
				AutoSize = false,
				IconChar = IconChar.WindowClose,
				IconColor = Color.Black,
				IconFont = IconFont.Auto,
				IconSize = 40,
				ImageScaling = ToolStripItemImageScaling.None,
				Name = "btnCancelSnippetPlaying",
				Size = new Size(446, 52),
				Text = "Stop playing snippets",
				ToolTipText = "Stop playing snippets"
			};
			endBtn.Click += (sender, args) => _playerController.StopPlayingSnippets();

			splitBtnRepeatAll.DropDownItems.Add(endBtn);
		}

		public void UpdateSnippetUi(SnippedUiState state)
		{
			btnRecordSnippet.Enabled = state.RecordIsEnabled;
			btnRecordSnippet.Checked = state.RepeatAllIsSelected;

			splitBtnRepeatAll.Enabled = state.RepeatAllIsEnabled;
			//TODO splitBtnRepeatAll. = state.RepeatAllIsSelected;

			for (int i = 0; i < splitBtnRepeatAll.DropDown.Items.Count - 1; i++)
			{
				var item = splitBtnRepeatAll.DropDown.Items[1] as IconMenuItem;
				item.Checked = state.SelectedSnippetIndex != null && state.SelectedSnippetIndex.Value == i;
			}

			splitBtnRepeatAll.DropDown.Items[splitBtnRepeatAll.DropDown.Items.Count - 1].Enabled = state.StopPlayingEnabled;

			if (state.SelectedSnippetIndex != null)
			{
				IconChar iconChar = GetIconChar(state.SelectedSnippetIndex.Value);

				string text = (state.SelectedSnippetIndex.Value + 1).ToString();

				splitBtnRepeatAll.IconChar = iconChar;
				splitBtnRepeatAll.Text = text;
				btnRemoveCurrentSnippet.IconChar = iconChar;
			}
			else
			{
				splitBtnRepeatAll.IconChar = IconChar.SyncAlt;
				splitBtnRepeatAll.Text = "Repeat All Snippets";

				btnRemoveCurrentSnippet.IconChar = IconChar.None;
			}

			splitBtnRepeatAll.DisplayStyle = splitBtnRepeatAll.IconChar == IconChar.None
				? ToolStripItemDisplayStyle.Text
				: ToolStripItemDisplayStyle.Image;

			dropDownEditSnippet.Enabled = state.DeleteAllIsEnabled || state.DeleteCurrentIsEnabled;
			btnRemoveAllSnippets.Enabled = state.DeleteAllIsEnabled;
			btnRemoveCurrentSnippet.Enabled = state.DeleteCurrentIsEnabled;
		}

		public bool IsExpectedError(Exception ex)
		{
			//Devoce Lost happens for example when user locks his computer while partitura is playing.
			return ex is SlimDXException slimEx && slimEx.ResultCode.Name == "D3DERR_DEVICELOST";
		}

		private IconChar GetIconChar(int i)
		{
			switch (i)
			{
				case 0: return IconChar.DiceOne;
				case 1: return IconChar.DiceTwo;
				case 2: return IconChar.DiceThree;
				case 3: return IconChar.DiceFour;
				case 4: return IconChar.DiceFive;
				case 5: return IconChar.DiceSix;
				default: return IconChar.None;
			}
		}

		private void CreatePartButtons(string partPattern)
		{
			string[] partStrings = new string[partPattern.Length];
			for (int i = 0; i < partPattern.Length; i++) partStrings[i] = partPattern[i].ToString();
			int partIndex = 0;
			foreach (var partString in partStrings)
			{
				var button = new IconToolStripButton
				{
					Name = partString,
					Text = partString,
					ToolTipText = $"Go to Part {partString}",
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

				int index = partIndex;
				button.Click += (sender, args) => { _playerController.GoToPart(index); };

				dropdownAllParts.DropDown.Items.Add(button);
				partIndex++;
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
			_playerController.GoToFirstSnippet();
		}

		private void btnRemoveAllSnippets_Click(object sender, EventArgs e)
		{
			_playerController.RemoveAllSnippets();
		}

		private void btnRemoveCurrentSnippet_Click(object sender, EventArgs e)
		{
			_playerController.RemoveCurrentSnippet();
		}
	}
}