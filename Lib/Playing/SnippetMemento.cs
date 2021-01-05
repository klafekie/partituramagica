#region usings

using System.Collections.Generic;

#endregion

namespace PartituraMagica.Lib.Playing
{
	public class SnippetMemento
	{
		public ICollection<KeyValuePair<double, double>> Snippets { get; set; }
	}
}