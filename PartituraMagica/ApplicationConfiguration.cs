#region usings

using System;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using PartituraMagica.Lib;

#endregion

namespace PartituraMagica.App
{
	internal class ApplicationConfiguration : IApplicationConfiguration
	{
		private const string LASTPARTITURAPATH = "lastPartituraPath";

		public ApplicationConfiguration()
		{
			string[] colString = ConfigurationManager.AppSettings["player.paperColor"]
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.ToArray();
			int red = int.Parse(colString[0]);
			int green = int.Parse(colString[1]);
			int blue = int.Parse(colString[2]);
			Color = System.Drawing.Color.FromArgb(red, green, blue).ToArgb();
			InvertColors = bool.Parse(ConfigurationManager.AppSettings["player.invertColors"]);
			LastPartituraPath = ConfigurationManager.AppSettings[LASTPARTITURAPATH];
		}

		public int Color { get; }

		public bool InvertColors { get; }

		public string LastPartituraPath { get; set; }

		public void Save()
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
			config.AppSettings.Settings[LASTPARTITURAPATH].Value = LastPartituraPath;
			config.Save();
			ConfigurationManager.RefreshSection("appSettings");
		}
	}
}