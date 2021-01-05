#region usings

using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

#endregion

// the namespace may not be changed to preserve backward compatibility with binary formater.
// ReSharper disable once CheckNamespace 
namespace PartituraMagica
{
	[Serializable]
	[Obsolete("This memento is only valid for converting partitura files from old binary format to json.")]
	public struct PartituraMemento
	{
		public PartituraMemento(ICollection<Page> pages, String partPattern)
		{
			this.partPattern = partPattern;
			this.pages = new ArrayList(pages.Count);
			foreach (Page p in pages) this.pages.Add(p.ToMemento());
		}

		private ArrayList pages;
		private String partPattern;

		public ICollection Pages
		{
			get { return pages; }
		}

		public String PartPattern
		{
			get { return partPattern; }
		}
	}
}