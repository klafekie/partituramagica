#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	public class UnLabelPartCommand : PartituraCommandBase, ICommand
	{
		public UnLabelPartCommand(ICommandParameters p)
			: base(p)
		{
		}

		public bool CanExecute()
		{
			if (P.Active == null || !P.Active.IsOpen) return false;
			if (P.Active.CurrentPageIndex == 0 && P.Active.CurrentPage.IsLeftTopSelected())
				//the implicit 'A' cannot be removed
				return false;
			return P.Active.CurrentPage.IsBoxSelected() && P.Active.CurrentPage.IsSelectionLabelled();
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.Partitura.UnLabelSelection(P.Panel, P.Active.CurrentPageIndex);
		}
	}
}