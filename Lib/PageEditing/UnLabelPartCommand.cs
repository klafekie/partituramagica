using System.Diagnostics;

namespace PartituraMagica
{
    public class UnLabelPartCommand : PartituraCommandBase, ICommand
    {
        public UnLabelPartCommand(ICommandParameters p)
            : base(p)
        {
        }

        public bool CanExecute()
        {
            if (p.Active == null || !p.Active.IsOpen) return false;
            if (p.Active.CurrentPageIndex == 0 && p.Active.CurrentPage.IsLeftTopSelected())
            {
                //the implicit 'A' cannot be removed
                return false;
            }
            return p.Active.CurrentPage.IsBoxSelected() && p.Active.CurrentPage.IsSelectionLabelled();
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            p.Active.Partitura.UnLabelSelection(p.Panel, p.Active.CurrentPageIndex);
        }
    }
}