#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	public class SelectBoxCommand : PartituraCommandBase, ICommand
	{
		private readonly IMainWindow _mainWindow;

		public SelectBoxCommand(ICommandParameters p, IMainWindow mainWindow)
			: base(p)
		{
			_mainWindow = mainWindow;
		}

		public bool CanExecute()
		{
			return P.Active.CurrentPage != null
			       && _mainWindow.CurrentMousePosition != null
			       && P.Active.CurrentPage.GetShapeTypeAt(_mainWindow.CurrentMousePosition.Value) !=
			       Shape.NothingSpecial;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			P.Active.CurrentPage.SelectBoxAt(_mainWindow.CurrentMousePosition.Value, P.Panel);
		}
	}
}