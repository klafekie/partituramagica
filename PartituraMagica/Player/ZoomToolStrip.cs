using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PartituraMagica
{
    /**
     * A custom control that allows to change the current horizontal and
     * vertical zoom 
     */
    public partial class ZoomToolStrip : ToolStrip
    {
        //with the help of xZoomdelta and yzoomdelta we implement a 
        //smooth change of the zoom. See GetNextZoom()
        private int xZoomDelta = 0;
        private int yZoomDelta = 0;
        private float xZoom;
        private float yZoom;

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
                this.xZoomDelta = 0;
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
                this.yZoomDelta = 0;
            }
        }

        public ZoomToolStrip()
        {
            InitializeComponent();
            this.btnSmaller.Click += new EventHandler(btnSmaller_Click);
            this.btnWider.Click += new EventHandler(btnWider_Click);
            this.btnZoomIn.Click += new EventHandler(btnZoomIn_Click);
            this.btnZoomOut.Click += new EventHandler(btnZoomOut_Click);
        }

        void btnZoomOut_Click(object sender, EventArgs e)
        {
            ZoomOut(false);
        }

        private void ZoomOut(bool xOnly)
        {
            this.xZoomDelta = -15;
            if (!xOnly)
            {
                this.yZoomDelta = -15;
            }
        }

        private void ZoomIn(bool xOnly)
        {
            this.xZoomDelta = 15;
            if (!xOnly)
            {
                this.yZoomDelta = 15;
            }
        }

        void btnZoomIn_Click(object sender, EventArgs e)
        {
            ZoomIn(false);
        }

        void btnWider_Click(object sender, EventArgs e)
        {
            ZoomIn(true);
        }

        void btnSmaller_Click(object sender, EventArgs e)
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
                this.xZoom *= 1.01F;
                xZoomDelta--;
            }
            if (xZoomDelta < 0)
            {
                this.xZoom *= 0.99F;
                xZoomDelta++;
            }

            if (yZoomDelta > 0)
            {
                this.yZoom *= 1.01F;
                yZoomDelta--;
            }
            if (yZoomDelta < 0)
            {
                this.yZoom *= 0.99F;
                yZoomDelta++;
            }
            zoomXRes = this.xZoom;
            zoomYRes = this.yZoom;
        }
    }
}
