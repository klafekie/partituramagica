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
			//TODO confirm from user
			_snippets.Clear();
			HasChanges = true;
			Finish();
		}

		public void RemoveCurrentSnippet()
		{
			int? index = GetCurrentPlayingSnippetIndex();
			if (index == null) return;

			//TODO confirm from user
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


		public void RepeatAllSnippets()
		{
			if (_snippets.Count == 0 || _recordingStartPos != null) return;

			if (_snippets.Count == 1) RepeatSnippet(0);

			Finish();
			PlaySnippets(3, null);
			UpdateSnippetUi();
		}

		public void RepeatSnippet(int snippetIndex)
		{
			if (_snippets.Count <= snippetIndex || _recordingStartPos != null) return;

			Finish();
			PlaySnippets(10, snippetIndex);
			UpdateSnippetUi();
		}

		private void UpdateSnippetUi()
		{
			_playerView.UpdateSnippetUi(new SnippedUiState
			{
				RecordIsEnabled = _playingSnippetIndex == null,
				RecordIsSelected = _recordingStartPos != null,
				RepeatAllIsEnabled = _recordingStartPos == null && _snippets.Any(),
				RepeatAllIsSelected = _playingSnippetIndex != null,
				SelectedSnippetIndex = GetCurrentPlayingSnippetIndex(),
				DeleteCurrentIsEnabled = _playingSnippetIndex != null,
				DeleteAllIsEnabled = _recordingStartPos == null && _snippets.Any()
			});
		}

		private int? GetCurrentPlayingSnippetIndex()
		{
			if (_playingSnippetIndex == null || _playingSnippets == null) return null;
			return _playingSnippets[_playingSnippetIndex.Value].Index;
		}


		private void PlaySnippets(int repeatCount, int? selectedIndex)
		{
			_playingSnippets = new List<Snippet>();

			if (selectedIndex == null)
			{
				int i = 0;
				foreach (var snippet in _snippets)
				{
					EspandSnippet(snippet.Key, snippet.Value, i);
					i++;
				}
			}
			else
			{
				EspandSnippet(_snippets.Keys[selectedIndex.Value], _snippets.Values[selectedIndex.Value], selectedIndex.Value);
			}


			_playingSnippetIndex = 0;
			_positionHolder.XPos = _playingSnippets.First().Start;
			_speedManager.IsPaused = true;
			_playerView.ExecuteDelayed(1500, () => { _speedManager.IsPaused = false; });

			void EspandSnippet(double start, double end, int i)
			{
				for (int j = 0; j < repeatCount; j++)
					_playingSnippets.Add(new Snippet
					{
						Start = start,
						End = end,
						Index = i
					});
			}
		}

		private void ViewHasScrolled()
		{
			if (_playingSnippetIndex == null || _playingSnippets.Count <= _playingSnippetIndex.Value) return;

			//todo mark corresponding button in repeat menu

			if (_positionHolder.XPos >= _playingSnippets[_playingSnippetIndex.Value].End)
			{
				if (_playingSnippets.Count > _playingSnippetIndex.Value + 1)
				{
					_speedManager.IsPaused = true;
					UpdateSnippetUi();
					_playerView.ExecuteDelayed(1500, () =>
					{
						if (_playingSnippetIndex == null)
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