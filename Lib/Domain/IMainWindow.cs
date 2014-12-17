using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{
	/**
	 * This itfs contains functionality from the main window that
	 * is not part of the DrawingPanel
	 * <seealso cref="IDrawingPanel"/>
	 */
	public interface IMainWindow
	{
		string PartPattern { get; set; }

		void MessageBox(string text);
		void CloseAndExit();

		bool IsEnabled { get; set; }
		bool Activate();
        void SignalWait(bool doWait);
	}
}
