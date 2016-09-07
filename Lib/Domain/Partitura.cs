using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace PartituraMagica
{
    [Serializable]
    public struct PartituraMemento
    {
        public PartituraMemento(ICollection<Page> pages, string partPattern)
        {
            PartPattern = partPattern;
            this.pages = new ArrayList(pages.Count);
            foreach (Page p in pages)
            {
                this.pages.Add(p.ToMemento());
            }
        }

        private readonly ArrayList pages;

        public ICollection Pages
        {
            get { return pages; }
        }

        public string PartPattern { get; }
    }

    /** 
     * Represents a musical piece to be displayed. 
     * consists of a list of pages. Several boxes on each page.
     * boxes are grouped into Parts (according to the repetition structure
     * of the piece.
     */

    public class Partitura : IPlayablePartitura
    {
        private bool dirty;
        private readonly ILabelProvider labelProvider;
        private readonly List<Page> pages;
        private string partPattern;

        public Partitura()
        {
            pages = new List<Page>();
            dirty = false;
            //@todo p3 not nice to depend on a class outside the
            //domain "package" here cause this would be a cyclic dependency
            labelProvider = new ABCLabelProvider(this);
        }

        public Partitura(PartituraMemento p)
            : this()
        {
            partPattern = p.PartPattern;
            foreach (object o in p.Pages)
            {
                pages.Add(new Page((PageMemento) o));
            }
        }

        public bool Dirty
        {
            get
            {
                if (dirty)
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

        public string PartPattern
        {
            get { return partPattern; }
            set
            {
                dirty = true;
                partPattern = value.ToUpper().Trim();
            }
        }

        public PartituraMemento ToMemento()
        {
            dirty = false;
            return new PartituraMemento(pages, partPattern);
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
            labelProvider.PageIndex = index;
            GetPage(index).PaintPage(panel, dimensions, labelProvider);
        }

        internal void LabelSelection(IDrawingPanel panel, int pageIndex)
        {
            labelProvider.PageIndex = pageIndex;
            GetPage(pageIndex).LabelSelection(panel, labelProvider);
        }

        internal void UnLabelSelection(IDrawingPanel panel, int pageIndex)
        {
            labelProvider.PageIndex = pageIndex;
            GetPage(pageIndex).UnLabelSelection(panel, labelProvider);
        }

        public void AddBox(int pageIndex, Rect r, IDrawingPanel panel)
        {
            labelProvider.PageIndex = pageIndex;
            GetPage(pageIndex).AddBox(r, panel, labelProvider);
        }

        public void RemoveSelectedBox(int pageIndex, IDrawingPanel panel)
        {
            labelProvider.PageIndex = pageIndex;
            GetPage(pageIndex).RemoveSelectedBox(panel, labelProvider);
        }
    }
}