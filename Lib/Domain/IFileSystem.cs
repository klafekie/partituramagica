#region usings

using System.Collections.Generic;
using System.IO;

#endregion

namespace PartituraMagica.Domain
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
		string ShowSavePartituraDialog(bool dummy = false);

		/**
		 * Do you want to save current Partitura? yes no cancel
		 */
		DialogResult AskToSave();

		/** 
         * to and from should be abs. paths and have the same file extension!
         */
		void Copy(string from, string to);

		Stream OpenRead(string path);

		/**
		 * file is created if it doesn't exist
		 */
		Stream OpenWrite(string path);

		bool FileExists(string path);
	}
}