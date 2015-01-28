using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PartituraMagica
{
    public class FakeFileSystem : IFileSystem
    {
        public SortedList<string, List<string>> matchingFileNames = new SortedList<string, List<string>>();

        public virtual ICollection<string> GetMatchingFileNames(string path, string pattern)
        {
            return GetNames(path);
        }

        public ICollection<string> GetNames(string path)
        {
            if (!this.matchingFileNames.ContainsKey(path))
            {
                this.matchingFileNames.Add(path, new List<string>());
            }
            return this.matchingFileNames[path];
        }

        public virtual string ShowOpenDialog(FileType type)
        {
            throw new NotImplementedException();
        }

        public virtual string ShowSaveDialog(FileType type)
        {
            throw new NotImplementedException();
        }

        public virtual DialogResult AskToSave()
        {
            throw new NotImplementedException();
        }

        public virtual void Copy(string from, string to)
        {
            string path = System.IO.Path.GetDirectoryName(to);
            this.GetNames(path).Add(to);
        }

        public virtual System.IO.Stream OpenRead(string path)
        {
            throw new NotImplementedException();
        }

        public virtual System.IO.Stream OpenWrite(string path)
        {
            throw new NotImplementedException();
        }
    }
}
