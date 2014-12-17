using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Diagnostics;
using Microsoft.Win32;

namespace PartituraMagica
{
	class FileSystem : IFileSystem
	{
		public ICollection<string> GetMatchingFileNames(string path, string pattern) {
			return System.IO.Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);
		}

		public string ShowOpenDialog(FileType type) {
			string result;
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Multiselect = false;

			result = ShowFileDialog(type, dialog);

			return result;
		}

		private static string ShowFileDialog(FileType type, FileDialog dialog) {
			if (type == FileType.Image) {
				dialog.Title = "Open Image File";
				dialog.Filter = "Image files (*.jpg)|*.jpg";
			} else {
				dialog.Title = "Open Partitura File";
				Debug.Assert(type == FileType.Partitura);
				dialog.Filter = "Partitura files (*.partitura)|*.partitura";
			}

			dialog.RestoreDirectory = true;

			if (dialog.ShowDialog() == true) {
				return dialog.FileName;
			}
			return null;
		}


		public string ShowSaveDialog(FileType type) {
			SaveFileDialog dialog = new SaveFileDialog();
			return ShowFileDialog(type, dialog);			
		}

		public DialogResult AskToSave() {
			//dont use System.windows.forms.MessageBox!
			MessageBoxResult r =
				System.Windows.MessageBox.Show("Do you want to save current Partitura?",
					"Save Changes?", System.Windows.MessageBoxButton.YesNoCancel);

			switch (r) {
				case MessageBoxResult.Yes: return DialogResult.Yes;
				case MessageBoxResult.No: return DialogResult.No;
				case MessageBoxResult.Cancel: return DialogResult.Cancel;
				default: throw new ApplicationException("unexpected return value");
			}
		}

		/**
		 * if target file exists, it is always overwritten!
		 */
		public void Copy(string from, string to) {
			string extension1 = Path.GetExtension(from).ToLower();
			string extension2 = Path.GetExtension(to).ToLower();
			Debug.Assert(extension1 == extension2);
			File.Copy(from, to, true);
		}


		public Stream OpenRead(string path) {
			return File.OpenRead(path);
		}

		public Stream OpenWrite(string path) {
			return File.Create(path);
		}
	}
}
