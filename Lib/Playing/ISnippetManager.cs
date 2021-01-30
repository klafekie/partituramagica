#region usings

using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	public interface ISnippetManager
	{
		bool HasChanges { get; }

		void Init(SnippetMemento memento, IPlayerView playerView);
		SnippetMemento ToMemento();
		void Finish();

		void StartRecording();
		void StopRecording();

		void GoToFirstSnippet();
		void GoToSnippet(int i);
		void GoToPreviousSnippet();
		void GoToNextSnippet();
		void GoToLastSnippet();
		void GoToEnd();

		void RemoveAllSnippets();
		void RemoveCurrentSnippet();
	}
}