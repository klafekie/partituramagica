#region usings

using System.Collections.Generic;

#endregion

namespace PartituraMagica.Lib.Playing
{
	public class SnippetMemento
	{
		public ICollection<KeyValuePair<double, double>> Snippets { get; set; }

		public SnippetMemento()
		{
			Snippets = new List<KeyValuePair<double, double>>();
		}
	}
}