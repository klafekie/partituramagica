using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            Debug.Assert(CanExecute());
            ICollection<string> sourceFiles = P.FileSystem.ShowOpenImagesDialog();
            if (sourceFiles.Any())
            {
                int firstAddedIndex = P.Active.GetPageCount();
                foreach (var sourceFile in sourceFiles)
                {
                    int index = P.Active.GetPageCount();
                    string destFile = P.Active.GetImagePath(index);
                    Debug.Assert(sourceFile != destFile,
                        "cause the index of destFile is always higher than the index of all existing image files");
                    P.FileSystem.Copy(sourceFile, destFile);
                    new ShowPageCommand(index, P).TryExecute();
                    if (P.Active.CurrentPageIndex == index)
                    {
                        //show was successfull
                        new AutoFillCommand(P).TryExecute();
                    }
                }

                //go back to the first added page
                new ShowPageCommand(firstAddedIndex, P).TryExecute();
            }
            P.MainWindow.ShowPartituraInfo(P.Active);
        }
    }
}