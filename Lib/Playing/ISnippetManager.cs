namespace PartituraMagica.Lib.Playing
{
	internal interface ISnippetManager
	{
		void Finish();
		void StartRecording();
		void StopRecording();
		void RepeatAllSnippets();
	}
}