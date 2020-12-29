#region usings

using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PageEditing
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
			_mainWindow.SignalWait(true);
			BoxFinder b = new BoxFinder(P.Panel.CurrentImage);

			foreach (Rect r in b.Execute())
				_commandFactory.Create<AddBoxCommand>(c => c.Rectangle = r).TryExecute();
			_mainWindow.SignalWait(false);
		}
	}
}