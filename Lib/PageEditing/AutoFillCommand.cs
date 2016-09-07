using System.Diagnostics;

namespace PartituraMagica
{
    public class AutoFillCommand : PartituraCommandBase, ICommand
    {
        public AutoFillCommand(ICommandParameters p)
            : base(p)
        {
        }

        public bool CanExecute()
        {
            return P.Active.IsOpen;
        }


        public void Execute()
        {
            Debug.Assert(CanExecute());
            P.MainWindow.SignalWait(true);
            BoxFinder b = new BoxFinder(P.Panel.CurrentImage);
            foreach (Rect r in b.Execute())
            {
                new AddBoxCommand(r, P).TryExecute();
            }
            P.MainWindow.SignalWait(false);
        }
    }
}