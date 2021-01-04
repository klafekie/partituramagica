#region usings

using System.Windows;

#endregion

namespace PartituraMagica.Lib.Domain
{
	/**
	 * This itfs contains functionality from the main window that
	 * is not part of the DrawingPanel
	 * <seealso cref="IDrawingPanel" />
	 */
	public interface IMainWindow
	{
		string PartPattern { get; set; }

		bool IsEnabled { get; set; }

		Point? CurrentMousePosition { get; set; }
		Point? PreviousMousePosition { get; set; }

		void ShowPartituraInfo(IEditedPartitura active);
		void ClearPartituraInfo();

		void MessageBox(string text);
		void CloseAndExit();
		bool Activate();
		void SignalWait(bool doWait);
	}
}