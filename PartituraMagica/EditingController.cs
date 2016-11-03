using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Interop;

namespace PartituraMagica
{
    internal class EditingController : ICommandParameters, IDisposable
    {
        private readonly MainWindow _mainWindow;
        private PlayerViewD3D _playerView;

        public EditingController(MainWindow mainWindow, IDrawingPanel panel)
        {
            Panel = panel;
            FileSystem = new FileSystem();
            Active = new ActivePartitura(FileSystem);
            _mainWindow = mainWindow;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
            {
                //first arg is the exe name
                new OpenCommand(args[1], this).Execute();
            }
        }

        public Page CurrentPage
        {
            get
            {
                if (Active.Partitura != null)
                {
                    int index = Active.CurrentPageIndex;
                    return Active.Partitura.GetPage(index);
                }
                return null;
            }
        }

        private string PlayerRuntimePath
        {
            get { return Active.Path + ".runtime"; }
        }

        public IActivePartitura Active { get; }

        public IDrawingPanel Panel { get; }

        public IFileSystem FileSystem { get; }

        public IMainWindow MainWindow
        {
            get { return _mainWindow; }
        }

        public void DisposePlayerView()
        {
            if (_playerView != null)
            {
                _playerView.Dispose();
                _playerView = null;
            }
        }

        public void Dispose()
        {
            DisposePlayerView();
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
                    return new ShowPageCommand(Active.CurrentPageIndex + 1, this);
                case "PreviousPage":
                    return new ShowPageCommand(Active.CurrentPageIndex - 1, this);
                case "Label":
                    return new LabelPartCommand(this);
                case "UnLabel":
                    return new UnLabelPartCommand(this);
                case "UpdatePartPattern":
                    return new UpdatePartPatternCommand(this);
                case "TuningTone":
                    return new PlayTuningToneCommand();
                case "StartPlayer":
                    Func<PlayerPositionHolder, IPartNavigator, IPlayerView> createView = (positionProvider, playerView) =>
                    {
                        WindowInteropHelper h = new WindowInteropHelper(_mainWindow);
                        
                        _playerView = new PlayerViewD3D(Active, Panel, h.Handle, LoadPlayer(), positionProvider, playerView);
                        _playerView.Closed += (o, args) =>
                        {
                            SavePlayer(_playerView);
                            _playerView = null;
                        };
                        return _playerView;
                    };
                    return new ShowPlayerCommand(createView, this);
                default:
                    throw new ArgumentException("unknown command");
            }
        }

        private PlayerMemento LoadPlayer()
        {
            if (!FileSystem.FileExists(PlayerRuntimePath))
            {
                return new PlayerMemento();
            }
            using (Stream str = FileSystem.OpenRead(PlayerRuntimePath))
            {
                BinaryFormatter sf = new BinaryFormatter();
                return (PlayerMemento)sf.Deserialize(str);
            }
        }

        private void SavePlayer(IPlayerView view)
        {
            using (Stream str = FileSystem.OpenWrite(PlayerRuntimePath))
            {
                BinaryFormatter sf = new BinaryFormatter();
                sf.Serialize(str, view.ToMemento());
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
            if (!Active.IsOpen)
            {
                return Shape.NothingSpecial;
            }
            return CurrentPage.GetShapeTypeAt(currentPos);
        }
    }
}