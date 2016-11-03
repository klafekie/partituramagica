using System;
using System.Windows.Forms;

namespace PartituraMagica
{
    /**
     * A custom control that allows to change the current horizontal and
     * vertical zoom 
     */

    public partial class ZoomToolStrip : ToolStrip
    {
        private ZoomManager _zoomManager;

        public ZoomToolStrip()
        {
            InitializeComponent();
            btnSmaller.Click += btnSmaller_Click;
            btnWider.Click += btnWider_Click;
            btnZoomIn.Click += btnZoomIn_Click;
            btnZoomOut.Click += btnZoomOut_Click;
        }

        public void RegisterEvents(ZoomManager zoomManager)
        {
            _zoomManager = zoomManager;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            _zoomManager.ZoomOut(false);
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            _zoomManager.ZoomIn(false);
        }

        private void btnWider_Click(object sender, EventArgs e)
        {
            _zoomManager.ZoomIn(true);
        }

        private void btnSmaller_Click(object sender, EventArgs e)
        {
            _zoomManager.ZoomOut(true);
        }
    }
}