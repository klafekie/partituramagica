#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	internal class SnippetManager : ISnippetManager
	{
		private class Snippet
		{
			public double Start { get; set; }
			public double End { get; set; }
			public int Index { get; set; }
		}

		private const double MIN_SNIPPET_LENGTH = 10;
		private readonly PlayerPositionHolder _positionHolder;
		private readonly SortedList<double, double> _snippets = new SortedList<double, double>();
		private readonly ISpeedManager _speedManager;

		private IPlayerView _playerView;

		//TODO naming: this is the index inside the expanded list.
		private int? _playingSnippetIndex;
		private List<Snippet> _playingSnippets;

		private double? _recordingStartPos;

		public SnippetManager(PlayerPositionHolder positionHolder, ISpeedManager speedManager)
		{
			_positionHolder = positionHolder;
			_speedManager = speedManager;
			_positionHolder.ViewHasScrolled += ViewHasScrolled;
		}

		public bool HasChanges { get; set; }

		public void Init(SnippetMemento memento, IPlayerView playerView)
		{
			_playerView = playerView;
			_snippets.Clear();
			_playingSnippetIndex = null;
			_playingSnippets = null;

			if (memento.Snippets != null)
				foreach (var pair in memento.Snippets)
					_snippets.Add(pair.Key, pair.Value);

			playerView.UpdateAvailableSnippets(_snippets.Count);

			UpdateSnippetUi();
		}

		public void RemoveAllSnippets()
		{
			if (_playerView.ShowConfirmDialog("Do you want to delete all snippets?"))
			{
				_snippets.Clear();
				HasChanges = true;
				Finish();
			}
		}

		public void RemoveCurrentSnippet()
		{
			int? index = GetCurrentPlayingSnippet();
			if (index == null || !_playerView.ShowConfirmDialog("Do you want to delete current snippet?"))
				return;

			HasChanges = true;
			_snippets.RemoveAt(index.Value);
			Finish();
		}

		public void Finish()
		{
			_playingSnippets = null;
			_playingSnippetIndex = null;
			StopRecording();
		}

		public void StartRecording()
		{
			if (_recordingStartPos == null && _playingSnippetIndex == null)
				_recordingStartPos = _positionHolder.XPos;
			UpdateSnippetUi();
		}

		public void StopRecording()
		{
			if (_recordingStartPos != null)
			{
				double start = _recordingStartPos.Value;
				double end = _positionHolder.XPos;

				//big enough?
				if (start + MIN_SNIPPET_LENGTH <= end)
				{
					//sortedList allows no duplicates in Key.
					if (_snippets.ContainsKey(start))
						_snippets[start] =
							Math.Max(_snippets[start], end);
					else
						_snippets.Add(start, end);

					HasChanges = true;
					_recordingStartPos = null;
				}
			}

			_playerView.UpdateAvailableSnippets(_snippets.Count);
			UpdateSnippetUi();
		}

		public SnippetMemento ToMemento()
		{
			return new SnippetMemento
			{
				Snippets = _snippets.ToList()
			};
		}

		public void GoToFirstSnippet()
		{
			if (_snippets.Count == 0 || _recordingStartPos != null) return;

			UpdateExpandedSnippets();

			int? currentPlayingSnippet = GetCurrentPlayingSnippet();
			if (currentPlayingSnippet == null) //first click
			{
				JumpToSnippet(0);
			}
			else
			{
				if (currentPlayingSnippet < _snippets.Count - 1) //jump to next snippet
					JumpToSnippet(currentPlayingSnippet.Value + 1);
				else
					//last snipped. go to end of last snippet to facilitate recording more snippets after last snippet.
					GoToEnd();
			}

			UpdateSnippetUi();
		}

		public void GoToLastSnippet()
		{
			GoToSnippet(_snippets.Count - 1);
		}

		public void GoToEnd()
		{
			_playingSnippetIndex = null;
			_positionHolder.XPos = _playingSnippets.Last().End;
			_speedManager.IsPaused = true;
			_playingSnippets = null;
			UpdateSnippetUi();
		}

		public void GoToSnippet(int snippetIndex)
		{
			if (_snippets.Count <= snippetIndex || _recordingStartPos != null) return;

			UpdateExpandedSnippets();

			JumpToSnippet(snippetIndex);

			UpdateSnippetUi();
		}

		public void GoToPreviousSnippet()
		{
			int? index = GetCurrentPlayingSnippet();
			if (index != null && index.Value > 0)
				GoToSnippet(index.Value - 1);
		}

		public void GoToNextSnippet()
		{
			int? index = GetCurrentPlayingSnippet();
			if (index != null && index.Value < _snippets.Count - 1)
				GoToSnippet(index.Value + 1);
		}

		private void UpdateExpandedSnippets()
		{
			_playingSnippets = new List<Snippet>();
			int i = 0;
			foreach (var snippet in _snippets)
			{
				for (int j = 0; j < 3; j++)
					//TODO repeat count configurable
					_playingSnippets.Add(new Snippet
					{
						Start = snippet.Key,
						End = snippet.Value,
						Index = i
					});

				i++;
			}
		}

		private void UpdateSnippetUi()
		{
			_playerView.UpdateSnippetUi(new SnippedUiState
			{
				RecordIsEnabled = _playingSnippetIndex == null,
				RecordIsSelected = _recordingStartPos != null,
				RepeatAllIsEnabled = _recordingStartPos == null && _snippets.Any(),
				RepeatAllIsSelected = _playingSnippetIndex != null,
				StopPlayingEnabled = _playingSnippetIndex != null,
				SelectedSnippetIndex = GetCurrentPlayingSnippet(),
				DeleteCurrentIsEnabled = _playingSnippetIndex != null,
				DeleteAllIsEnabled = _recordingStartPos == null && _snippets.Any()
			});
		}

		private int? GetCurrentPlayingSnippet()
		{
			if (_playingSnippetIndex == null || _playingSnippets == null) return null;
			return _playingSnippets[_playingSnippetIndex.Value].Index;
		}

		private void JumpToSnippet(int selectedIndex)
		{
			int i = 0;
			foreach (Snippet snippet in _playingSnippets)
			{
				if (snippet.Index == selectedIndex)
				{
					_playingSnippetIndex = i;
					_positionHolder.XPos = snippet.Start;
					break;
				}

				i++;
			}


			_speedManager.IsPaused = true;
			_playerView.ExecuteDelayed(1500, () => { _speedManager.IsPaused = false; });
		}

		private void ViewHasScrolled()
		{
			if (_playingSnippetIndex == null || _playingSnippets.Count <= _playingSnippetIndex.Value) return;

			if (_positionHolder.XPos >= _playingSnippets[_playingSnippetIndex.Value].End)
			{
				if (_playingSnippets.Count > _playingSnippetIndex.Value + 1)
				{
					_speedManager.IsPaused = true;
					UpdateSnippetUi();
					_playerView.ExecuteDelayed(1500, () =>
					{
						if (_playingSnippetIndex == null ||
						    _playingSnippetIndex.Value >= _playingSnippets.Count - 1)
							//during the pause snippet playing was paused or player window closed
							return;

						_playingSnippetIndex++;
						_positionHolder.XPos = _playingSnippets[_playingSnippetIndex.Value].Start;
						_speedManager.IsPaused = false;
						UpdateSnippetUi();
					});
				}
				else
				{
					_speedManager.IsPaused = true;
					Finish();
				}
			}
		}
	}
}