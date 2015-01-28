using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{

    /** 
     * Gives read only access to a Partitura object in order to play it.
     */
    public interface IPlayablePartitura
    {
        IList<Part> GetAllParts(int pageCount);

        String PartPattern { get; }
    }
}
