#region usings

using System.Text.Json;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class OpenCommand : PartituraCommandBase, ICommand
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IMainWindow _mainWindow;
		public string Path { get; set; }

		public OpenCommand(ICommandParameters p, IMainWindow mainWindow, ICommandFactory commandFactory)
			: base(p)
		{
			_mainWindow = mainWindow;
			_commandFactory = commandFactory;
		}

		public void Execute()
		{
			string chosenFile = Path ?? P.FileSystem.ShowOpenPartituraDialog();
			if (chosenFile != null)
			{
				_commandFactory.Create<ClosePartituraCommand>().TryExecute();
				if (!P.Active.IsOpen)
				{
					string aString = P.FileSystem.ReadFile(chosenFile);
					PartituraJsonMemento mem = JsonSerializer.Deserialize<PartituraJsonMemento>(aString);
					Partitura partitura = new Partitura(mem);
					P.Active.Open(chosenFile, partitura);

					_commandFactory.Create<ShowFirstPageCommand>().TryExecute();
					_mainWindow.PartPattern = P.Active.Partitura.PartPattern;
					_mainWindow.ShowPartituraInfo(P.Active);
				}
			}
		}

		public bool CanExecute()
		{
			return true;
		}
	}
}