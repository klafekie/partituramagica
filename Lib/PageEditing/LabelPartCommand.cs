using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PartituraMagica
{
	public class LabelPartCommand : PartituraCommandBase, ICommand
	{
		public LabelPartCommand(ICommandParameters p)
			: base(p)
		{
		}

		public bool CanExecute()
		{
			if (p.Active == null || !p.Active.IsOpen) return false;
			if (p.Active.CurrentPageIndex == 0 && p.Active.CurrentPage.IsLeftTopSelected()) {
				//the implicit 'A' cannot be changed
				return false;
			}
			return p.Active.CurrentPage.IsBoxSelected() && !p.Active.CurrentPage.IsSelectionLabelled();
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
				
			p.Active.Partitura.LabelSelection(p.Panel, 
				p.Active.CurrentPageIndex);
		}
	}
}
