using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            Form1 f = new Form1();
            f.Show();
            while (f.Created)
            {
                f.Render();
                Application.DoEvents();
            }
            f.Shutdown();
        }
    }
}
