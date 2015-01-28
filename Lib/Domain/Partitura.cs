using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;

namespace PartituraMagica
{
    [Serializable]
    public struct PartituraMemento
    {
        public PartituraMemento(ICollection<Page> pages, String partPattern)
        {
            this.partPattern = partPattern;
            this.pages = new ArrayList(pages.Count);
            foreach (Page p in pages)
            {
                this.pages.Add(p.SaveToMemento());
            }
        }
        private ArrayList pages;
        private String partPattern;
        public ICollection Pages
        {
            get
            {
                return this.pages;
            }
        }
        public String PartPattern
        {
            get
            {
                return partPattern;
            }
        }
    }

    /** 
     * Represents a musical piece to be displayed. 
     * consists of a list of pages. Several boxes on each page.
     * boxes are grouped into Parts (according to the repetition structure
     * of the piece.
     */
    public class Partitura : IPlayablePartitura
    {
        private List<Page> pages;
        private bool dirty;
        private ILabelProvider labelProvider;
        private string partPattern;

        public Partitura()
        {
            this.pages = new List<Page>();
            this.dirty = false;
            //@todo p3 not nice to depend on a class outside the
            //domain "package" here cause this would be a cyclic dependency
            this.labelProvider = new ABCLabelProvider(this);
        }

        public Partitura(PartituraMemento p)
            : this()
        {
            this.partPattern = p.PartPattern;
            foreach (object o in p.Pages)
            {
                this.pages.Add(new Page((PageMemento)o));
            }
        }

        public PartituraMemento SaveToMemento()
        {
            this.dirty = false;
            return new PartituraMemento(this.pages, this.partPattern);
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
