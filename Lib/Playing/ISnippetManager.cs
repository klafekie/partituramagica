namespace PartituraMagica.Lib.Playing
{
	public interface ISnippetManager
	{
		bool HasChanges { get; }
		void Finish();
		void StartRecording();
		void StopRecording();
		void RepeatAllSnippets();

		SnippetMemento ToMemento();
		void Init(SnippetMemento memento);
	}
}