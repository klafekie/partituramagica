#region usings

using System.Collections.Generic;

#endregion

namespace PartituraMagica.Lib.Domain
{
	/**
	 * the partitura currently selected for playing
	 */
	public interface IPlayedPartitura
	{
		string PartPattern { get; }
		string RuntimePath { get; }
		IList<Part> AllParts { get; }
		List<string> ImagePaths { get; }
	}
}