#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class ShowPageCommand : ShowPageCommandBase, ICommand
	{
		public int Index { get; set; }

		public ShowPageCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p, mainWindow)
		{
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPageIndex = Index;
			ExecuteBase();
		}

		public bool CanExecute()
		{
			return CanExecuteBase() &&
			       (Index >= 0 ||
			        Index < P.Active.GetPageCount());
		}
	}
}