#region usings

using System;
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
			_container?.Dispose(); //this will dispose all disposable objects created by container
		}


		private void Application_Startup(object sender, StartupEventArgs e)
		{
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

			string[] args = Environment.GetCommandLineArgs();
			if (args.Length == 2)
				//first arg is the exe name
				_container.GetInstance<ICommandFactory>()
					.Create<OpenCommand>(c => c.Path = args[1]).Execute();

			wnd.Show();
		}
	}
}