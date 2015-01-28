using System;
using System.Windows;
using System.Windows.Interop;

namespace PartituraMagica
{
    internal class EditingController : ICommandParameters, IDisposable
    {
        private readonly IDrawingPanel panel;
        private readonly IActivePartitura active;
        private readonly IFileSystem fileSystem;
        private readonly MainWindow mainWindow;
        private PlayerViewD3D playerView;

        public EditingController(MainWindow mainWindow, IDrawingPanel panel)
        {
            this.panel = panel;
            fileSystem = new FileSystem();
            active = new ActivePartitura(fileSystem);
            this.mainWindow = mainWindow;

#if DEBUG
            //@todo p3 for testing ->
            try
            {
                //string debugQuickLoadFileName = @"D:\eike\PartituraMagica\Examples\aussente.partitura";
                string debugQuickLoadFileName = @"c:\VisualStudio\PartituraMagica\Examples\aussente.partitura";
                //string debugQuickLoadFileName = @"C:\Users\jan\Documents\My Partitura Files\aussente.partitura"; // for jed Note-008
                //string debugQuickLoadFileName = @"C:\Users\jan\Documents\My Partitura Files\Aussente04\aussente4b.partitura"; // for jed Note-008
                //string debugQuickLoadFileName = @"C:\Users\jan\Documents\My Partitura Files\Holy\Holy.partitura"; // for jed Note-008
                //string debugQuickLoadFileName = @"C:\Users\jan\Documents\My Partitura Files\Holy20141229a\Holy01b.partitura"; // for jed Note-008
                new OpenCommand(debugQuickLoadFileName, this).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //ignore, testing code...
            }
            //<-
#endif
        }

        public IActivePartitura Active { get { return active; } }

        public Page CurrentPage
        {
            get
            {
                if (active.Partitura != null)
                {
                    int index = active.CurrentPageIndex;
                    return active.Partitura.GetPage(index);
                }
                return null;
            }
        }

        public ICommand CreateCommand(string name)
        {
            bool takesLong;
            return CreateCommand(name, out takesLong);
        }

        public ICommand CreateCommand(string name, out bool takesLong)
        {
            takesLong = false;
            switch (name)
            {
                case "New":
                    takesLong = true;
                    return new NewPartituraCommand(this);
                case "Open":
                    return new OpenCommand(this);
                case "Save":
                    return new SaveCommand(false, this);
                case "SaveAs":
                    return new SaveCommand(true, this);
                case "CloseApplication":
                    return new CloseApplicationCommand(this);
                case "ClosePartitura":
                    return new ClosePartituraCommand(this);
                case "Delete":
                    return new RemoveBoxCommand(this);
                case "AddPage":
                    takesLong = true;
                    return new AddPagesCommand(this);
                case "AutoFill":
                    takesLong = true;
                    return new AutoFillCommand(this);
                case "NextPage":
                    return new ShowPageCommand(active.CurrentPageIndex + 1, this);
                case "PreviousPage":
                    return new ShowPageCommand(active.CurrentPageIndex - 1, this);
                case "Label":
                    return new LabelPartCommand(this);
                case "UnLabel":
                    return new UnLabelPartCommand(this);
                case "UpdatePartPattern":
                    return new UpdatePartPatternCommand(this);
                case "StartPlayer":
                    Func<IPlayerView> createView = delegate
                    {
                        WindowInteropHelper h = new WindowInteropHelper(mainWindow);
                        if (playerView == null)
                        {
                            playerView = new PlayerViewD3D(Active, panel, h.Handle);
                        }
                        return playerView;
                    };
                    return new ShowPlayerCommand(createView, this);
                default:
                    throw new ArgumentException("unknown command");
            }
        }

        public bool CanExecuteCommand(string name)
        {
            ICommand cmd = CreateCommand(name);
            if (cmd != null)
            {
                return cmd.CanExecute();
            }
            return false;
        }

        public ICommand CreateMouseDownCommand(Point pos, int clickCount)
        {
            ICommand cmd = null;
            if (clickCount == 1)
            {
                cmd = new SelectItemCommand(pos, this);
                if (!cmd.CanExecute())
                {
                    cmd = new AddBoxCommand(pos, this);
                    if (!cmd.CanExecute())
                    {
                        return null;
                    }
                }
            }
            else
            {
                cmd = new SplitBoxCommand(pos, this);
                if (!cmd.CanExecute())
                {
                    return null;
                }
            }
            return cmd;
        }

        public ICommand CreateMouseMoveCommand(Point lastPos, Point currentPos, bool leftButtonIsDown)
        {
            if (leftButtonIsDown)
            {
                ICommand cmd = new UpdateBoxCommand(lastPos, currentPos, this);
                if (cmd.CanExecute())
                {
                    return cmd;
                }
            }
            return null;
        }

        public Shape GetShapeAt(Point currentPos)
        {
            if (!active.IsOpen)
            {
                return Shape.NothingSpecial;
            }
            return CurrentPage.GetShapeTypeAt(currentPos);
        }

        public IDrawingPanel Panel { get { return panel; } }

        public IFileSystem FileSystem { get { return fileSystem; } }

        public IMainWindow MainWindow { get { return mainWindow; } }

        public void Dispose()
        {
            DisposePlayerView();
        }

        public void DisposePlayerView()
        {
            if (playerView != null)
            {
                playerView.Dispose();
                playerView = null;
            }
        }
    }
}