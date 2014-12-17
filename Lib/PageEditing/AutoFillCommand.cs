using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;


namespace PartituraMagica
{
	public class AutoFillCommand : PartituraCommandBase, ICommand
	{
		public AutoFillCommand(ICommandParameters p)
			: base(p)
		{

		}

		public bool CanExecute()
		{
			return p.Active.IsOpen;
		}


		public void Execute()
		{
			Debug.Assert(CanExecute());
			this.p.MainWindow.SignalWait(true);
			BoxFinder b = new BoxFinder(p.Panel.CurrentImage);
			foreach (Rect r in b.Execute()) {
				new AddBoxCommand(r, this.p).TryExecute();
			}
			this.p.MainWindow.SignalWait(false);
		}
	}
}
