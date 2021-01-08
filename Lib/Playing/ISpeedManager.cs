#region usings

using System;
using PartituraMagica.Lib.Domain;

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
		void Init(PlayerMemento memento, IPlayerView playerView);
		void ToMemento(PlayerMemento memento);
	}
}