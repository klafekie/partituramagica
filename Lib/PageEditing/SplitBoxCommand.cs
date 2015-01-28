using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;

namespace PartituraMagica
{
    /**
     * Split the currently selected box vertically.
     * If current position is inside the selected box, the vertical splitter is inserted
     * at that position. Otherwise it is inserted in the middle of the selected box, or as near
     * as possible to the middle if the middle is already occupied by a splitter.
     */
    public class SplitBoxCommand : PartituraCommandBase, ICommand
    {
        private Point pos;
        public SplitBoxCommand(Point pos, ICommandParameters p)
            : base(p)
        {
            this.pos = pos;
        }

        public bool CanExecute()
        {
            return p.Active.CurrentPage != null && p.Active.CurrentPage.IsBoxSelected();
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            p.Active.CurrentPage.SplitSelectedBox(pos, p.Panel);
        }
    }
}
