#region usings

using System.Diagnostics;
using Lamar;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class ShowPageCommand : ShowPageCommandBase, ICommand
	{
		private readonly int _index;

		[DefaultConstructor]
		public ShowPageCommand(ICommandParameters p)
			: base(p)
		{
		}

		public ShowPageCommand(int index, ICommandParameters p)
			: base(p)
		{
			_index = index;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPageIndex = _index;
			ExecuteBase();
		}

		public bool CanExecute()
		{
			return CanExecuteBase() &&
			       (_index >= 0 ||
			        _index < P.Active.GetPageCount());
		}
	}
}