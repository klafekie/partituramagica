using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private Device device;
        public Form1()
        {
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PresentParameters pp = new PresentParameters();
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Copy;//??
            this.device = new Device(Manager.Adapters.Default.Adapter,
                DeviceType.Hardware, this, CreateFlags.HardwareVertexProcessing, pp);
        }

        public void Shutdown()
        {
            this.device.Dispose();
        }


        public void Render()
        {
            this.device.Clear(ClearFlags.Target, Color.Red, 0, 0);
            this.device.BeginScene();
            this.device.EndScene();
            this.device.Present();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (int)Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
