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
            this._pages = new ArrayList(pages.Count);
            foreach (Page p in pages)
            {
                this._pages.Add(p.ToMemento());
            }
        }

        private readonly ArrayList _pages;

        public ICollection Pages
        {
            get { return _pages; }
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
        private bool _dirty;
        private readonly ILabelProvider _labelProvider;
        private readonly List<Page> _pages;
        private string _partPattern;

        public Partitura()
        {
            _pages = new List<Page>();
            _dirty = false;
            //@todo p3 not nice to depend on a class outside the
            //domain "package" here cause this would be a cyclic dependency
            _labelProvider = new AbcLabelProvider(this);
        }

        public Partitura(PartituraMemento p)
            : this()
        {
            _partPattern = p.PartPattern;
            foreach (object o in p.Pages)
            {
                _pages.Add(new Page((PageMemento) o));
            }
        }

        public bool Dirty
        {
            get
            {
                if (_dirty)
                {
                    return true;
                }
                foreach (Page p in _pages)
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
            get { return _partPattern; }
            set
            {
                _dirty = true;
                _partPattern = value.ToUpper().Trim();
            }
        }

        public PartituraMemento ToMemento()
        {
            _dirty = false;
            return new PartituraMemento(_pages, _partPattern);
        }

        public Page GetPage(int index)
        {
            //this makes sure that there is a PageInfo for each index
            //The index range is defined by the number of available image files!
            for (int i = _pages.Count; i <= index; i++)
            {
                _pages.Add(new Page());
                _dirty = true;
            }
            return _pages[index];
        }

        internal void Paint(int index, IDrawingPanel panel, Point dimensions)
        {
            _labelProvider.PageIndex = index;
            GetPage(index).PaintPage(panel, dimensions, _labelProvider);
        }

        internal void LabelSelection(IDrawingPanel panel, int pageIndex)
        {
            _labelProvider.PageIndex = pageIndex;
            GetPage(pageIndex).LabelSelection(panel, _labelProvider);
        }

        internal void UnLabelSelection(IDrawingPanel panel, int pageIndex)
        {
            _labelProvider.PageIndex = pageIndex;
            GetPage(pageIndex).UnLabelSelection(panel, _labelProvider);
        }

        public void AddBox(int pageIndex, Rect r, IDrawingPanel panel)
        {
            _labelProvider.PageIndex = pageIndex;
            GetPage(pageIndex).AddBox(r, panel, _labelProvider);
        }

        public void RemoveSelectedBox(int pageIndex, IDrawingPanel panel)
        {
            _labelProvider.PageIndex = pageIndex;
            GetPage(pageIndex).RemoveSelectedBox(panel, _labelProvider);
        }
    }
}