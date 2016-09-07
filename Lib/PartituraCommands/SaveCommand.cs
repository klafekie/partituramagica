using System.Diagnostics;

namespace PartituraMagica
{
    public class SaveCommand : PartituraCommandBase, ICommand
    {
        private readonly bool _saveAs;

        public SaveCommand(bool saveAs, ICommandParameters p)
            : base(p)
        {
            this._saveAs = saveAs;
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            string path = P.Active.Path;

            if (_saveAs || path == null)
            {
                path = P.FileSystem.ShowSaveDialog(FileType.Partitura);
            }

            if (path != null)
            {
                P.Active.Save(path);
            }
        }

        public bool CanExecute()
        {
            return P.Active != null && P.Active.IsOpen
                   && (_saveAs || P.Active.Partitura.Dirty);
        }
    }
}