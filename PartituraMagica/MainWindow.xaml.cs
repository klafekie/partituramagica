using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PartituraMagica
{
	/** 
	 * The application main window
	 */
	public partial class MainWindow : Window, IMainWindow
	{
		private EditingController controller;
		private Point lastClickPos;

		public MainWindow() {
			Trace.WriteLine("hallo listener");
			
			InitializeComponent();
			
			controller = new EditingController(this, new DrawingPanel(PageCanvas));
		}

		private static RoutedUICommand addPage = new
			RoutedUICommand("Add ...", "AddPage", typeof(MainWindow));

		public static RoutedUICommand AddPage {
			get { return addPage; }
		}

		private static RoutedUICommand startPlayer = new
			RoutedUICommand("Start Player", "StartPlayer", typeof(MainWindow),
			new InputGestureCollection() {
				new KeyGesture(Key.Space)
			});

		public static RoutedUICommand StartPlayer {
			get { return startPlayer; }
		}

		private static RoutedUICommand label = new
			RoutedUICommand("Label As New Part", "Label", typeof(MainWindow),
			new InputGestureCollection() {
				new KeyGesture(Key.L, ModifierKeys.Control)
			});

		public static RoutedUICommand Label {
			get { return label; }
		}

		private static RoutedUICommand unLabel = new
			RoutedUICommand("Remove Label", "UnLabel", typeof(MainWindow));

		public static RoutedUICommand UnLabel {
			get { return unLabel; }
		}

		private static RoutedUICommand autoFill = new
			RoutedUICommand("Find Lines", "AutoFill", typeof(MainWindow));

		public static RoutedUICommand AutoFill {
			get { return autoFill; }
		}

		private static RoutedUICommand exit = new
			RoutedUICommand("Exit", "CloseApplication", typeof(MainWindow));

		public static RoutedUICommand Exit {
			get { return exit; }
		}


		private static RoutedUICommand closePartitura = new
			RoutedUICommand("Close", "ClosePartitura", typeof(MainWindow));

		public static RoutedUICommand ClosePartitura {
			get { return closePartitura; }
		}


		void CanExecute(object sender, CanExecuteRoutedEventArgs e) {
			string name = ((RoutedCommand)e.Command).Name;
			e.CanExecute = controller.CanExecuteCommand(name);
		}

		void Execute(object sender, ExecutedRoutedEventArgs e) {
			string name = ((RoutedCommand)e.Command).Name;
			controller.CreateCommand(name).Execute();
		}

		private void PageCanvas_MouseMove(object sender, MouseEventArgs e) {
			Point currentPos = e.GetPosition(PageCanvas);
			SetMouseCursor(currentPos);
			controller.CreateMouseMoveCommand(
					this.lastClickPos, currentPos,
					e.LeftButton == MouseButtonState.Pressed).TryExecute();

			this.lastClickPos = currentPos;
		}

		private void SetMouseCursor(Point currentPos) {
			Cursor cursor = Cursors.Arrow;
			Shape type = controller.GetShapeAt(currentPos);
			switch (type) {
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

		private void PageCanvas_MouseDown(object sender, MouseButtonEventArgs e) {
			Point pos = e.GetPosition(PageCanvas);
			this.lastClickPos = pos;
			controller.CreateMouseDownCommand(pos, e.ClickCount).TryExecute();
		}

		private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			//caution: this event is fired twice during shutdown.			
			//first when window is closed by user,
			//this leads to a call to MainWindow.CloseAndExit() which fires
			//this event again.
			ICommand cmd = controller.CreateCommand("CloseApplication");

			//Cancel closing if the piece is still open
			e.Cancel = cmd.TryExecute();
		}

		public string PartPattern {
			get { return this.patternBox.Text; }
			set {
				this.patternBox.Text = value;
			}
		}

		private void patternBox_LostFocus(object sender, RoutedEventArgs e) {
			//this.controller.CreateCommand("UpdatePartPattern").TryExecute();
		}


		public void MessageBox(string text) {
			System.Windows.MessageBox.Show(text);
		}


		public void CloseAndExit() {
			this.controller.Dispose();
			App.Current.Shutdown();
		}


		public void SignalWait(bool doWait) {
			this.Cursor = doWait ? Cursors.Wait : Cursors.Arrow;
		}

		public new bool IsEnabled {
			get { return base.IsEnabled; }
			set {
				base.IsEnabled = value;
				if (value) {
					//if we don't do this, main window won't have 
					//keyboard focus after the playwindow is hidden
					this.Focus();
				}
			}
		}

		private void patternBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {			
			this.controller.CreateCommand("UpdatePartPattern").TryExecute();
		}

		private void patternBox_PreviewKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Space) {
				if (this.controller.CreateCommand("UpdatePartPattern").TryExecute()) {
					this.controller.CreateCommand("StartPlayer").TryExecute();
				}
				e.Handled = true;
			}
		}
	}
}
