using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Globalization;
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
            return p.Active != null && p.Active.IsOpen;
        }

        public void Execute()
        {
            //@todo p2 fallback handling for unexpected exceptions (close partitura??)
            Debug.Assert(CanExecute());
            string sourceFile = p.FileSystem.ShowOpenDialog(FileType.Image);
            if (sourceFile != null)
            {
                int index = p.Active.GetPageCount();
                string destFile = p.Active.GetImage(index);
                Debug.Assert(sourceFile != destFile, "cause the index of destFile is always higher than the index of all existing image files");
                p.FileSystem.Copy(sourceFile, destFile);

                //show the added page
                new ShowPageCommand(index, p).TryExecute();
                if (p.Active.CurrentPageIndex == index)
                {	//show was successfull
                    new AutoFillCommand(p).TryExecute();
                }
            }
        }
    }
}
