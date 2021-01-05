#region usings

#endregion

#region usings

using System.Collections.Generic;
using System.Text.Json;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	internal class PlayerController : IPlayerController
	{
		private readonly IFileSystem _fileSystem;
		private readonly IPartNavigator _navigator;
		private readonly PlayerPositionHolder _positionHolder;
		private readonly ISnippetManager _snippetManager;
		private readonly ISpeedManager _speedManager;
		private readonly IZoomManager _zoomManager;
		private IPlayerView _playerView;

		public PlayerController(IPartNavigator navigator, ISnippetManager snippetManager, ISpeedManager speedManager,
			IZoomManager zoomManager, PlayerPositionHolder positionHolder, IFileSystem fileSystem)
		{
			_navigator = navigator;
			_snippetManager = snippetManager;
			_speedManager = speedManager;
			_zoomManager = zoomManager;
			_positionHolder = positionHolder;
			_fileSystem = fileSystem;
		}

		public void GoToPart(int i)
		{
			_navigator.PartClicked(i);
			_snippetManager.Finish();
		}

		public void GoToFirstPart()
		{
			_navigator.FirstPartClicked();
			_snippetManager.Finish();
		}

		public void GoToPreviousPart()
		{
			_navigator.PreviousPartClicked();
			_snippetManager.Finish();
		}

		public double XPos => _positionHolder.XPos;

		public void GoToNextPart()
		{
			_navigator.NextPartClicked();
			_snippetManager.Finish();
		}

		public void GoToLastPart()
		{
			_navigator.LastPartClicked();
			_snippetManager.Finish();
		}

		public void JumpBackward()
		{
			_navigator.JumpBackwardClicked();
		}

		public void TogglePause()
		{
			_speedManager.TogglePaused();
		}

		public void JumpForward()
		{
			_navigator.JumpForwardClicked();
		}

		public void DecreaseSpeed()
		{
			_speedManager.DecreaseSpeed();
		}

		public void IncreaseSpeed()
		{
			_speedManager.IncreaseSpeed();
		}

		public void Resume()
		{
			_speedManager.IsPaused = false;
		}

		public void ZoomIn(bool xOnly)
		{
			_zoomManager.ZoomIn(xOnly);
		}

		public void ZoomOut(bool xOnly)
		{
			_zoomManager.ZoomOut(xOnly);
		}

		public void GetNextZoom(out float zoomX, out float zoomY)
		{
			_zoomManager.GetNextZoom(out zoomX, out zoomY);
		}

		public void StartSnippetRecording()
		{
			_snippetManager.StartRecording();
		}

		public void StopSnippetRecording()
		{
			_snippetManager.StopRecording();
		}

		public void RepeatAllSnippets()
		{
			_snippetManager.RepeatAllSnippets();
		}

		public void Pause()
		{
			_speedManager.IsPaused = true;
		}

		public void PlayerClosed()
		{
			_snippetManager.Finish();
		}

		public void PlayerShown()
		{
			_positionHolder.XPos = 0;
			_speedManager.IsPaused = false;
		}

		public bool IsPartituraFinished()
		{
			return _navigator.IsPartituraFinished();
		}

		public void ScrollForward()
		{
			if (_speedManager.IsPaused) return;

			if (!_navigator.IsPartituraFinished())
				_positionHolder.ScrollForwardBy(_speedManager.Speed);
		}

		public IList<Clipping> GetClippings()
		{
			return _navigator.GetClippings();
		}

		public void Init(IPlayedPartitura partitura, IPlayerView playerView)
		{
			_playerView = playerView;

			_navigator.LoadPartitura(partitura);

			var memento = LoadPlayer(partitura);
			_zoomManager.XZoom = memento.XZoom;
			_zoomManager.YZoom = memento.YZoom;
			_speedManager.Speed = memento.Speed;

			_playerView.Closed += (o, args) => { SavePlayer(_playerView, partitura.RuntimePath); };

			_playerView.MarkActivePart(0);
			_navigator.CurrentPartIndexChanged += () => _playerView.MarkActivePart(_navigator.CurrentPartIndex);

			_speedManager.IsPausedChanged += () => { _playerView.MarkPaused(_speedManager.IsPaused); };
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

		private PlayerMemento LoadPlayer(IPlayedPartitura partitura)
		{
			if (!_fileSystem.FileExists(partitura.RuntimePath)) return new PlayerMemento();

			return JsonSerializer.Deserialize<PlayerMemento>(
				_fileSystem.ReadFile(partitura.RuntimePath));
		}

		private void SavePlayer(IPlayerView view, string runtimePath)
		{
			var aString = JsonSerializer.Serialize(ToMemento());
			_fileSystem.WriteFile(runtimePath, aString);
		}
	}
}