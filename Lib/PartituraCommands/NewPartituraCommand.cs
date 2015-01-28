using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            string path = p.FileSystem.ShowSaveDialog(FileType.Partitura);

            if (path != null)
            {
                new ClosePartituraCommand(p).TryExecute();

                if (!p.Active.IsOpen)
                {
                    //only continue if previous piece was really closed.				

                    Partitura partitura = new Partitura();
                    p.Active.Open(path, partitura);

                    //show the first page if we already have at least one image file
                    //matching on disk
                    //otherwise add a page, 
                    if (!new ShowPageCommand(0, p).TryExecute())
                    {
                        new AddPagesCommand(p).TryExecute();
                    }
                }
            }
        }


        public bool CanExecute()
        {
            return true;
        }
    }
}
