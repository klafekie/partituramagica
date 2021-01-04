#region usings

#endregion

#region usings

using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	public interface IPlayerViewFactory
	{
		/**
		 * Creates player for partitura and starts scrolling.
		 * This method call blocks until the player vindow is closed.
		 */
		void CreateAndShowView(IPlayedPartitura partitura);
	}
}