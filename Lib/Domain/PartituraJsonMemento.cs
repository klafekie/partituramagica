#region usings

using System.Collections.Generic;

#endregion

namespace PartituraMagica.Lib.Domain
{
	public class PartituraJsonMemento
	{
		public ICollection<PageJsonMemento> Pages { get; set; }
		public string PartPattern { get; set; }

		//TODO
		// ReSharper disable once EmptyConstructor
		//ctor needed for json deserialization
		public PartituraJsonMemento()
		{
		}
	}
}