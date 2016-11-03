using System.Diagnostics;

namespace PartituraMagica
{
    public class NewPartituraCommand : PartituraCommandBase, ICommand
    {
        public NewPartituraCommand(ICommandParameters p)
            : base(p)
        {
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            string path = P.FileSystem.ShowSavePartituraDialog(false);

            if (path != null)
            {
                new ClosePartituraCommand(P).TryExecute();

                if (!P.Active.IsOpen)
                {
                    //only continue if previous piece was really closed.				

                    Partitura partitura = new Partitura();
                    P.Active.Open(path, partitura);

                    new AddPagesCommand(P).Execute();
                }
            }
            P.MainWindow.ShowPartituraInfo(P.Active);
        }


        public bool CanExecute()
        {
            return true;
        }
    }
}