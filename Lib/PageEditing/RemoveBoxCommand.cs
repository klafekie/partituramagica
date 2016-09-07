using System.Diagnostics;

namespace PartituraMagica
{
    /** removes the currently selected box. 
     */

    public class RemoveBoxCommand : PartituraCommandBase, ICommand
    {
        public RemoveBoxCommand(ICommandParameters p)
            : base(p)
        {
        }

        public bool CanExecute()
        {
            return p.Active.CurrentPage != null && p.Active.CurrentPage.IsBoxSelected();
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            p.Active.Partitura.RemoveSelectedBox(p.Active.CurrentPageIndex, p.Panel);
        }
    }
}