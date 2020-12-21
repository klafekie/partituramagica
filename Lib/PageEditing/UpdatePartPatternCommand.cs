#region usings

using System.Diagnostics;
using System.Linq;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PageEditing
{
	public class UpdatePartPatternCommand : PartituraCommandBase, ICommand
	{
		public UpdatePartPatternCommand(ICommandParameters p)
			: base(p)
		{
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			string partPattern = P.MainWindow.PartPattern;
			if (partPattern == null || partPattern.Length == 0) partPattern = "A";
			partPattern = partPattern.ToUpper();
			int max = partPattern.ToCharArray().Max() - 'A';
			//foreach (Char i in partPattern) {
			//    max = Math.Max((int)(i - 'A'), max);
			//}

			Partitura pActivePartitura = P.Active.Partitura;
			if (max >= pActivePartitura.GetAllParts(P.Active.GetPageCount()).Count)
			{
				P.MainWindow.MessageBox("You have specified a part that does not exist.");
				//@todo p2 set focus to the TextBox. Problem box.Focus() doesn't do it...
				//can be done in the Validating event?
				P.MainWindow.PartPattern = pActivePartitura.PartPattern;
			}
			else
			{
				pActivePartitura.PartPattern = partPattern;
			}
		}
	}
}