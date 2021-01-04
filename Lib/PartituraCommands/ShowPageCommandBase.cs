#region usings

using System.Windows;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class ShowPageCommandBase : PartituraCommandBase
	{
		private readonly IMainWindow _mainWindow;

		protected ShowPageCommandBase(ICommandParameters p, IMainWindow mainWindow) : base(p)
		{
			_mainWindow = mainWindow;
		}

		protected void ExecuteBase()
		{
			P.Panel.Clear();
			Point dimensions = P.Panel.DrawImage(P.Active.CurrentImagePath);

			P.Active.Partitura.Paint(P.Active.CurrentPageIndex, P.Panel, dimensions);
			_mainWindow.ShowPartituraInfo(P.Active);
		}

		protected bool CanExecuteBase()
		{
			return P.Active != null && P.Active.IsOpen;
		}
	}
}