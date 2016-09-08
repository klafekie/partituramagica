using System.Diagnostics;
using System.Windows;

namespace PartituraMagica
{
    public class UpdateBoxCommand : PartituraCommandBase, ICommand
    {
        private readonly Point _lastPos;
        private readonly Point _newPos;

        public UpdateBoxCommand(Point lastPos, Point newPos, ICommandParameters p)
            : base(p)
        {
            _lastPos = lastPos;
            _newPos = newPos;
        }

        public bool CanExecute()
        {
            Page current = P.Active.CurrentPage;
            if (current == null || !current.IsBoxSelected())
            {
                return false;
            }
            return current.CanUpdateSelectedBox(_lastPos, _newPos);
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            P.Active.CurrentPage.UpdateSelectedBox(_lastPos, _newPos, P.Panel);
        }
    }
}