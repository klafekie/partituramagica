using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PartituraMagica
{
    /** 
     * <see cref="IActivePartitura"/>
     */

    public class ActivePartitura : IActivePartitura
    {
        private readonly IFileSystem fileSystem;

        public ActivePartitura(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
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
                ICollection<string> oldImages = new List<string>(GetAllImages(Path));
                foreach (string oldImage in oldImages)
                {
                    string newDir = System.IO.Path.GetDirectoryName(newPath);
                    string fileName = System.IO.Path.GetFileName(oldImage);
                    string newImage = System.IO.Path.Combine(newDir, fileName);
                    fileSystem.Copy(oldImage, newImage);
                }
            }
            using (Stream str = fileSystem.OpenWrite(newPath))
            {
                BinaryFormatter sf = new BinaryFormatter();
                sf.Serialize(str, Partitura.ToMemento());
            }
            Path = newPath;
        }

        public void Close()
        {
            Path = null;
            Partitura = null;
        }

        public int CurrentPageIndex { get; set; }

        public string CurrentImagePath
        {
            get { return GetImage(CurrentPageIndex, Path); }
        }


        public int GetPageCount()
        {
            return GetAllImages(Path).Count;
        }

        public string GetImage(int index)
        {
            return GetImage(index, Path);
        }


        public Page CurrentPage
        {
            get
            {
                return Partitura?.GetPage(CurrentPageIndex);
            }
        }


        public bool IsOpen
        {
            get { return Partitura != null; }
        }

        private void CheckImageFileConsistency(string path)
        {
            ICollection<string> matchingFiles = GetAllImages(path);
            for (int i = 0; i < matchingFiles.Count; i++)
            {
                if (!matchingFiles.Contains(
                    GetImage(i, path)))
                {
                    throw new PartituraException(
                        "The image files for " + GetPartituraName(path) + " are not properly indexed.\n" +
                        "Please make sure that the image files have a three digit index that starts with '000' and there is a file for each page.");
                }
            }
            //if there are less image files than pages in Partitura, we ignore this,
            //and keep the extra page stored forever.
        }

        private ICollection<string> GetAllImages(string path)
        {
            string dir = System.IO.Path.GetDirectoryName(path);
            string partituraName = GetPartituraName(path);

            ICollection<string> matchingImages =
                fileSystem.GetMatchingFileNames(dir, partituraName + "-???.jpg");
            return matchingImages;
        }

        private static string GetPartituraName(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        private string GetImage(int index, string path)
        {
            string dir = System.IO.Path.GetDirectoryName(path);
            string partituraName = System.IO.Path.GetFileNameWithoutExtension(path);
            string indexString = index.ToString("000");

            return System.IO.Path.Combine(dir, partituraName + "-" + indexString + ".jpg");
        }
    }
}