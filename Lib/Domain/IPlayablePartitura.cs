#region usings

using System.Collections.Generic;

#endregion

namespace PartituraMagica.Domain
{
	/** 
                             * Gives read only access to a Partitura object in order to play it.
                             */
	public interface IPlayablePartitura
	{
		string PartPattern { get; }
		IList<Part> GetAllParts(int pageCount);
	}
}