using System.Diagnostics;

namespace PartituraMagica
{
    public class SaveCommand : PartituraCommandBase, ICommand
    {
        private readonly bool saveAs;

        public SaveCommand(bool saveAs, ICommandParameters p)
            : base(p)
        {
            this.saveAs = saveAs;
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            string path = p.Active.Path;

            if (saveAs || path == null)
            {
                path = p.FileSystem.ShowSaveDialog(FileType.Partitura);
            }

            if (path != null)
            {
                p.Active.Save(path);
            }
        }

        public bool CanExecute()
        {
            return p.Active != null && p.Active.IsOpen
                && (saveAs || p.Active.Partitura.Dirty);
        }
    }
}
