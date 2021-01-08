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

		void RepeatAllSnippets();
		void RepeatSnippet(int i);

		void RemoveAllSnippets();
		void RemoveCurrentSnippet();
	}
}