#region usings

using System;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.Lib.Domain
{
	/** 
     * represents a view that can display (and scroll) a continuous
     * "stream" of musical lines (clippings)
     */
	public interface IPlayerView : IDisposable
	{
		void Close();
		void StartScrolling(IPlayedPartitura partitura, IPlayerController playerMemento);

		void MarkActivePart(int index);
		void MarkPaused(bool isPaused);
	}
}