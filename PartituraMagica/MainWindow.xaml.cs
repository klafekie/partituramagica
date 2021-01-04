#region usings

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using PartituraMagica.Lib.Domain;
using ICommand = PartituraMagica.Lib.Domain.ICommand;
using IContainer = Lamar.IContainer;

#endregion

namespace PartituraMagica.App
{
	/** 
   * The application main window
   */
	public partial class MainWindow : Window, IMainWindow
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IContainer _container;
		private readonly ICommandParameters _controller;
		private bool _waiting;

		public static RoutedUICommand AddPage { get; } = new
			RoutedUICommand("Add ...", "AddPages", typeof(MainWindow));

		public static RoutedUICommand StartPlayer { get; } = new
			RoutedUICommand("Start Player", "ShowPlayer", typeof(MainWindow),
				new InputGestureCollection
				{
					new KeyGesture(Key.Space)
				});

		public static RoutedUICommand NewPartitura { get; } = new
			RoutedUICommand("New Partitura", "NewPartitura", typeof(MainWindow),
				new InputGestureCollection
				{
					new KeyGesture(Key.N, ModifierKeys.Control)
				});


		public static RoutedUICommand RemoveBox { get; } = new
			RoutedUICommand("Remove Box", "RemoveBox", typeof(MainWindow),
				new InputGestureCollection
				{
					new KeyGesture(Key.Delete)
				});

		public static RoutedUICommand Label { get; } = new
			RoutedUICommand("Label As New Part", "LabelPart", typeof(MainWindow),
				new InputGestureCollection
				{
					new KeyGesture(Key.L, ModifierKeys.Control)
				});

		public static RoutedUICommand UnLabel { get; } = new
			RoutedUICommand("Remove Label", "UnLabelPart", typeof(MainWindow));

		public static RoutedUICommand AutoFill { get; } = new
			RoutedUICommand("Find Lines", "AutoFill", typeof(MainWindow));

		public static RoutedUICommand Exit { get; } = new
			RoutedUICommand("Exit", "CloseApplication", typeof(MainWindow));

		public static RoutedUICommand ClosePartitura { get; } = new
			RoutedUICommand("Close", "ClosePartitura", typeof(MainWindow));

		public static RoutedUICommand ShowNextPage { get; } = new
			RoutedUICommand("Next Page", "ShowNextPage", typeof(MainWindow));

		public static RoutedUICommand ShowPreviousPage { get; } = new
			RoutedUICommand("Previous Page", "ShowPreviousPage", typeof(MainWindow));

		public static RoutedUICommand TuningTone { get; } = new
			RoutedUICommand("Tuning Tone", "PlayTuningTone", typeof(MainWindow),
				new InputGestureCollection
				{
					new KeyGesture(Key.T, ModifierKeys.Control)
				});


		public MainWindow(IContainer container, ICommandParameters controller, ICommandFactory commandFactory)
		{
			_container = container;
			_controller = controller;

			_commandFactory = commandFactory;
			Trace.WriteLine("hallo listener");

			InitializeComponent();

			_controller.Panel = new DrawingPanel(PageCanvas);
		}

		public Point? PreviousMousePosition { get; set; }
		public Point? CurrentMousePosition { get; set; }

		public string PartPattern
		{
			get => patternBox.Text;
			set => patternBox.Text = value;
		}

		public void ShowPartituraInfo(IEditedPartitura active)
		{
			int pageCount = active.GetPageCount();
			if (pageCount == 0)
				Title = $"Partitura Magica - {Path.GetFileNameWithoutExtension(active.Path)} (no pages)";
			else
				Title =
					$"Partitura Magica - {Path.GetFileNameWithoutExtension(active.Path)} ({active.CurrentPageIndex + 1} / {pageCount})";
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
			Application.Current.Shutdown();
		}


		public void SignalWait(bool doWait)
		{
			_waiting = doWait;
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


		private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			string name = ((RoutedCommand) e.Command).Name;
			e.CanExecute = _commandFactory.Create(name).CanExecute();
		}

		private void Execute(object sender, ExecutedRoutedEventArgs e)
		{
			string name = ((RoutedCommand) e.Command).Name;
			_commandFactory.Create(name).Execute();
		}

		private void PageCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			UpdateMousePosition(e);
			if (!_waiting) SetMouseCursor(CurrentMousePosition.Value);
			if (e.LeftButton == MouseButtonState.Pressed && PreviousMousePosition != null)
				_commandFactory.Create("UpdateBox").TryExecute();
		}

		private void UpdateMousePosition(MouseEventArgs e)
		{
			Point currentPos = e.GetPosition(PageCanvas);
			if (CurrentMousePosition != null && CurrentMousePosition != currentPos)
				PreviousMousePosition = CurrentMousePosition;
			CurrentMousePosition = currentPos;
		}

		private void SetMouseCursor(Point currentPos)
		{
			Cursor cursor;
			Shape type = _controller.GetShapeAt(currentPos);
			switch (type)
			{
				case Shape.BoxContent:
					cursor = Cursors.Hand;
					break;
				case Shape.BoxLeftTopCorner:
				case Shape.BoxRightBottomCorner:
					cursor = Cursors.SizeNWSE;
					break;
				case Shape.BoxTopMargin:
				case Shape.BoxBottomMargin:
					cursor = Cursors.SizeNS;
					break;
				case Shape.BoxRightTopCorner:
				case Shape.BoxLeftBottomCorner:
					cursor = Cursors.SizeNESW;
					break;
				case Shape.BoxRightMargin:
				case Shape.BoxLeftMargin:
				case Shape.BoxSplitter:
					cursor = Cursors.SizeWE;
					break;
				case Shape.NothingSpecial:
					cursor = Cursors.Arrow;
					break;
				default:
					throw new ApplicationException("unexpected ShapeType");
			}

			Mouse.SetCursor(cursor);
		}

		private void PageCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			UpdateMousePosition(e);

			ICommand cmd;
			if (e.ClickCount == 1)
			{
				cmd = _commandFactory.Create("SelectBox");
				if (!cmd.CanExecute())
				{
					cmd = _commandFactory.Create("AddBox");
					if (!cmd.CanExecute())
						return;
				}
			}
			else
			{
				cmd = _commandFactory.Create("SplitBox");
				if (!cmd.CanExecute()) return;
			}

			cmd.Execute();
		}

		private void WindowClosing(object sender, CancelEventArgs e)
		{
			//caution: this event is fired twice during shutdown.			
			//first when window is closed by user,
			//this leads to a call to MainWindow.CloseAndExit() which fires
			//this event again.
			ICommand cmd = _commandFactory.Create("CloseApplication");

			//Cancel closing if the piece is still open
			e.Cancel = cmd.TryExecute();
		}

		private void patternBox_LostFocus(object sender, RoutedEventArgs e)
		{
			//this.controller.Create("UpdatePartPattern").TryExecute();
		}

		private void patternBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			_commandFactory.Create("UpdatePartPattern").TryExecute();
		}

		private void patternBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				if (_commandFactory.Create("UpdatePartPattern").TryExecute())
					_commandFactory.Create("ShowPlayer").TryExecute();
				e.Handled = true;
			}
		}
	}
}