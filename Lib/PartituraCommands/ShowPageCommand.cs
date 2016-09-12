using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PartituraMagica
{
    public class ShowPageCommand : PartituraCommandBase, ICommand
    {
        private readonly int _index;

        public ShowPageCommand(int index, ICommandParameters p)
            : base(p)
        {
            _index = index;
        }

        public void Execute()
        {
            Debug.Assert(CanExecute());
            P.Active.CurrentPageIndex = _index;
            P.Panel.Clear();
            Point dimensions = P.Panel.DrawImage(P.Active.CurrentImagePath);

            P.Active.Partitura.Paint(_index, P.Panel, dimensions);
            P.MainWindow.ShowPartituraInfo(Path.GetFileNameWithoutExtension(P.Active.Path), P.Active.CurrentPageIndex, P.Active.GetPageCount());
        }

        public bool CanExecute()
        {
            return P.Active != null && P.Active.IsOpen &&
                   _index >= 0 && _index < P.Active.GetPageCount();
        }
    }
}