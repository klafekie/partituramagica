#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	public class UpdateBoxCommand : PartituraCommandBase, ICommand
	{
		private readonly IMainWindow _mainWindow;

		public UpdateBoxCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p)
		{
			_mainWindow = mainWindow;
		}

		public bool CanExecute()
		{
			Page current = P.Active.CurrentPage;
			if (current == null || !current.IsAnyBoxSelected() || _mainWindow.CurrentMousePosition == null ||
			    _mainWindow.PreviousMousePosition == null ||
			    _mainWindow.CurrentMouseShape == Shape.NothingSpecial) return false;
			return current.CanUpdateSelectedBox(_mainWindow.PreviousMousePosition.Value,
				_mainWindow.CurrentMousePosition.Value, _mainWindow.CurrentMouseShape);
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPage.UpdateSelectedBox(_mainWindow.PreviousMousePosition.Value,
				_mainWindow.CurrentMousePosition.Value,
				_mainWindow.CurrentMouseShape,
				P.Panel);
		}
	}
}