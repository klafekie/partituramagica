using System.Diagnostics;

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
            if (!p.Active.IsOpen || new ClosePartituraCommand(p).TryExecute())
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