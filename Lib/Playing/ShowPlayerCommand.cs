#region usings

using System;
using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	public class ShowPlayerCommand : PartituraCommandBase, ICommand
	{
		private readonly IPlayerViewFactory _factory;
		private readonly IMainWindow _mainWindow;

		public ShowPlayerCommand(IPlayerViewFactory factory, ICommandParameters p, IMainWindow mainWindow)
			: base(p)
		{
			_factory = factory;
			_mainWindow = mainWindow;
		}

		public void Execute()
		{
			try
			{
				Debug.Assert(CanExecute());

				_mainWindow.IsEnabled = false;
				_factory.CreateAndShowView(P.CreatePlayedPartitura());
				_mainWindow.IsEnabled = true;
				_mainWindow.Activate();
			}
			catch (Exception e)
			{
				Trace.TraceError("Exception when showing the player:");
				Trace.TraceError(e.ToString());
				_mainWindow.MessageBox("A problem with the initialization of DirectX has occurred:\n\n" +
				                       e.Message +
				                       "\n\nPlease see ReleaseNotes.doc for troubleshooting hints and restart the application.");
			}
		}

		public bool CanExecute()
		{
			return P.Active?.Partitura != null &&
			       P.Active.Partitura.GetAllParts(P.Active.GetPageCount()).Count > 0;
		}
	}
}