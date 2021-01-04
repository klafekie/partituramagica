#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class SaveAsCommand : PartituraCommandBase, ICommand
	{
		private readonly IMainWindow _mainWindow;

		public SaveAsCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p)
		{
			_mainWindow = mainWindow;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			string path = P.FileSystem.ShowSavePartituraDialog();

			if (path != null) P.Active.Save(path);
			_mainWindow.ShowPartituraInfo(P.Active);
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen;
		}
	}
}