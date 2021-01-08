namespace PartituraMagica.Lib.Domain
{
	public class SnippedUiState
	{
		public bool RecordIsEnabled { get; set; }
		public bool RecordIsSelected { get; set; }

		public bool RepeatAllIsEnabled { get; set; }
		public bool RepeatAllIsSelected { get; set; }

		public int? SelectedSnippetIndex { get; set; }

		public bool DeleteCurrentIsEnabled { get; set; }
		public bool DeleteAllIsEnabled { get; set; }
	}
}