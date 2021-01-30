#region usings

using System.Collections.Generic;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	public interface IPlayerController
	{
		double XPos { get; }

		void Init(IPlayedPartitura partitura, IPlayerView view);

		void PlayerClosed();
		void PlayerShown();

		IList<Clipping> GetClippings();

		void GoToPart(int i);
		void GoToFirstPart();
		void GoToPreviousPart();
		void GoToNextPart();
		void GoToLastPart();

		void JumpBackward();
		void JumpForward();

		bool IsPartituraFinished();

		void DecreaseSpeed();
		void IncreaseSpeed();
		void TogglePause();
		void ScrollForward();
		void Pause();
		void Resume();

		void ZoomIn(bool xOnly);
		void ZoomOut(bool xOnly);
		void GetNextZoom(out float zoomX, out float zoomY);

		void StartSnippetRecording();
		void StopSnippetRecording();

		void GoToFirstSnippet();
		void GoToSnippet(int i);
		void GoToPreviousSnippet();
		void GoToNextSnippet();
		void GoToLastSnippet();
		void StopPlayingSnippets();

		void RemoveAllSnippets();
		void RemoveCurrentSnippet();
	}
}