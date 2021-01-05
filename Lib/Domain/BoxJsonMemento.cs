#region usings

using System.Collections.Generic;

#endregion

namespace PartituraMagica.Lib.Domain
{
	public class BoxJsonMemento
	{
		public int Id { get; set; }
		public RectJsonMemento Coords { get; set; }
		public ICollection<KeyValuePair<double, int>> Splitters { get; set; }
		public ICollection<int> LabelIds { get; set; }
	}
}