#region usings

using System;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	internal class SpeedManager : ISpeedManager
	{
		private bool _isPaused = true;
		private IPlayerView _playerView;

		public bool IsPaused
		{
			get => _isPaused;
			set
			{
				bool isChanged = _isPaused != value;
				_isPaused = value;
				if (isChanged) IsPausedChanged?.Invoke();
			}
		}

		public float Speed { get; set; }

		public event Action IsPausedChanged;

		public void IncreaseSpeed()
		{
			Speed *= 1.2F;
		}

		public void TogglePaused()
		{
			IsPaused = !IsPaused;
		}

		public void DecreaseSpeed()
		{
			Speed *= 0.8F;
		}

		public void ToMemento(PlayerMemento memento)
		{
			memento.Speed = Speed;
		}

		public void Init(PlayerMemento memento, IPlayerView playerView)
		{
			_playerView = playerView;
			Speed = memento.Speed;
			IsPausedChanged += () => { playerView.MarkPaused(IsPaused); };
		}
	}
}