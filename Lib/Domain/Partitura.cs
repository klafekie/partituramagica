using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.PageEditing;
using PartituraMagica.Lib.Playing;

namespace PartituraMagica
{
	/** 
     * Represents a musical piece to be displayed. 
     * consists of a list of pages. Several boxes on each page.
     * boxes are grouped into Parts (according to the repetition structure
     * of the piece.
     */
	public class Partitura
	{
		private List<Page> pages;
		private bool dirty;
		private ILabelProvider labelProvider;
		private string partPattern;
		private SnippetMemento _snippets;

		internal SnippetMemento Snippets
		{
			get => _snippets;
			set
			{
				dirty = true;
				_snippets = value;
			}
		}

		public Partitura()
		{
			this.pages = new List<Page>();
			this.dirty = false;
			//@todo p3 not nice to depend on a class outside the
			//domain "package" here cause this would be a cyclic dependency
			this.labelProvider = new AbcLabelProvider(this);
		}

		public Partitura(PartituraJsonMemento p)
				: this()
		{
			this.partPattern = p.PartPattern;
			foreach (PageJsonMemento page in p.Pages)
			{
				this.pages.Add(new Page(page));
			}

			_snippets = p.Snippets;
		}

		public PartituraJsonMemento ToMemento()
		{
			this.dirty = false;
			return new PartituraJsonMemento()
			{
				PartPattern = PartPattern,
				Pages = this.pages.Select(p => p.ToMemento()).ToArray(),
				Snippets = _snippets
			};
		}

		public bool Dirty
		{
			get
			{
				if (this.dirty)
				{
					return true;
				}
				foreach (Page p in pages)
				{
					if (p.Dirty)
					{
						return true;
					}
				}
				return false;
			}
		}

		public Page GetPage(int index)
		{
			//this makes sure that there is a PageInfo for each index
			//The index range is defined by the number of available image files!
			for (int i = pages.Count; i <= index; i++)
			{
				pages.Add(new Page());
				dirty = true;
			}
			return pages[index];
		}

		internal void Paint(int index, IDrawingPanel panel, Point dimensions)
		{
			this.labelProvider.PageIndex = index;
			this.GetPage(index).PaintPage(panel, dimensions, labelProvider);
		}

		/// <summary>
		/// Returns a list of all Parts, as collected by nested calling of AppendParts from Partitura to pages to boxes.
		/// The result can contain empty parts (maybe only as the first part?)
		/// </summary>
		public IList<Part> GetAllParts(int pageCount)
		{
			IList<Part> result = new List<Part>(); // Used as collecting variable.

			for (int i = 0; i < pageCount; i++)
			{
				GetPage(i).AppendParts(result, i);
			}

			return result;
		}

		internal void LabelSelection(IDrawingPanel panel, int pageIndex)
		{
			this.labelProvider.PageIndex = pageIndex;
			GetPage(pageIndex).LabelSelection(panel, this.labelProvider);
		}

		internal void UnLabelSelection(IDrawingPanel panel, int pageIndex)
		{
			this.labelProvider.PageIndex = pageIndex;
			GetPage(pageIndex).UnLabelSelection(panel, this.labelProvider);
		}

		public void AddBox(int pageIndex, Rect r, IDrawingPanel panel)
		{
			this.labelProvider.PageIndex = pageIndex;
			GetPage(pageIndex).AddBox(r, panel, this.labelProvider);
		}

		public void RemoveSelectedBox(int pageIndex, IDrawingPanel panel)
		{
			this.labelProvider.PageIndex = pageIndex;
			GetPage(pageIndex).RemoveSelectedBox(panel, this.labelProvider);
		}

		public string PartPattern
		{
			get
			{
				return this.partPattern;
			}
			set
			{
				this.dirty = true;
				this.partPattern = value.ToUpper().Trim();
			}
		}
	}
}
