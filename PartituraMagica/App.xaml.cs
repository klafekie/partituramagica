using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace PartituraMagica
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
			Trace.TraceError("Unhandled exception in application:");
			Trace.TraceError(e.Exception.Message + "\n" + e.Exception.StackTrace);
			MessageBox.Show("An error in the application has occurred and we are sorry for your inconvenience.\n\n" +
				e.Exception.Message + "\n\nPlease see ReleaseNotes.doc for troubleshooting hints and restart the application.");
		}

		private void Application_Startup(object sender, StartupEventArgs e) {
			Trace.WriteLine("");
			Trace.WriteLine("-------------------");
			Trace.TraceInformation("-- Application started --");
			Trace.WriteLine("-------------------");
			
		}
	}
}
