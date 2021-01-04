#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class ShowNextPageCommand : ShowPageCommandBase, ICommand
	{
		public ShowNextPageCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p, mainWindow)
		{
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPageIndex += 1;
			ExecuteBase();
		}

		public bool CanExecute()
		{
			return CanExecuteBase() &&
			       P.Active.CurrentPageIndex < P.Active.GetPageCount() - 1;
		}
	}
}