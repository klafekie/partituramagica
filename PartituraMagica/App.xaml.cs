using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace PartituraMagica
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Trace.TraceError("Unhandled exception in application:");
            Trace.TraceError(e.Exception.Message + "\n" + e.Exception.StackTrace);
            MessageBox.Show("An error in the application has occurred and we are sorry for your inconvenience.\n\n" +
                            e.Exception.Message +
                            "\n\nPlease see ReleaseNotes.doc for troubleshooting hints and restart the application.");
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Trace.WriteLine("");
            Trace.WriteLine("-------------------");
            Trace.TraceInformation("-- Application started --");
            Trace.WriteLine("-------------------");
        }
    }
}