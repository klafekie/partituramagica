using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PartituraMagica
{
	public class UpdatePartPatternCommand : PartituraCommandBase, ICommand
	{		
		public UpdatePartPatternCommand(ICommandParameters p)
			: base(p)
		{			
		}

		public bool CanExecute()
		{
			return p.Active != null && p.Active.IsOpen;
		}

		public void Execute()
		{
			Debug.Assert(this.CanExecute());
			string partPattern = p.MainWindow.PartPattern;
			if (partPattern == null || partPattern.Length == 0) {
				partPattern = "A";
			}
			partPattern = partPattern.ToUpper();
			int max = (int)partPattern.ToCharArray().Max() - 'A';
			//foreach (Char i in partPattern) {
			//    max = Math.Max((int)(i - 'A'), max);
			//}

			Partitura pActivePartitura = p.Active.Partitura;
			if (max >= pActivePartitura.GetAllParts(p.Active.GetPageCount()).Count) {
				p.MainWindow.MessageBox("You have specified a part that does not exist.");				
				//@todo p2 set focus to the TextBox. Problem box.Focus() doesn't do it...
				//can be done in the Validating event?
				p.MainWindow.PartPattern = pActivePartitura.PartPattern;
			} else {
				pActivePartitura.PartPattern = partPattern;
			}
		}
	}
}
