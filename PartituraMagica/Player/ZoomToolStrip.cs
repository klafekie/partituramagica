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
        private float xZoom;
        //with the help of xZoomdelta and yzoomdelta we implement a 
        //smooth change of the zoom. See GetNextZoom()
        private int xZoomDelta;
        private float yZoom;
        private int yZoomDelta;

        public ZoomToolStrip()
        {
            InitializeComponent();
            btnSmaller.Click += btnSmaller_Click;
            btnWider.Click += btnWider_Click;
            btnZoomIn.Click += btnZoomIn_Click;
            btnZoomOut.Click += btnZoomOut_Click;
        }

        public float XZoom
        {
            get { return xZoom; }
            set
            {
                if (value == 0F)
                {
                    xZoom = 0.000001F;
                }
                else
                {
                    xZoom = value;
                }
                xZoomDelta = 0;
            }
        }

        public float YZoom
        {
            get { return yZoom; }
            set
            {
                if (value == 0F)
                {
                    yZoom = 0.000001F;
                }
                else
                {
                    yZoom = value;
                }
                yZoomDelta = 0;
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            ZoomOut(false);
        }

        private void ZoomOut(bool xOnly)
        {
            xZoomDelta = -15;
            if (!xOnly)
            {
                yZoomDelta = -15;
            }
        }

        private void ZoomIn(bool xOnly)
        {
            xZoomDelta = 15;
            if (!xOnly)
            {
                yZoomDelta = 15;
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            ZoomIn(false);
        }

        private void btnWider_Click(object sender, EventArgs e)
        {
            ZoomIn(true);
        }

        private void btnSmaller_Click(object sender, EventArgs e)
        {
            ZoomOut(true);
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        ZoomOut(e.Shift);
                        break;
                    case Keys.Oemplus:
                    case Keys.Add:
                        ZoomIn(e.Shift);
                        break;
                }
            }
        }

        /** 
         * This method is called each time the current view is rendered.
         * When the user has increased the zoom, the new value is "smoothly"
         * reached. That is, on each call to this method the returned value is 
         * a bit bigger until the actual target zoom value is reached.
         * This results in a nice "animated" zoom in/out behaviour
         */

        public void GetNextZoom(out float zoomXRes, out float zoomYRes)
        {
            if (xZoomDelta > 0)
            {
                xZoom *= 1.01F;
                xZoomDelta--;
            }
            if (xZoomDelta < 0)
            {
                xZoom *= 0.99F;
                xZoomDelta++;
            }

            if (yZoomDelta > 0)
            {
                yZoom *= 1.01F;
                yZoomDelta--;
            }
            if (yZoomDelta < 0)
            {
                yZoom *= 0.99F;
                yZoomDelta++;
            }
            zoomXRes = xZoom;
            zoomYRes = yZoom;
        }
    }
}