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
            if (!P.Active.IsOpen || new ClosePartituraCommand(P).TryExecute())
            {
                P.MainWindow.CloseAndExit();
            }
        }

        public bool CanExecute()
        {
            return true;
        }
    }
}