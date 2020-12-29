#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PageEditing
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
			if (current == null || !current.IsBoxSelected() || _mainWindow.CurrentMousePosition == null ||
			    _mainWindow.PreviousMousePosition == null) return false;
			return current.CanUpdateSelectedBox(_mainWindow.PreviousMousePosition.Value,
				_mainWindow.CurrentMousePosition.Value);
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPage.UpdateSelectedBox(_mainWindow.PreviousMousePosition.Value,
				_mainWindow.CurrentMousePosition.Value,
				P.Panel);
		}
	}
}