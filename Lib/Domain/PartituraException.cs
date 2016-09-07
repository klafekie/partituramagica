using System;

namespace PartituraMagica
{
    public class PartituraException : Exception
    {
        public PartituraException(string message) : base(message)
        {
        }
    }
}