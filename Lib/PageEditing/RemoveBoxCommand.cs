#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	/**
	 * removes the currently selected box.
	 */
	public class RemoveBoxCommand : PartituraCommandBase, ICommand
	{
		public RemoveBoxCommand(ICommandParameters p)
			: base(p)
		{
		}

		public bool CanExecute()
		{
			return P.Active.CurrentPage != null && P.Active.CurrentPage.IsAnyBoxSelected();
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.Partitura.RemoveSelectedBox(P.Active.CurrentPageIndex, P.Panel);
		}
	}
}