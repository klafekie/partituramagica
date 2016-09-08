using System.Diagnostics;
using System.Windows;

namespace PartituraMagica
{
    public class AddBoxCommand : PartituraCommandBase, ICommand
    {
        private readonly Rect _r;

        public AddBoxCommand(Point pos, ICommandParameters p)
            : base(p)
        {
            _r = CreateRect(pos);
        }

        public AddBoxCommand(Rect r, ICommandParameters p)
            : base(p)
        {
            _r = r;
        }

        public bool CanExecute()
        {
            return P.Active != null && P.Active.CurrentPage != null &&
                   !P.Active.CurrentPage.Overlaps(_r);
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            //Trace.TraceInformation("Adding Box at " + r);
            P.Active.Partitura.AddBox(P.Active.CurrentPageIndex, _r, P.Panel);
        }

        private Rect CreateRect(Point pos)
        {
            double defaultWidth = 2*Page.ClickTolerance + 1;
            double defaultHeight = defaultWidth;
            Rect r = new Rect(pos.X - defaultWidth, pos.Y - defaultHeight,
                defaultWidth, defaultHeight);
            return r;
        }
    }
}