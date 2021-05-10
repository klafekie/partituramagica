#region usings

using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Lamar;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.PartituraCommands;

#endregion

namespace PartituraMagica.App
{
	/// <summary>
	///   Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private Container _container;

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Trace.TraceError("Unhandled exception in application:");
			Trace.TraceError(e.Exception.Message + "\n" + e.Exception.StackTrace);
			MessageBox.Show("An error in the application has occurred and we are sorry for your inconvenience.\n\n" +
			                e.Exception.Message +
			                "\n\nPlease see ReleaseNotes.doc for troubleshooting hints and restart the application.");
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Trace.TraceInformation("disposing DI container");
			_container?.Dispose(); //this will dispose all disposable objects created by container
		}


		private void Application_Startup(object sender, StartupEventArgs e)
		{
			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

			Trace.WriteLine("");
			Trace.WriteLine("-------------------");
			Trace.TraceInformation("-- Application started --");
			Trace.WriteLine("-------------------");

			Thread.CurrentThread.CurrentCulture =
				CultureInfo.GetCultureInfo("en-GB");
			Thread.CurrentThread.CurrentUICulture =
				CultureInfo.GetCultureInfo("en-GB");

			_container = new Container(new BasicRegistry());

			_container.Inject(typeof(IContainer), _container, false);

			//var scanned = _container.WhatDidIScan();
			//var have = _container.WhatDoIHave();
			_container.AssertConfigurationIsValid();

			var wnd = _container.GetInstance<MainWindow>();

			_container.GetInstance<ICommandFactory>()
				.Create<ApplicationStartupCommand>().Execute();

			wnd.Show();

			//create player view as early as possible to start loading Textures in background
			//_container.GetInstance<IPlayerViewFactory>().Create();
		}
	}
}