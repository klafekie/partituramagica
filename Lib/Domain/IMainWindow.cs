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

        bool IsEnabled { get; set; }
        
        void ShowPartituraInfo(IActivePartitura active);
        void ClearPartituraInfo();
        

        void MessageBox(string text);
        void CloseAndExit();
        bool Activate();
        void SignalWait(bool doWait);
    }
}