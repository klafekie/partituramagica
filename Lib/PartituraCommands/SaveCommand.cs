#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class SaveCommand : PartituraCommandBase, ICommand
	{
		private readonly IMainWindow _mainWindow;

		public SaveCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p)
		{
			_mainWindow = mainWindow;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			string path = P.Active.Path;

			if (path != null) P.Active.Save(path);
			_mainWindow.ShowPartituraInfo(P.Active);
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen
			                        && P.Active.Partitura.Dirty;
		}
	}
}