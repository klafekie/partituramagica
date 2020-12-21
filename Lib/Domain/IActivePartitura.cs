namespace PartituraMagica.Domain
{
	/** 
     * Represents a partitura currently open in the application.
     * Active Partitura knows how to associate image files with 
     * a partitura. At most one active partitura exists at
     * a given time.
     */
	public interface IActivePartitura : IPageFiles
	{
		string Path { get; }

		Partitura Partitura { get; }

		bool IsOpen { get; }

		int CurrentPageIndex { get; set; }

		string CurrentImagePath { get; }

		Page CurrentPage { get; }

		void Open(string path, Partitura partitura);
		void Save(string path);
		void Close();
	}
}