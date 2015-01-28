using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace PartituraMagica
{
    public class ShowPageCommand : PartituraCommandBase, ICommand
    {
        private int index;

        public ShowPageCommand(int index, ICommandParameters p)
            : base(p)
        {
            this.index = index;
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            p.Active.CurrentPageIndex = index;
            p.Panel.Clear();
            Point dimensions = p.Panel.DrawImage(p.Active.CurrentImagePath);

            p.Active.Partitura.Paint(index, p.Panel, dimensions);
        }

        public bool CanExecute()
        {
            return p.Active != null && p.Active.IsOpen &&
                index >= 0 && index < p.Active.GetPageCount();
        }
    }
}
