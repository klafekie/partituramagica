#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using PartituraMagica.Lib.Domain;

#endregion

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