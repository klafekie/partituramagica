#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class SaveAsCommand : PartituraCommandBase, ICommand
	{
		public SaveAsCommand(ICommandParameters p)
			: base(p)
		{
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			string path = P.FileSystem.ShowSavePartituraDialog();

			if (path != null) P.Active.Save(path);
			P.MainWindow.ShowPartituraInfo(P.Active);
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen;
		}
	}
}