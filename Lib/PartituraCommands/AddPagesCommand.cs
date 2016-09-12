using System.Diagnostics;
using System.IO;

namespace PartituraMagica
{
    public class AddPagesCommand : PartituraCommandBase, ICommand
    {
        public AddPagesCommand(ICommandParameters p)
            : base(p)
        {
        }

        public bool CanExecute()
        {
            return P.Active != null && P.Active.IsOpen;
        }

        public void Execute()
        {
            //@todo p2 fallback handling for unexpected exceptions (close partitura??)
            Debug.Assert(CanExecute());
            string sourceFile = P.FileSystem.ShowOpenDialog(FileType.Image);
            if (sourceFile != null)
            {
                int index = P.Active.GetPageCount();
                string destFile = P.Active.GetImagePath(index);
                Debug.Assert(sourceFile != destFile,
                    "cause the index of destFile is always higher than the index of all existing image files");
                P.FileSystem.Copy(sourceFile, destFile);

                //show the added page
                new ShowPageCommand(index, P).TryExecute();
                if (P.Active.CurrentPageIndex == index)
                {
                    //show was successfull
                    new AutoFillCommand(P).TryExecute();
                }
                P.MainWindow.ShowPartituraInfo(Path.GetFileNameWithoutExtension(P.Active.Path), P.Active.CurrentPageIndex, P.Active.GetPageCount());
            }
        }
    }
}