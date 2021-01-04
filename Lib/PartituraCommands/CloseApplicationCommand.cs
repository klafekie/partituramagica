#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class CloseApplicationCommand : PartituraCommandBase, ICommand
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IMainWindow _mainWindow;

		public CloseApplicationCommand(ICommandParameters p, ICommandFactory commandFactory, IMainWindow mainWindow)
			: base(p)
		{
			_commandFactory = commandFactory;
			_mainWindow = mainWindow;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			if (!P.Active.IsOpen ||
			    _commandFactory.Create<ClosePartituraCommand>().TryExecute()) _mainWindow.CloseAndExit();
		}

		public bool CanExecute()
		{
			return true;
		}
	}
}