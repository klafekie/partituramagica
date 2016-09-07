using System;
using System.Collections.Generic;
using System.Diagnostics;
using PartituraMagica.Playing;

namespace PartituraMagica
{
    public class ShowPlayerCommand : PartituraCommandBase, ICommand
    {
        private readonly Func<IPlayerView> CreateView;

        public ShowPlayerCommand(Func<IPlayerView> createView, ICommandParameters p)
            : base(p)
        {
            CreateView = createView;
        }

        public void Execute()
        {
            try
            {
                Debug.Assert(CanExecute());

                IList<Part> allParts = p.Active.Partitura.GetAllParts(p.Active.GetPageCount());

                IPlayerView view = CreateView();
                using (Navigator n = new Navigator(p.Active.Partitura.PartPattern, allParts, view))
                {
                    p.MainWindow.IsEnabled = false;
                    n.Start();
                    p.MainWindow.IsEnabled = true;
                }
                bool test = p.MainWindow.Activate();
                //Debug.WriteLine("Activate Success = " + test);
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception when showing the player:");
                Trace.TraceError(e.ToString());
                p.MainWindow.MessageBox("A problem with the initialization of DirectX has occurred:\n\n" +
                                        e.Message +
                                        "\n\nPlease see ReleaseNotes.doc for troubleshooting hints and restart the application.");
            }
        }

        public bool CanExecute()
        {
            return p.Active != null && p.Active.Partitura != null &&
                   p.Active.Partitura.GetAllParts(p.Active.GetPageCount()).Count > 0;
        }
    }
}