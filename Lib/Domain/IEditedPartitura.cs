namespace PartituraMagica.Domain
{
	/** 
     * Represents a partitura currently open in the application for editing.
     * At most one active partitura exists at
     * a given time.
     */
	public interface IEditedPartitura
	{
		string Path { get; }

		Partitura Partitura { get; }

		bool IsOpen { get; }

		int CurrentPageIndex { get; set; }

		string CurrentImagePath { get; }

		Page CurrentPage { get; }
		string GetImagePath(int index);

		int GetPageCount();
		void Open(string path, Partitura partitura);
		void Save(string path);
		void Close();
	}
}