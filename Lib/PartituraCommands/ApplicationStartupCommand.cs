#region usings

using System;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class ApplicationStartupCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly ICommandFactory _commandFactory;

		public ApplicationStartupCommand(ICommandFactory commandFactory, IApplicationConfiguration applicationConfiguration)
		{
			_commandFactory = commandFactory;
			_applicationConfiguration = applicationConfiguration;
		}

		public void Execute()
		{
			string[] args = Environment.GetCommandLineArgs();
			string path = _applicationConfiguration.LastPartituraPath;
			if (args.Length == 2)
				//first arg is the exe name
				path = args[1];

			_commandFactory.Create<OpenCommand>(c => c.Path = path).TryExecute();
		}

		public bool CanExecute()
		{
			return true;
		}
	}
}