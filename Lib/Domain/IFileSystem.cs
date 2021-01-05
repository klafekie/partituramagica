#region usings

using System.Collections.Generic;

#endregion

namespace PartituraMagica.Lib.Domain
{
	public enum DialogResult
	{
		Yes,
		No,
		Cancel
	}

	/**
   * offers an abstraction from standard filesystem operations and dialogs
   */
	public interface IFileSystem
	{
		/** return all files in the directory path that match pattern.
     * returns an empty collection (not null) if nothing matches.
     */
		ICollection<string> GetMatchingFileNames(string path, string pattern);

		/**
     * @return selected files (full path), empty collection if cancel is pressed
     */
		ICollection<string> ShowOpenImagesDialog();

		string ShowOpenPartituraDialog();

		/**
     * @return chosen file (full path), null on cancel
     */
		string ShowSavePartituraDialog();

		/**
		 * Do you want to save current Partitura? yes no cancel
		 */
		DialogResult AskToSave();

		/** 
     * to and from should be abs. paths and have the same file extension!
     */
		void Copy(string from, string to);

		string ReadFile(string path);

		/**
		 * file is created if it doesn't exist
		 */
		void WriteFile(string path, string aString);

		bool FileExists(string path);
	}
}