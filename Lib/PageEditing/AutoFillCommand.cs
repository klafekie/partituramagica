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
            return p.Active.IsOpen;
        }


        public void Execute()
        {
            Debug.Assert(CanExecute());
            p.MainWindow.SignalWait(true);
            BoxFinder b = new BoxFinder(p.Panel.CurrentImage);
            foreach (Rect r in b.Execute())
            {
                new AddBoxCommand(r, p).TryExecute();
            }
            p.MainWindow.SignalWait(false);
        }
    }
}