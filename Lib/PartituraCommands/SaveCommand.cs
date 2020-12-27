#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class SaveCommand : PartituraCommandBase, ICommand
	{
		public SaveCommand(ICommandParameters p)
			: base(p)
		{
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			string path = P.Active.Path;

			if (path != null) P.Active.Save(path);
			P.MainWindow.ShowPartituraInfo(P.Active);
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen
			                        && P.Active.Partitura.Dirty;
		}
	}
}