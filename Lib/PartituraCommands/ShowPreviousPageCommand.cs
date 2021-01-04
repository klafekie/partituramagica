#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class ShowPreviousPageCommand : ShowPageCommandBase, ICommand
	{
		public ShowPreviousPageCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p, mainWindow)
		{
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPageIndex -= 1;
			ExecuteBase();
		}

		public bool CanExecute()
		{
			return CanExecuteBase() &&
			       P.Active.CurrentPageIndex > 0;
		}
	}
}