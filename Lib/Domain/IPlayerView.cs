#region usings

using System;
using System.Collections.Generic;

#endregion

namespace PartituraMagica.Domain
{
	/** 
     * represents a view that can display (and scroll) a continuous
     * "stream" of musical lines (clippings)
     */
	public interface IPlayerView : IDisposable
	{
		/** 
         * Initialize the views graphical objects using the given clippings
         */
		void Init(string partPattern, IEnumerable<Clipping> clippings, double lineDistance);

		void StartScrolling();

		PlayerMemento ToMemento();

		void Close();
	}
}