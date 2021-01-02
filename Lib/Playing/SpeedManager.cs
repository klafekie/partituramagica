#region usings

using System;

#endregion

namespace PartituraMagica.Playing
{
	public class SpeedManager
	{
		private bool _isPaused = true;
		private DateTime? _resumePlayingTime;

		public bool IsPaused
		{
			get
			{
				if (_resumePlayingTime != null && _resumePlayingTime <= DateTime.Now) SetIsPausedAndTriggerEvent(false);

				return _isPaused;
			}
			set => SetIsPausedAndTriggerEvent(value);
		}

		public float Speed { get; set; }

		private void SetIsPausedAndTriggerEvent(bool value)
		{
			bool isChanged = _isPaused != value;
			_resumePlayingTime = null; //abort any timed pause.
			_isPaused = value;
			if (isChanged) IsPausedChanged?.Invoke();
		}

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

		public void PauseFor(int seconds)
		{
			IsPaused = true;
			_resumePlayingTime = DateTime.Now.AddSeconds(seconds);
		}
	}
}