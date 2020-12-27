#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class ShowFirstPageCommand : ShowPageCommandBase, ICommand
	{
		public ShowFirstPageCommand(ICommandParameters p)
			: base(p)
		{
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPageIndex = 0;
			ExecuteBase();
		}

		public bool CanExecute()
		{
			return CanExecuteBase() && P.Active.GetPageCount() > 0;
		}
	}
}