#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.Playing
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

				IList<Part> allParts = P.Active.Partitura.GetAllParts(P.Active.GetPageCount());

				PlayerPositionHolder positionHolder = new PlayerPositionHolder();
				PartNavigator navigator = new PartNavigator(P.Active.Partitura.PartPattern, allParts, positionHolder);
				IPlayerView view = _factory.CreateView(positionHolder, navigator);


				_mainWindow.IsEnabled = false;
				navigator.PlayerOpened(view);
				_mainWindow.IsEnabled = true;

				bool test = _mainWindow.Activate();
				//Debug.WriteLine("Activate Success = " + test);
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
			return P.Active != null && P.Active.Partitura != null &&
			       P.Active.Partitura.GetAllParts(P.Active.GetPageCount()).Count > 0;
		}
	}
}