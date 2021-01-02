#region usings

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using FontAwesome.Sharp;
using PartituraMagica.Domain;
using PartituraMagica.Playing;
using IWin32Window = System.Windows.Forms.IWin32Window;

#endregion

namespace PartituraMagica.Player
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

		private readonly KeyboardMap _keyboardMap;

		private readonly IPartNavigator _navigator;

		private readonly IntPtr _ownerHandle;
		private readonly ScrollingPanel _scrollPanel;
		private readonly SnippetManager _snippets;
		private readonly SpeedManager _speedManager;
		private readonly ZoomManager _zoomManager;

		public PlayerViewD3D(IImageLoader imageLoader, MainWindow mainWindow,
			PlayerPositionHolder positionHolder, IPartNavigator partNavigator)
		{
			Trace.WriteLine("ctor of PlayerViewD3D");
			InitializeComponent();
			WindowState = FormWindowState.Maximized;

			_navigator = partNavigator;
			_ownerHandle = new WindowInteropHelper(mainWindow).Handle;
			_speedManager = new SpeedManager();
			_zoomManager = new ZoomManager();
			_snippets = new SnippetManager(positionHolder, _speedManager);
			_scrollPanel = new ScrollingPanel(imageLoader, panel1.Handle, positionHolder,
				partNavigator, _speedManager, _zoomManager);
			_keyboardMap = new KeyboardMap(_speedManager, _zoomManager, _navigator, this);

			_speedManager.IsPausedChanged += () =>
			{
				btnPauseResume.Checked = _speedManager.IsPaused;
				btnPauseResume.IconChar = _speedManager.IsPaused
					? IconChar.Play
					: IconChar.Pause;
			};
		}

		public PlayerMemento ToMemento()
		{
			return new PlayerMemento
			{
				Speed = _speedManager.Speed,
				XZoom = _zoomManager.XZoom,
				YZoom = _zoomManager.YZoom
			};
		}

		public void StartScrolling(IPlayedPartitura partitura, PlayerMemento memento)
		{
			CreatePartButtons(partitura.PartPattern);

			_navigator.LoadPartitura(partitura);
			CheckCurrentPartButton(partitura);
			_navigator.CurrentPartIndexChanged += () => CheckCurrentPartButton(partitura);

			_zoomManager.XZoom = memento.XZoom;
			_zoomManager.YZoom = memento.YZoom;
			_speedManager.Speed = memento.Speed;

			IWin32Window w = new Win32Window(_ownerHandle);

			_speedManager.IsPaused = false;

			Enabled = true;
			Show(w);

			_scrollPanel.StartScrolling(partitura);

			_snippets.StopRecording(); //make sure that currently recording snippet is stored before leave
			_snippets.CancelPlaying();

			//we return here only if window is closed
			Enabled = false;
		}

		private void CheckCurrentPartButton(IPlayedPartitura partitura)
		{
			if (toolStrip1.Items.Count < partitura.PartPattern.Length + PART_A_BUTTON_INDEX)
				//this check is required. When you play to second part, then close and reopen player the Items list is empty sometimes.
				return;

			for (int i = 0; i < partitura.PartPattern.Length; i++)
			{
				var button = (ToolStripButton) toolStrip1.Items[i + PART_A_BUTTON_INDEX];
				button.Checked = i == _navigator.CurrentPartIndex;
			}
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
				button.Click += (sender, args) =>
				{
					//TODO this needs to be done also in keyboard map!
					//Also stopRecording
					_snippets.CancelPlaying();
					_navigator.PartClicked(index);
				};
				partIndex--;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) components?.Dispose();
			base.Dispose(disposing);

			_scrollPanel?.Dispose();
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
			_navigator.FirstPartClicked();
			_snippets.CancelPlaying();
		}

		private void btnPreviousPart_Click(object sender, EventArgs e)
		{
			_navigator.PreviousPartClicked();
			_snippets.CancelPlaying();
		}

		private void btnNextPart_Click(object sender, EventArgs e)
		{
			_navigator.NextPartClicked();
			_snippets.CancelPlaying();
		}

		private void btnLastPart_Click(object sender, EventArgs e)
		{
			_navigator.LastPartClicked();
			_snippets.CancelPlaying();
		}

		private void btnJumpBackward_Click(object sender, EventArgs e)
		{
			_navigator.JumpBackwardClicked();
		}

		private void btnPauseResume_Click(object sender, EventArgs e)
		{
			_speedManager.TogglePaused();
		}

		private void btnJumpForward_Click(object sender, EventArgs e)
		{
			_navigator.JumpForwardClicked();
		}

		private void btnSlower_Click(object sender, EventArgs e)
		{
			_speedManager.DecreaseSpeed();
		}

		private void btnFaster_Click(object sender, EventArgs e)
		{
			_speedManager.IncreaseSpeed();
		}

		private void btnZoomIn_Click(object sender, EventArgs e)
		{
			_zoomManager.ZoomIn(false);
		}

		private void btnWider_Click(object sender, EventArgs e)
		{
			_zoomManager.ZoomIn(true);
		}

		private void btnZoomOut_Click(object sender, EventArgs e)
		{
			_zoomManager.ZoomOut(false);
		}

		private void btnSmaller_Click(object sender, EventArgs e)
		{
			_zoomManager.ZoomOut(true);
		}

		private void btnRemoveCurrentSnippet_Click(object sender, EventArgs e)
		{
		}

		private void btnRecordSnippet_MouseDown(object sender, MouseEventArgs e)
		{
			_snippets.StartRecording();
		}

		private void btnRecordSnippet_MouseUp(object sender, MouseEventArgs e)
		{
			_snippets.StopRecording();
		}

		private void splitBtnRepeatAll_Click(object sender, EventArgs e)
		{
			_snippets.RepeatAllSnippets();
		}
	}
}