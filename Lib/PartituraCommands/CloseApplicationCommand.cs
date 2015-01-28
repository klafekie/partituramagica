using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{
    public class CloseApplicationCommand : PartituraCommandBase, ICommand
    {
        public CloseApplicationCommand(ICommandParameters p)
            : base(p)
        {
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            if (!p.Active.IsOpen || new ClosePartituraCommand(this.p).TryExecute())
            {
                p.MainWindow.CloseAndExit();
            }
        }

        public bool CanExecute()
        {
            return true;
        }
    }
}
