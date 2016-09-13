using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace PartituraMagica
{
    internal class FileSystem : IFileSystem
    {
        public ICollection<string> GetMatchingFileNames(string path, string pattern)
        {
            return Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);
        }

        public ICollection<string> ShowOpenImagesDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Open Image File(s)",
                Filter = "Image files (*.jpg)|*.jpg",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileNames;
            }
            return new string[0];
        }

        public string ShowOpenPartituraDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = "Open Partitura File",
                Filter = "Partitura files (*.partitura)|*.partitura",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            return null;
        }

        public string ShowSavePartituraDialog(bool dummy)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Save Partitura File",
                Filter = "Partitura files (*.partitura)|*.partitura",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            return null;
        }

        public DialogResult AskToSave()
        {
            //dont use System.windows.forms.MessageBox!
            MessageBoxResult r =
                MessageBox.Show("Do you want to save current Partitura?",
                    "Save Changes?", MessageBoxButton.YesNoCancel);

            switch (r)
            {
                case MessageBoxResult.Yes:
                    return DialogResult.Yes;
                case MessageBoxResult.No:
                    return DialogResult.No;
                case MessageBoxResult.Cancel:
                    return DialogResult.Cancel;
                default:
                    throw new ApplicationException("unexpected return value");
            }
        }

        /**
         * if target file exists, it is always overwritten!
         */

        public void Copy(string from, string to)
        {
            string extension1 = Path.GetExtension(from).ToLower();
            string extension2 = Path.GetExtension(to).ToLower();
            Debug.Assert(extension1 == extension2);
            File.Copy(from, to, true);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public Stream OpenWrite(string path)
        {
            return File.Create(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}