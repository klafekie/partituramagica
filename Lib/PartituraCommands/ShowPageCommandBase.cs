#region usings

using System.Windows;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class ShowPageCommandBase : PartituraCommandBase
	{
		protected ShowPageCommandBase(ICommandParameters p) : base(p)
		{
		}

		protected void ExecuteBase()
		{
			P.Panel.Clear();
			Point dimensions = P.Panel.DrawImage(P.Active.CurrentImagePath);

			P.Active.Partitura.Paint(P.Active.CurrentPageIndex, P.Panel, dimensions);
			P.MainWindow.ShowPartituraInfo(P.Active);
		}

		protected bool CanExecuteBase()
		{
			return P.Active != null && P.Active.IsOpen;
		}
	}
}