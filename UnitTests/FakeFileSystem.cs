#region usings

using System;
using System.Collections.Generic;
using System.IO;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Test
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

		public virtual string ShowSavePartituraDialog()
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
			return null;
		}

		public virtual Stream OpenWrite(string path)
		{
			return null;
		}

		public bool FileExists(string path)
		{
			return false;
		}

		public ICollection<string> GetNames(string path)
		{
			if (!MatchingFileNames.ContainsKey(path)) MatchingFileNames.Add(path, new List<string>());
			return MatchingFileNames[path];
		}
	}
}