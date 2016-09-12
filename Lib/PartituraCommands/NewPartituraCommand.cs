using System.Diagnostics;
using System.IO;

namespace PartituraMagica
{
    public class NewPartituraCommand : PartituraCommandBase, ICommand
    {
        public NewPartituraCommand(ICommandParameters p)
            : base(p)
        {
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            string path = P.FileSystem.ShowSaveDialog(FileType.Partitura);

            if (path != null)
            {
                new ClosePartituraCommand(P).TryExecute();

                if (!P.Active.IsOpen)
                {
                    //only continue if previous piece was really closed.				

                    Partitura partitura = new Partitura();
                    P.Active.Open(path, partitura);

                    //show the first page if we already have at least one image file
                    //matching on disk
                    //otherwise add a page, 
                    if (!new ShowPageCommand(0, P).TryExecute())
                    {
                        new AddPagesCommand(P).TryExecute();
                    }
                    P.MainWindow.ShowPartituraInfo(Path.GetFileNameWithoutExtension(P.Active.Path), P.Active.CurrentPageIndex, P.Active.GetPageCount());
                }
            }
        }


        public bool CanExecute()
        {
            return true;
        }
    }
}