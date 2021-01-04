#region usings

using System;
using System.Configuration;
using System.Drawing;
using System.Linq;

#endregion

namespace PartituraMagica.App
{
	internal class ApplicationConfiguration
	{
		public Color Color { get; }
		public bool InvertColors { get; }

		public ApplicationConfiguration()
		{
			string[] colString = ConfigurationManager.AppSettings["player.paperColor"]
				.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.ToArray();
			int red = int.Parse(colString[0]);
			int green = int.Parse(colString[1]);
			int blue = int.Parse(colString[2]);
			Color = Color.FromArgb(red, green, blue);
			InvertColors = bool.Parse(ConfigurationManager.AppSettings["player.invertColors"]);
		}
	}
}