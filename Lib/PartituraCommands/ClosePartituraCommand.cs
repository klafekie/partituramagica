#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class ClosePartituraCommand : PartituraCommandBase, ICommand
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IMainWindow _mainWindow;

		public ClosePartituraCommand(ICommandParameters p, IMainWindow mainWindow, ICommandFactory commandFactory)
			: base(p)
		{
			_mainWindow = mainWindow;
			_commandFactory = commandFactory;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			DialogResult c = DialogResult.No;
			ICommand cmd = _commandFactory.Create<SaveCommand>();
			if (cmd.CanExecute())
			{
				c = P.FileSystem.AskToSave();
				if (c == DialogResult.Cancel) return;
				if (c == DialogResult.Yes) cmd.Execute();
			}

			if (P.Active.IsOpen && (!P.Active.Partitura.Dirty || c == DialogResult.No))
			{
				//file was really safed or user doesn't care
				P.Panel.Clear();
				P.Active.Close();
				P.DisposePlayerView();
				_mainWindow.ClearPartituraInfo();
				_mainWindow.PartPattern = string.Empty;
			}
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen;
		}
	}
}