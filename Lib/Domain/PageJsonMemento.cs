#region usings

using System.Collections.Generic;

#endregion

namespace PartituraMagica.Lib.Domain
{
	public class PageJsonMemento
	{
		public ICollection<BoxJsonMemento> Boxes { get; set; }
	}
}