using System;

namespace PartituraMagica
{
    public class SpeedManager
    {
        private bool _isPaused = true;

        public SpeedManager(float initialSpped)
        {
            Speed = initialSpped;
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                bool isChanged = _isPaused != value;
                _isPaused = value;
                if (isChanged)
                {
                    IsPausedChanged?.Invoke();
                }
            }
        }

        public float Speed { get; private set; }

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