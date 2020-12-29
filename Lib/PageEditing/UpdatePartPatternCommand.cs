#region usings

using System.Diagnostics;
using System.Linq;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PageEditing
{
	public class UpdatePartPatternCommand : PartituraCommandBase, ICommand
	{
		private readonly IMainWindow _mainWindow;

		public UpdatePartPatternCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p)
		{
			_mainWindow = mainWindow;
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			string partPattern = _mainWindow.PartPattern;
			if (string.IsNullOrEmpty(partPattern)) partPattern = "A";
			partPattern = partPattern.ToUpper();
			int max = partPattern.ToCharArray().Max() - 'A';

			Partitura pActivePartitura = P.Active.Partitura;
			if (max >= pActivePartitura.GetAllParts(P.Active.GetPageCount()).Count)
			{
				_mainWindow.MessageBox("You have specified a part that does not exist.");
				_mainWindow.PartPattern = pActivePartitura.PartPattern;
			}
			else
			{
				pActivePartitura.PartPattern = partPattern;
			}
		}
	}
}