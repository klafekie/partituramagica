#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PageEditing
{
	/**
     * Split the currently selected box vertically.
     * If current position is inside the selected box, the vertical splitter is inserted
     * at that position. Otherwise it is inserted in the middle of the selected box, or as near
     * as possible to the middle if the middle is already occupied by a splitter.
     */
	public class SplitBoxCommand : PartituraCommandBase, ICommand
	{
		public SplitBoxCommand(ICommandParameters p)
			: base(p)
		{
		}

		public bool CanExecute()
		{
			return P.Active.CurrentPage != null && P.Active.CurrentPage.IsBoxSelected();
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPage.SplitSelectedBox(P.MainWindow.CurrentMousePosition, P.Panel);
		}
	}
}