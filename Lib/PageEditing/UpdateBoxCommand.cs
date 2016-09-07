using System.Diagnostics;
using System.Windows;

namespace PartituraMagica
{
    public class UpdateBoxCommand : PartituraCommandBase, ICommand
    {
        private readonly Point lastPos;
        private readonly Point newPos;

        public UpdateBoxCommand(Point lastPos, Point newPos, ICommandParameters p)
            : base(p)
        {
            this.lastPos = lastPos;
            this.newPos = newPos;
        }

        public bool CanExecute()
        {
            Page current = p.Active.CurrentPage;
            if (current == null || !current.IsBoxSelected())
            {
                return false;
            }
            return current.CanUpdateSelectedBox(lastPos, newPos);
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            p.Active.CurrentPage.UpdateSelectedBox(lastPos, newPos, p.Panel);
        }
    }
}