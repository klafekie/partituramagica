#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	public class LabelPartCommand : PartituraCommandBase, ICommand
	{
		public LabelPartCommand(ICommandParameters p)
			: base(p)
		{
		}

		public bool CanExecute()
		{
			if (P.Active == null || !P.Active.IsOpen) return false;
			if (P.Active.CurrentPageIndex == 0 && P.Active.CurrentPage.IsLeftTopSelected())
				//the implicit 'A' cannot be changed
				return false;
			return P.Active.CurrentPage.IsBoxSelected() && !P.Active.CurrentPage.IsSelectionLabelled();
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());

			P.Active.Partitura.LabelSelection(P.Panel,
				P.Active.CurrentPageIndex);
		}
	}
}