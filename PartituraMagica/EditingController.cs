using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Interop;
using System.Diagnostics;

namespace PartituraMagica
{
	class EditingController : ICommandParameters, IDisposable
	{
		private IDrawingPanel panel;
		private IActivePartitura active;
		private IFileSystem fileSystem;
		private MainWindow mainWindow;
		private PlayerViewD3D playerView = null;

		public EditingController(MainWindow mainWindow, IDrawingPanel panel)
		{
			this.panel = panel;
			this.fileSystem = new FileSystem();
			this.active = new ActivePartitura(this.fileSystem);
			this.mainWindow = mainWindow;

#if DEBUG
			//@todo p3 for testing ->
			try {				

//				new OpenCommand(@"D:\eike\PartituraMagica\Examples\aussente.partitura", this).Execute();
				new OpenCommand(@"C:\Dokumente und Einstellungen\2-22-a\Eigene Dateien\PartituraMagica\Examples\parts.partitura", this).Execute();
			} catch (Exception e) {
				Console.WriteLine(e);
				//ignore, testing code...
			}
			//<-
#endif
		}

		public IActivePartitura Active
		{
			get
			{
				return this.active;
			}
		}

		public Page CurrentPage
		{
			get
			{
				if (this.active.Partitura != null) {
					int index = this.active.CurrentPageIndex;
					return this.active.Partitura.GetPage(index);
				} else {
					return null;
				}
			}
		}

		public ICommand CreateCommand(string name) {
			bool takesLong;
			return CreateCommand(name, out takesLong);
		}		

		public ICommand CreateCommand(string name, out bool takesLong)
		{
			takesLong = false;
			switch (name) {
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
					Func<IPlayerView> createView = delegate() {
						WindowInteropHelper h = new WindowInteropHelper(this.mainWindow);
						if (this.playerView == null) {
							this.playerView = new PlayerViewD3D(this.Active, this.panel, h.Handle);
						}
						return this.playerView;
					};
					return new ShowPlayerCommand(createView, this);
				default:
					throw new ArgumentException("unknown command");
			}
		}

		public bool CanExecuteCommand(string name)
		{
			ICommand cmd = CreateCommand(name);
			if (cmd != null) {
				return cmd.CanExecute();
			} else {
				return false;
			}
		}

		public ICommand CreateMouseDownCommand(Point pos, int clickCount)
		{
			ICommand cmd = null;
			if (clickCount == 1) {
				cmd = new SelectItemCommand(pos, this);
				if (!cmd.CanExecute()) {
					cmd = new AddBoxCommand(pos, this);
					if (!cmd.CanExecute()) {
						return null;
					}
				}
			} else {
				cmd = new SplitBoxCommand(pos, this);
				if (!cmd.CanExecute()) {
					return null;
				}
			}
			return cmd;
		}

		public ICommand CreateMouseMoveCommand(Point lastPos, Point currentPos, bool leftButtonIsDown)
		{
			if (leftButtonIsDown) {
				ICommand cmd = new UpdateBoxCommand(lastPos, currentPos, this);
				if (cmd.CanExecute()) {
					return cmd;
				}
			}
			return null;
		}

		public Shape GetShapeAt(Point currentPos)
		{
			if (!this.active.IsOpen) {
				return Shape.NothingSpecial;
			}
			return CurrentPage.GetShapeTypeAt(currentPos);
		}

		public IDrawingPanel Panel
		{
			get
			{
				return this.panel;
			}
		}

		public IFileSystem FileSystem
		{
			get
			{
				return this.fileSystem;
			}
		}


		public IMainWindow MainWindow
		{
			get { return this.mainWindow; }
		}

		public void Dispose() {
            DisposePlayerView();
		}

        public void DisposePlayerView() {
            if (this.playerView != null) {
                this.playerView.Dispose();
                this.playerView = null;
            }
        }
	}
}
