using System.Diagnostics;
using System.IO;

namespace PartituraMagica
{
    public class SaveCommand : PartituraCommandBase, ICommand
    {
        private readonly bool _saveAs;

        public SaveCommand(bool saveAs, ICommandParameters p)
            : base(p)
        {
            _saveAs = saveAs;
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
            P.MainWindow.ShowPartituraInfo(Path.GetFileNameWithoutExtension(P.Active.Path), P.Active.CurrentPageIndex, P.Active.GetPageCount());
        }

        public bool CanExecute()
        {
            return P.Active != null && P.Active.IsOpen
                   && (_saveAs || P.Active.Partitura.Dirty);
        }
    }
}