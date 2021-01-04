#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class ShowLastPageCommand : ShowPageCommandBase, ICommand
	{
		public ShowLastPageCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p, mainWindow)
		{
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPageIndex = P.Active.GetPageCount() - 1;
			ExecuteBase();
		}

		public bool CanExecute()
		{
			return CanExecuteBase() &&
			       P.Active.CurrentPageIndex < P.Active.GetPageCount() - 1;
		}
	}
}