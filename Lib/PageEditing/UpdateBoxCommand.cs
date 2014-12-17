using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PartituraMagica
{
	public class UpdateBoxCommand : PartituraCommandBase, ICommand
	{
		private Point lastPos;
		private Point newPos;

		public UpdateBoxCommand(Point lastPos, Point newPos, ICommandParameters p)
			: base(p) {
			this.lastPos = lastPos;
			this.newPos = newPos;
		}

		public bool CanExecute() {
			Page current = p.Active.CurrentPage;
			if (current == null || !current.IsBoxSelected()) {
				return false;
			}
			return current.CanUpdateSelectedBox(lastPos, newPos);
		}

		public void Execute() {
			Debug.Assert(CanExecute());
			p.Active.CurrentPage.UpdateSelectedBox(lastPos, newPos, p.Panel);
		}

	}
}
