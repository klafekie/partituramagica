#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	/**
     * Split the currently selected box vertically.
     * If current position is inside the selected box, the vertical splitter is inserted
     * at that position. Otherwise it is inserted in the middle of the selected box, or as near
     * as possible to the middle if the middle is already occupied by a splitter.
     */
	public class SplitBoxCommand : PartituraCommandBase, ICommand
	{
		private readonly IMainWindow _mainWindow;

		public SplitBoxCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p)
		{
			_mainWindow = mainWindow;
		}

		public bool CanExecute()
		{
			return P.Active.CurrentPage != null && P.Active.CurrentPage.IsBoxSelected()
			                                    && _mainWindow.CurrentMousePosition != null;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPage.SplitSelectedBox(_mainWindow.CurrentMousePosition.Value, P.Panel);
		}
	}
}