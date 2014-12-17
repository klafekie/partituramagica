using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PlayerView
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Form1 f = new Form1();
			f.Show();
			Application.Exit();
		}
	}
}
