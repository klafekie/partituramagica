#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	public class AutoFillCommand : PartituraCommandBase, ICommand
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IMainWindow _mainWindow;

		public AutoFillCommand(ICommandParameters p, IMainWindow mainWindow, ICommandFactory commandFactory)
			: base(p)
		{
			_mainWindow = mainWindow;
			_commandFactory = commandFactory;
		}

		public bool CanExecute()
		{
			return P.Active.IsOpen && P.Active.GetPageCount() > 0;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			var boxes = new BoxFinder(P.Panel.CurrentImage).Execute();

			foreach (Rect r in boxes)
				_commandFactory.Create<AddBoxCommand>(c => c.Rectangle = r).TryExecute();
		}
	}
}