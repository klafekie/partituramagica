#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PartituraMagica.Domain;
using PartituraMagica.Playing;
using PartituraMagica.Properties;

#endregion

namespace PartituraMagica.Player
{
	/**
     * Uses Direct3D to implement a smooth scrolling view.
     * Contains custom controls for navigating, zooming, speed control
     */
	public partial class PlayerViewD3D : Form, IPlayerView
	{
		private const int PartAButtonIndex = 2;

		private readonly IPartNavigator _navigator;
		private readonly SpeedManager _speedManager;
		private readonly ZoomManager _zoomManager;
		private readonly KeyboardMap _keyboardMap;

		private readonly IntPtr _ownerHandle;
		private readonly ScrollingPanel _scrollPanel;

		public PlayerViewD3D(IPageFiles pageFiles, IImageLoader imageLoader, IntPtr ownerHandle, PlayerMemento memento,
			PlayerPositionHolder positionHolder, IPartNavigator partNavigator)
		{
			Trace.WriteLine("ctor of PlayerViewD3D");
			InitializeComponent();
			WindowState = FormWindowState.Maximized;

			_navigator = partNavigator;
			_ownerHandle = ownerHandle;
			_speedManager = new SpeedManager(memento.Speed);
			_zoomManager = new ZoomManager(memento.XZoom, memento.YZoom);
			_scrollPanel = new ScrollingPanel(pageFiles, imageLoader, ownerHandle, panel1.Handle, positionHolder,
				partNavigator, _speedManager, _zoomManager);
			_keyboardMap = new KeyboardMap(_speedManager, _zoomManager, _navigator, this);

			_navigator.CurrentPartIndexChanged += () =>
			{
				for (int i = 0; i < _navigator.PartCount; i++)
				{
					var button = (ToolStripButton) toolStrip1.Items[i + PartAButtonIndex];
					button.Checked = i == _navigator.CurrentPartIndex;
				}
			};
			_speedManager.IsPausedChanged += () =>
			{
				btnPauseResume.Checked = _speedManager.IsPaused;
				btnPauseResume.Image = _speedManager.IsPaused
					? Resources.play_button
					: Resources.pause;
			};
		}

		public void Init(string partPattern, IEnumerable<Clipping> clips, double lineDistance)
		{
			_scrollPanel.Init(partPattern, clips, lineDistance);

			CreatePartButtons(partPattern);
		}

		public void StartScrolling()
		{
			IWin32Window w = new Win32Window(_ownerHandle);

			_speedManager.IsPaused = false;

			Enabled = true;
			Show(w);

			_scrollPanel.StartScrolling();

			//we return here only if window is closed
			Enabled = false;
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

		private void CreatePartButtons(string partPattern)
		{
			string[] partStrings = new string[partPattern.Length];
			for (int i = 0; i < partPattern.Length; i++) partStrings[i] = partPattern[i].ToString();
			int partIndex = partStrings.Length - 1;
			foreach (var partString in partStrings.Reverse())
			{
				var button = new ToolStripButton
				{
					DisplayStyle = ToolStripItemDisplayStyle.Text,
					Font = new Font("Segoe UI", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0),
					ImageTransparentColor = Color.Magenta,
					Name = partString,
					Size = new Size(49, 51),
					Text = partString
				};
				toolStrip1.Items.Insert(PartAButtonIndex, button);

				int index = partIndex;
				button.Click += (sender, args) => { _navigator.PartClicked(index); };
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
		}

		private void btnPreviousPart_Click(object sender, EventArgs e)
		{
			_navigator.PreviousPartClicked();
		}

		private void btnNextPart_Click(object sender, EventArgs e)
		{
			_navigator.NextPartClicked();
		}

		private void btnLastPart_Click(object sender, EventArgs e)
		{
			_navigator.LastPartClicked();
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

		private class Win32Window : IWin32Window
		{
			public Win32Window(IntPtr w)
			{
				Handle = w;
			}

			public IntPtr Handle { get; }
		}
	}
}