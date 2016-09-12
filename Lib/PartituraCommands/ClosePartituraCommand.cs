using System.Diagnostics;

namespace PartituraMagica
{
    public class ClosePartituraCommand : PartituraCommandBase, ICommand
    {
        public ClosePartituraCommand(ICommandParameters p)
            : base(p)
        {
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            DialogResult c = DialogResult.No;
            ICommand cmd = new SaveCommand(false, P);
            if (cmd.CanExecute())
            {
                c = P.FileSystem.AskToSave();
                if (c == DialogResult.Cancel)
                {
                    return;
                }
                if (c == DialogResult.Yes)
                {
                    cmd.Execute();
                }
            }
            if (P.Active.IsOpen && (!P.Active.Partitura.Dirty || c == DialogResult.No))
            {
                //file was really safed or user doesn't care
                P.Panel.Clear();
                P.Active.Close();
                P.DisposePlayerView();
                P.MainWindow.ClearPartituraInfo();
            }
        }

        public bool CanExecute()
        {
            return P.Active != null && P.Active.IsOpen;
        }
    }
}