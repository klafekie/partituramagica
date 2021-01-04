#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	public class AddBoxCommand : PartituraCommandBase, ICommand
	{
		private readonly IMainWindow _mainWindow;
		private Rect _rectangle;

		public Rect Rectangle
		{
			get => _rectangle ?? CreateRectangleAtMouse();
			set => _rectangle = value;
		}

		public AddBoxCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p)
		{
			_mainWindow = mainWindow;
		}

		public bool CanExecute()
		{
			return P.Active?.CurrentPage != null &&
			       Rectangle?.Width > 0 &&
			       !P.Active.CurrentPage.Overlaps(Rectangle);
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			//Trace.TraceInformation("Adding Box at " + r);
			P.Active.Partitura.AddBox(P.Active.CurrentPageIndex,
				Rectangle, P.Panel);
		}

		private Rect CreateRectangleAtMouse()
		{
			if (_mainWindow.CurrentMousePosition == null ||
			    _mainWindow.CurrentMousePosition.Value.X < 0D ||
			    _mainWindow.CurrentMousePosition.Value.Y < 0D) return new Rect(0, 0, 0, 0);

			double defaultWidth = 2 * Page.ClickTolerance + 1;
			double defaultHeight = defaultWidth;
			Rect r = new Rect(_mainWindow.CurrentMousePosition.Value.X - defaultWidth,
				_mainWindow.CurrentMousePosition.Value.Y - defaultHeight,
				defaultWidth, defaultHeight);
			return r;
		}
	}
}