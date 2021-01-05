#region usings

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

#endregion

namespace PartituraMagica.Lib.Domain
{
	/**
	 * <see cref="IEditedPartitura" />
	 */
	public class EditedPartitura : IEditedPartitura
	{
		public class PlayedPartitura : IPlayedPartitura
		{
			public PlayedPartitura(EditedPartitura editedPartitura)
			{
				PartPattern = editedPartitura.Partitura?.PartPattern?.Trim() ?? "A";
				RuntimePath = editedPartitura.Path + ".runtime";

				ImagePaths = new List<string>();
				int pageCount = editedPartitura.GetPageCount();
				for (int i = 0; i < pageCount; i++)
					ImagePaths.Add(editedPartitura.GetImagePath(i));

				AllParts = editedPartitura.Partitura.GetAllParts(pageCount);
			}

			public IList<Part> AllParts { get; }

			public List<string> ImagePaths { get; }

			public string RuntimePath { get; }

			public string PartPattern { get; }
		}

		private readonly IFileSystem _fileSystem;

		public EditedPartitura(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

		public string Path { get; private set; }

		public Partitura Partitura { get; private set; }

		public void Open(string path, Partitura partitura)
		{
			CheckImageFileConsistency(path);

			Path = path;
			Partitura = partitura;
			CurrentPageIndex = 0;
		}

		public void Save(string newPath)
		{
			if (Path != null && !newPath.Equals(Path))
			{
				var oldImages = GetAllImages(Path).OrderBy(p => p);
				int index = 0;
				foreach (string oldImage in oldImages)
				{
					string newImage = GetImagePath(index, newPath);
					_fileSystem.Copy(oldImage, newImage);
					index++;
				}
			}

			//using (Stream str = _fileSystem.OpenWrite(newPath))
			//{
			//	BinaryFormatter sf = new BinaryFormatter();
			//	sf.Serialize(str, Partitura.ToMemento());
			//}

			var asString = JsonSerializer.Serialize(Partitura.ToMemento());
			_fileSystem.WriteFile(newPath, asString);

			Path = newPath;
		}

		public void Close()
		{
			Path = null;
			Partitura = null;
		}

		public int CurrentPageIndex { get; set; }

		public string CurrentImagePath => GetImagePath(CurrentPageIndex, Path);


		public int GetPageCount()
		{
			return GetAllImages(Path).Count;
		}

		public string GetImagePath(int index)
		{
			return GetImagePath(index, Path);
		}

		public Page CurrentPage => Partitura?.GetPage(CurrentPageIndex);


		public bool IsOpen => Partitura != null;


		public IPlayedPartitura CreatePlayedPartitura()
		{
			return new PlayedPartitura(this);
		}

		private void CheckImageFileConsistency(string path)
		{
			ICollection<string> matchingFiles = GetAllImages(path);
			for (int i = 0; i < matchingFiles.Count; i++)
				if (!matchingFiles.Contains(
					GetImagePath(i, path)))
					throw new PartituraException(
						"The image files for " + GetPartituraName(path) + " are not properly indexed.\n" +
						"Please make sure that the image files have a three digit index that starts with '000' and there is a file for each page.");
			//if there are less image files than pages in Partitura, we ignore this,
			//and keep the extra page stored forever.
		}

		private ICollection<string> GetAllImages(string path)
		{
			string dir = System.IO.Path.GetDirectoryName(path);
			string partituraName = GetPartituraName(path);

			ICollection<string> matchingImages =
				_fileSystem.GetMatchingFileNames(dir, partituraName + "-???.jpg");
			return matchingImages;
		}

		private static string GetPartituraName(string path)
		{
			string fileName = System.IO.Path.GetFileName(path);
			return fileName.Substring(0, fileName.Length - ".partitura.json".Length);
		}

		private string GetImagePath(int index, string path)
		{
			string dir = System.IO.Path.GetDirectoryName(path);
			string partituraName = GetPartituraName(path);
			string indexString = index.ToString("000");

			return System.IO.Path.Combine(dir, partituraName + "-" + indexString + ".jpg");
		}
	}
}