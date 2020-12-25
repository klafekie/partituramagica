#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PageEditing
{
	public class UpdateBoxCommand : PartituraCommandBase, ICommand
	{
		public UpdateBoxCommand(ICommandParameters p)
			: base(p)
		{
		}

		public bool CanExecute()
		{
			Page current = P.Active.CurrentPage;
			if (current == null || !current.IsBoxSelected()) return false;
			return current.CanUpdateSelectedBox(P.MainWindow.PreviousMousePosition, P.MainWindow.CurrentMousePosition);
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPage.UpdateSelectedBox(P.MainWindow.PreviousMousePosition, P.MainWindow.CurrentMousePosition,
				P.Panel);
		}
	}
}