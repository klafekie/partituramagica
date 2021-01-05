using System;

namespace PartituraMagica
{
    [Serializable]
    public class PlayerMemento
    {
        public PlayerMemento()
        {
            Speed = 1F;
            XZoom = 0.004F;
            YZoom = 0.01F;
        }

        public float Speed { get; set; }
        public float XZoom { get; set; }
        public float YZoom { get; set; }
    }
}