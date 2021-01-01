#region usings

using System;

#endregion

namespace PartituraMagica.Domain
{
	/** 
     * represents a view that can display (and scroll) a continuous
     * "stream" of musical lines (clippings)
     */
	public interface IPlayerView : IDisposable
	{
		PlayerMemento ToMemento();

		void Close();
	}
}