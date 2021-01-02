#region usings

using System;

#endregion

namespace PartituraMagica.Playing
{
	public class SpeedManager
	{
		private bool _isPaused = true;

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
	}
}