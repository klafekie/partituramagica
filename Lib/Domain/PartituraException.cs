using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{
    public class PartituraException : Exception
    {
        public PartituraException(string message) : base(message) { }
    }
}
