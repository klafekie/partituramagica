#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PageEditing
{
	public class SelectItemCommand : PartituraCommandBase, ICommand
	{
		public SelectItemCommand(ICommandParameters p)
			: base(p)
		{
		}

		public bool CanExecute()
		{
			return P.Active.CurrentPage != null && P.Active.CurrentPage.GetShapeTypeAt(P.MainWindow.CurrentMousePosition) !=
				Shape.NothingSpecial;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPage.SelectBoxAt(P.MainWindow.CurrentMousePosition, P.Panel);
		}
	}
}