#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class ShowNextPageCommand : ShowPageCommandBase, ICommand
	{
		public ShowNextPageCommand(ICommandParameters p)
			: base(p)
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