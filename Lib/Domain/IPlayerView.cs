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
		void ExecuteDelayed(int milliseconds, Action action);

		void MarkActivePart(int index);
		void MarkPaused(bool isPaused);

		void UpdateAvailableSnippets(int snippetsCount);
		void UpdateSnippetUi(SnippedUiState state);

		bool IsExpectedError(Exception ex);
		void Init();
	}
}