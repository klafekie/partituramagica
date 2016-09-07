using System;
using System.Collections.Generic;
using System.Diagnostics;
using PartituraMagica.Playing;

namespace PartituraMagica
{
    public class ShowPlayerCommand : PartituraCommandBase, ICommand
    {
        private readonly Func<IPlayerView> _createView;

        public ShowPlayerCommand(Func<IPlayerView> createView, ICommandParameters p)
            : base(p)
        {
            _createView = createView;
        }

        public void Execute()
        {
            try
            {
                Debug.Assert(CanExecute());

                IList<Part> allParts = P.Active.Partitura.GetAllParts(P.Active.GetPageCount());

                IPlayerView view = _createView();
                using (Navigator n = new Navigator(P.Active.Partitura.PartPattern, allParts, view))
                {
                    P.MainWindow.IsEnabled = false;
                    n.Start();
                    P.MainWindow.IsEnabled = true;
                }
                bool test = P.MainWindow.Activate();
                //Debug.WriteLine("Activate Success = " + test);
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception when showing the player:");
                Trace.TraceError(e.ToString());
                P.MainWindow.MessageBox("A problem with the initialization of DirectX has occurred:\n\n" +
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