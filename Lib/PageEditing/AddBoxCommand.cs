using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;

namespace PartituraMagica
{
    public class AddBoxCommand : PartituraCommandBase, ICommand
    {
        private Rect r;
        public AddBoxCommand(Point pos, ICommandParameters p)
            : base(p)
        {
            this.r = CreateRect(pos);
        }

        public AddBoxCommand(Rect r, ICommandParameters p)
            : base(p)
        {
            this.r = r;
        }

        public bool CanExecute()
        {
            return p.Active != null && p.Active.CurrentPage != null &&
                !p.Active.CurrentPage.Overlaps(this.r);
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            //Trace.TraceInformation("Adding Box at " + r);
            p.Active.Partitura.AddBox(p.Active.CurrentPageIndex, r, p.Panel);
        }

        private Rect CreateRect(Point pos)
        {
            double defaultWidth = 2 * Page.ClickTolerance + 1;
            double defaultHeight = defaultWidth;
            Rect r = new Rect(pos.X - defaultWidth, pos.Y - defaultHeight,
                    defaultWidth, defaultHeight);
            return r;
        }

    }
}
