using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PartituraMagica
{
    /** 
     * <see cref="IActivePartitura"/>
     */
    public class ActivePartitura : IActivePartitura
    {
        private string path;
        private Partitura partitura;
        private int currentPageIndex = 0;

        private IFileSystem fileSystem;

        public ActivePartitura(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public string Path
        {
            get
            {
                return this.path;
            }
        }

        public Partitura Partitura
        {
            get
            {
                return this.partitura;
            }
        }

        public void Open(string path, Partitura partitura)
        {
            CheckImageFileConsistency(path);

            this.path = path;
            this.partitura = partitura;
            this.currentPageIndex = 0;
        }

        private void CheckImageFileConsistency(string path)
        {
            ICollection<string> matchingFiles = this.GetAllImages(path);
            for (int i = 0; i < matchingFiles.Count; i++)
            {
                if (!matchingFiles.Contains(
                        this.GetImage(i, path)))
                {

                    throw new PartituraException(
                        "The image files for " + GetPartituraName(path) + " are not properly indexed.\n" +
                        "Please make sure that the image files have a three digit index that starts with '000' and there is a file for each page.");
                }
            }
            //if there are less image files than pages in Partitura, we ignore this,
            //and keep the extra page stored forever.
        }

        public void Save(string newPath)
        {
            if (this.Path != null && !newPath.Equals(this.Path))
            {
                ICollection<string> oldImages = new List<string>(GetAllImages(this.Path));
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
                sf.Serialize(str, this.Partitura.SaveToMemento());
            }
            this.path = newPath;
        }

        public void Close()
        {
            this.path = null;
            this.partitura = null;
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

        public int CurrentPageIndex
        {
            get
            {
                return this.currentPageIndex;
            }
            set
            {
                this.currentPageIndex = value;
            }
        }

        public string CurrentImagePath
        {
            get
            {
                return this.GetImage(CurrentPageIndex, this.Path);
            }
        }

        private string GetImage(int index, string path)
        {
            string dir = System.IO.Path.GetDirectoryName(path);
            string partituraName = System.IO.Path.GetFileNameWithoutExtension(path);
            string indexString = index.ToString("000");

            return System.IO.Path.Combine(dir, partituraName + "-" + indexString + ".jpg");
        }


        public int GetPageCount()
        {
            return this.GetAllImages(this.Path).Count;
        }

        public string GetImage(int index)
        {
            return GetImage(index, this.Path);
        }


        public Page CurrentPage
        {
            get
            {
                if (this.partitura != null)
                {
                    return this.partitura.GetPage(this.currentPageIndex);
                }
                return null;
            }
        }


        public bool IsOpen
        {
            get { return this.partitura != null; }
        }
    }
}
