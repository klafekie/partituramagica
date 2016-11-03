using System;
using System.Collections.Generic;
using System.IO;

namespace PartituraMagica
{
    public class FakeFileSystem : IFileSystem
    {
        public SortedList<string, List<string>> MatchingFileNames = new SortedList<string, List<string>>();

        public virtual ICollection<string> GetMatchingFileNames(string path, string pattern)
        {
            return GetNames(path);
        }

        public virtual ICollection<string> ShowOpenImagesDialog()
        {
            return null;
        }

        public virtual string ShowOpenPartituraDialog()
        {
            return null;
        }

        public virtual string ShowSavePartituraDialog(bool dummy)
        {
            return null;
        }

        public virtual DialogResult AskToSave()
        {
            throw new NotImplementedException();
        }

        public virtual void Copy(string from, string to)
        {
            string path = Path.GetDirectoryName(to);
            GetNames(path).Add(to);
        }

        public virtual Stream OpenRead(string path)
        {
            throw new NotImplementedException();
        }

        public virtual Stream OpenWrite(string path)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string path)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetNames(string path)
        {
            if (!MatchingFileNames.ContainsKey(path))
            {
                MatchingFileNames.Add(path, new List<string>());
            }
            return MatchingFileNames[path];
        }
    }
}