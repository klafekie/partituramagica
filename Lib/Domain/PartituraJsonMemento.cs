#region usings

using System.Collections.Generic;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.Lib.Domain
{
	public class PartituraJsonMemento
	{
		public ICollection<PageJsonMemento> Pages { get; set; }
		public string PartPattern { get; set; }
		public SnippetMemento Snippets { get; set; }
	}
}