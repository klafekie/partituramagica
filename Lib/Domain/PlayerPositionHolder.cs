#region usings

using System;

#endregion

namespace PartituraMagica.Lib.Domain
{
	/**
	 * helper class to avoid the circular dependency between IPlayerView and PartNavigator
	 */
	public class PlayerPositionHolder
	{
		public double XPos { get; set; }

		/**
         * Event triggered when the view has scrolled forward. 
         * It is NOT fired when the position is changed by navigation.
         * 
         * Take care that event handlers to nothing that takes much time!
         */
		public event Action ViewHasScrolled;

		public void ScrollForwardBy(float pixels)
		{
			XPos += pixels;
			ViewHasScrolled?.Invoke();
		}
	}
}