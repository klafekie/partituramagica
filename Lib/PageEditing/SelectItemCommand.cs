using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;

namespace PartituraMagica
{
	public class SelectItemCommand : PartituraCommandBase, ICommand
	{
		protected Point pos;

		public bool CanExecute()
		{
			return p.Active.CurrentPage != null && p.Active.CurrentPage.GetShapeTypeAt(pos) != Shape.NothingSpecial;
		}

		public SelectItemCommand(Point pos, ICommandParameters p)
			: base(p)
		{
			this.pos = pos;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			p.Active.CurrentPage.SelectBoxAt(pos, p.Panel);
		}
	}
}