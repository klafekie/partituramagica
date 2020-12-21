#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class SaveCommand : PartituraCommandBase, ICommand
	{
		private readonly bool _saveAs;

		public SaveCommand(bool saveAs, ICommandParameters p)
			: base(p)
		{
			_saveAs = saveAs;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			string path = P.Active.Path;

			if (_saveAs || path == null) path = P.FileSystem.ShowSavePartituraDialog();

			if (path != null) P.Active.Save(path);
			P.MainWindow.ShowPartituraInfo(P.Active);
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen
			                        && (_saveAs || P.Active.Partitura.Dirty);
		}
	}
}