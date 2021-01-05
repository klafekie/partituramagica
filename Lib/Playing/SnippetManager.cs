#region usings

using System.Collections.Generic;
using System.Linq;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	internal class SnippetManager : ISnippetManager
	{
		private readonly PlayerPositionHolder _positionHolder;
		private readonly SortedList<double, double> _snippets = new SortedList<double, double>();
		private readonly ISpeedManager _speedManager;
		private int? _playingSnippetIndex;
		private List<(double Start, double End)> _playingSnippets;

		private double? _recordingStartPos;
		private bool _snippedChangeInProgress;

		public SnippetManager(PlayerPositionHolder positionHolder, ISpeedManager speedManager)
		{
			_positionHolder = positionHolder;
			_speedManager = speedManager;
			_positionHolder.ViewHasScrolled += ViewHasScrolled;
		}

		public bool HasChanges { get; set; }

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
		}

		public void StopRecording()
		{
			if (_recordingStartPos != null)
			{
				_snippets.Add(_recordingStartPos.Value, _positionHolder.XPos);
				HasChanges = true;
				_recordingStartPos = null;
			}
		}

		public void RepeatAllSnippets()
		{
			if (_snippets.Count == 0 || _recordingStartPos != null) return;

			_playingSnippets = new List<(double, double)>();
			foreach (var snippet in _snippets)
				for (int i = 0; i < 3; i++) //TODO make repeat count configurable
					_playingSnippets.Add((snippet.Key, snippet.Value));
			_playingSnippetIndex = 0;

			_positionHolder.XPos = _playingSnippets.First().Start;
			_speedManager.PauseFor(2);
		}

		public SnippetMemento ToMemento()
		{
			return new SnippetMemento
			{
				Snippets = _snippets.ToList()
			};
		}

		public void Init(SnippetMemento memento)
		{
			if (memento.Snippets == null) return;
			foreach (var pair in memento.Snippets) _snippets.Add(pair.Key, pair.Value);
		}

		private void ViewHasScrolled()
		{
			if (_playingSnippetIndex == null || _playingSnippets.Count <= _playingSnippetIndex.Value) return;

			//todo mark corresponding button in repeat menu

			//we must pause at the end and at the beginning of each snippet. 
			//While pausing at the end of the snippet, _snippedChangeInProgress is true.
			//it is reset on first scroll event we receive after the pause.
			if (!_snippedChangeInProgress)
			{
				if (_positionHolder.XPos >= _playingSnippets[_playingSnippetIndex.Value].End)
				{
					if (_playingSnippets.Count > _playingSnippetIndex.Value + 1)
					{
						_snippedChangeInProgress = true;
						_playingSnippetIndex++;
						_speedManager.PauseFor(2);
					}
					else
					{
						_speedManager.IsPaused = true;
						Finish();
					}
				}
			}
			else
			{
				_snippedChangeInProgress = false;
				if (_playingSnippets.Count > _playingSnippetIndex.Value)
				{
					_positionHolder.XPos = _playingSnippets[_playingSnippetIndex.Value].Start;
					_speedManager.PauseFor(2);
				}
			}
		}
	}
}