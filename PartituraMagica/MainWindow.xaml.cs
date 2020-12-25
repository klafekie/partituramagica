#region usings

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using PartituraMagica.Domain;
using ICommand = PartituraMagica.Domain.ICommand;
using IContainer = Lamar.IContainer;

#endregion

namespace PartituraMagica
{
	/** 
     * The application main window
     */
	public partial class MainWindow : Window, IMainWindow
	{
		private readonly IContainer _container;
		private Point _lastClickPos;

		public static RoutedUICommand AddPage { get; } = new
			RoutedUICommand("Add ...", "AddPage", typeof(MainWindow));

		public static RoutedUICommand StartPlayer { get; } = new
			RoutedUICommand("Start Player", "StartPlayer", typeof(MainWindow),
				new InputGestureCollection
				{
					new KeyGesture(Key.Space)
				});

		public static RoutedUICommand Label { get; } = new
			RoutedUICommand("Label As New Part", "Label", typeof(MainWindow),
				new InputGestureCollection
				{
					new KeyGesture(Key.L, ModifierKeys.Control)
				});

		public static RoutedUICommand UnLabel { get; } = new
			RoutedUICommand("Remove Label", "UnLabel", typeof(MainWindow));

		public static RoutedUICommand AutoFill { get; } = new
			RoutedUICommand("Find Lines", "AutoFill", typeof(MainWindow));

		public static RoutedUICommand Exit { get; } = new
			RoutedUICommand("Exit", "CloseApplication", typeof(MainWindow));

		public static RoutedUICommand ClosePartitura { get; } = new
			RoutedUICommand("Close", "ClosePartitura", typeof(MainWindow));

		public static RoutedUICommand TuningTone { get; } = new
			RoutedUICommand("Tuning Tone", "TuningTone", typeof(MainWindow),
				new InputGestureCollection
				{
					new KeyGesture(Key.T, ModifierKeys.Control)
				});

		public Point PreviousMousePosition { get; set; }
		public MouseButtonState CurrentMouseButtonState { get; set; }

		public MainWindow(IContainer container)
		{
			_container = container;
			Trace.WriteLine("hallo listener");

			InitializeComponent();
		}


		public string PartPattern
		{
			get => patternBox.Text;
			set => patternBox.Text = value;
		}

		public void ShowPartituraInfo(IActivePartitura active)
		{
			var pageCount = active.GetPageCount();
			if (pageCount == 0)
				Title = string.Format("Partitura Magica - {0} (no pages)",
					Path.GetFileNameWithoutExtension(active.Path));
			else
				Title = string.Format("Partitura Magica - {0} ({1} / {2})",
					Path.GetFileNameWithoutExtension(active.Path),
					active.CurrentPageIndex + 1,
					pageCount);
		}

		public void ClearPartituraInfo()
		{
			Title = "Partitura Magica";
		}

		public void MessageBox(string text)
		{
			System.Windows.MessageBox.Show(text);
		}


		public void CloseAndExit()
		{
			//TODO disppose controller?
			//_controller.Dispose();
			Application.Current.Shutdown();
		}


		public void SignalWait(bool doWait)
		{
			Cursor = doWait ? Cursors.Wait : Cursors.Arrow;
		}

		public new bool IsEnabled
		{
			get => base.IsEnabled;
			set
			{
				base.IsEnabled = value;
				if (value)
					//if we don't do this, main window won't have 
					//keyboard focus after the playwindow is hidden
					Focus();
			}
		}

		public Point CurrentMousePosition { get; set; }

		private ICommand CreateCommand(string name)
		{
			return _container.GetInstance<ICommand>(name);
		}

		private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			string name = ((RoutedCommand) e.Command).Name;
			e.CanExecute = CreateCommand(name).CanExecute();
		}

		private void Execute(object sender, ExecutedRoutedEventArgs e)
		{
			string name = ((RoutedCommand) e.Command).Name;
			CreateCommand(name).Execute();
		}

		private void PageCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			Point currentPos = e.GetPosition(PageCanvas);
			SetMouseCursor(currentPos);
			PreviousMousePosition = CurrentMousePosition;
			CurrentMousePosition = currentPos;
			CurrentMouseButtonState = e.LeftButton;
		}

		private void SetMouseCursor(Point currentPos)
		{
			Cursor cursor = Cursors.Arrow;
			//TODO
			//Shape type = _controller.GetShapeAt(currentPos);
			//switch (type)
			//{
			//	case Shape.BoxContent:
			//		cursor = Cursors.Hand;
			//		break;
			//	case Shape.BoxLeftTopCorner:
			//	case Shape.BoxRightBottomCorner:
			//		cursor = Cursors.SizeNWSE;
			//		break;
			//	case Shape.BoxTopMargin:
			//	case Shape.BoxBottomMargin:
			//		cursor = Cursors.SizeNS;
			//		break;
			//	case Shape.BoxRightTopCorner:
			//	case Shape.BoxLeftBottomCorner:
			//		cursor = Cursors.SizeNESW;
			//		break;
			//	case Shape.BoxRightMargin:
			//	case Shape.BoxLeftMargin:
			//	case Shape.BoxSplitter:
			//		cursor = Cursors.SizeWE;
			//		break;
			//	case Shape.NothingSpecial:
			//		cursor = Cursors.Arrow;
			//		break;
			//	default:
			//		throw new ApplicationException("unexpected ShapeType");
			//}

			Mouse.SetCursor(cursor);
		}

		private void PageCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			//Point pos = e.GetPosition(PageCanvas);
			//_lastClickPos = pos;
			//_controller.CreateMouseDownCommand(pos, e.ClickCount).TryExecute();
		}

		private void WindowClosing(object sender, CancelEventArgs e)
		{
			//caution: this event is fired twice during shutdown.			
			//first when window is closed by user,
			//this leads to a call to MainWindow.CloseAndExit() which fires
			//this event again.
			ICommand cmd = CreateCommand("CloseApplication");

			//Cancel closing if the piece is still open
			e.Cancel = cmd.TryExecute();
		}

		private void patternBox_LostFocus(object sender, RoutedEventArgs e)
		{
			//this.controller.CreateCommand("UpdatePartPattern").TryExecute();
		}

		private void patternBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			CreateCommand("UpdatePartPattern").TryExecute();
		}

		private void patternBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				if (CreateCommand("UpdatePartPattern").TryExecute())
					CreateCommand("StartPlayer").TryExecute();
				e.Handled = true;
			}
		}
	}
}