#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class NewPartituraCommand : PartituraCommandBase, ICommand
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IMainWindow _mainWindow;

		public NewPartituraCommand(ICommandParameters p, ICommandFactory commandFactory, IMainWindow mainWindow)
			: base(p)
		{
			_commandFactory = commandFactory;
			_mainWindow = mainWindow;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			string path = P.FileSystem.ShowSavePartituraDialog();

			if (path != null)
			{
				_commandFactory.Create<ClosePartituraCommand>().TryExecute();

				if (!P.Active.IsOpen)
				{
					//only continue if previous piece was really closed.				

					Partitura partitura = new Partitura();
					P.Active.Open(path, partitura);

					_commandFactory.Create<AddPagesCommand>().Execute();
				}
			}

			_mainWindow.ShowPartituraInfo(P.Active);
		}


		public bool CanExecute()
		{
			return true;
		}
	}
}