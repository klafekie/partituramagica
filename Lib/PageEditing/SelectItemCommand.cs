using System.Diagnostics;
using System.Windows;

namespace PartituraMagica
{
    public class SelectItemCommand : PartituraCommandBase, ICommand
    {
        protected Point Pos;

        public SelectItemCommand(Point pos, ICommandParameters p)
            : base(p)
        {
            Pos = pos;
        }

        public bool CanExecute()
        {
            return P.Active.CurrentPage != null && P.Active.CurrentPage.GetShapeTypeAt(Pos) != Shape.NothingSpecial;
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            P.Active.CurrentPage.SelectBoxAt(Pos, P.Panel);
        }
    }
}