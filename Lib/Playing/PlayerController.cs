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
		private readonly IEditedPartitura _editedPartitura;
		private readonly IFileSystem _fileSystem;
		private readonly IPartNavigator _navigator;
		private readonly PlayerPositionHolder _positionHolder;
		private readonly ISnippetManager _snippetManager;
		private readonly ISpeedManager _speedManager;
		private readonly IZoomManager _zoomManager;
		private string _partituraRuntimePath;

		public PlayerController(IPartNavigator navigator, ISnippetManager snippetManager, ISpeedManager speedManager,
			IZoomManager zoomManager, PlayerPositionHolder positionHolder, IFileSystem fileSystem,
			IEditedPartitura editedPartitura)
		{
			_navigator = navigator;
			_snippetManager = snippetManager;
			_speedManager = speedManager;
			_zoomManager = zoomManager;
			_positionHolder = positionHolder;
			_fileSystem = fileSystem;
			_editedPartitura = editedPartitura;
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

		public void GoToFirstSnippet()
		{
			_snippetManager.GoToFirstSnippet();
		}

		public void GoToSnippet(int i)
		{
			_snippetManager.GoToSnippet(i);
		}

		public void StopPlayingSnippets()
		{
			_snippetManager.GoToEnd();
		}

		public void GoToPreviousSnippet()
		{
			_snippetManager.GoToPreviousSnippet();
		}

		public void GoToNextSnippet()
		{
			_snippetManager.GoToNextSnippet();
		}

		public void GoToLastSnippet()
		{
			_snippetManager.GoToLastSnippet();
		}

		public void RemoveAllSnippets()
		{
			_snippetManager.RemoveAllSnippets();
		}

		public void RemoveCurrentSnippet()
		{
			_snippetManager.RemoveCurrentSnippet();
		}

		public void Pause()
		{
			_speedManager.IsPaused = true;
		}

		public void PlayerClosed()
		{
			_snippetManager.Finish();

			PlayerMemento memento = new PlayerMemento();
			_speedManager.ToMemento(memento);
			_zoomManager.ToMemento(memento);

			SavePlayer(memento, _partituraRuntimePath);

			if (_snippetManager.HasChanges)
				_editedPartitura.Partitura.Snippets = _snippetManager.ToMemento();
		}

		public void PlayerShown()
		{
			_positionHolder.XPos = 0;
			_speedManager.IsPaused = false;
			_navigator.FirstPartClicked();
		}

		public bool IsPartituraFinished()
		{
			return _navigator.IsPartituraFinished();
		}

		public void ScrollForward()
		{
			//IDEA: merge speedmanager and positionHolder?
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
			_partituraRuntimePath = partitura.RuntimePath;

			_navigator.Init(partitura, playerView);

			var memento = LoadPlayer(_partituraRuntimePath);
			_zoomManager.Init(memento);
			_speedManager.Init(memento, playerView);

			_snippetManager.Init(_editedPartitura.Partitura.Snippets, playerView);
		}

		private PlayerMemento LoadPlayer(string partituraRuntimePath)
		{
			if (!_fileSystem.FileExists(partituraRuntimePath)) return new PlayerMemento();

			return JsonSerializer.Deserialize<PlayerMemento>(
				_fileSystem.ReadFile(partituraRuntimePath));
		}

		private void SavePlayer(PlayerMemento memento, string runtimePath)
		{
			var aString = JsonSerializer.Serialize(memento);
			_fileSystem.WriteFile(runtimePath, aString);
		}
	}
}