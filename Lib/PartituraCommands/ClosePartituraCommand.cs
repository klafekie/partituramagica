using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{
    public class ClosePartituraCommand : PartituraCommandBase, ICommand
    {
        public ClosePartituraCommand(ICommandParameters p)
            : base(p)
        {
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            DialogResult c = DialogResult.No;
            ICommand cmd = new SaveCommand(false, p);
            if (cmd.CanExecute())
            {
                c = p.FileSystem.AskToSave();
                if (c == DialogResult.Cancel)
                {
                    return;
                }
                if (c == DialogResult.Yes)
                {
                    cmd.Execute();
                }
            }
            if (p.Active.IsOpen && (!p.Active.Partitura.Dirty || c == DialogResult.No))
            {
                //file was really safed or user doesn't care
                p.Panel.Clear();
                p.Active.Close();
                p.DisposePlayerView();
            }
        }

        public bool CanExecute()
        {
            return p.Active != null && p.Active.IsOpen;
        }
    }
}
