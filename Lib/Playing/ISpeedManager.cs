#region usings

using System;

#endregion

namespace PartituraMagica.Lib.Playing
{
	internal interface ISpeedManager
	{
		bool IsPaused { get; set; }
		float Speed { get; set; }
		event Action IsPausedChanged;
		void IncreaseSpeed();
		void TogglePaused();
		void DecreaseSpeed();
		void PauseFor(int seconds);
		void Init(PlayerMemento memento);
		void ToMemento(PlayerMemento memento);
	}
}