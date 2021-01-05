#region usings

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

// please do not adjust namespace or type name to preserve backward compatibiltiy with old binary format.
// ReSharper disable once CheckNamespace
namespace PartituraMagica
{
	[Serializable]
	internal struct PageMemento
	{
		public PageMemento(ICollection<Box> theboxes)
		{
			boxes = new ArrayList(theboxes.Count);
			foreach (Box b in theboxes) boxes.Add(b);
		}

		private ArrayList boxes;
		public ICollection Boxes => boxes;
	}
}