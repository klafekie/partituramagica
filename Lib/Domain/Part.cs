using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PartituraMagica
{
    /** 
     * represents a part of a partitura. A part is a sequence of Boxes/Splitters
     * possibly spread over several pages that might be repeated when playing.
     * Parts are currently labled A, B, C etc but they might as well have user defined
     * names later
     */

    public class Part : IEnumerable<Clipping>
    {
        private readonly IList<Clipping> _clippings;

        public Part()
        {
            _clippings = new List<Clipping>();
            //this.islabelled = isLabelled;
        }

        public Part(Part rhs)
        {
            _clippings = new List<Clipping>();
            foreach (Clipping c in rhs._clippings)
            {
                _clippings.Add(new Clipping(c));
            }
        }

        internal Clipping this[int index]
        {
            get { return _clippings[index]; }
        }

        internal bool IsEmpty
        {
            get { return _clippings.Count == 0; }
        }

        internal int Count
        {
            get { return _clippings.Count; }
        }

        public double Width
        {
            get
            {
                double result = 0D;
                foreach (Clipping c in _clippings)
                {
                    result += c.Coords.Width;
                }
                return result;
            }
        }

        public IEnumerator<Clipping> GetEnumerator()
        {
            return _clippings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        internal void Add(Clipping clipping)
        {
            _clippings.Add(clipping);
        }

        internal int ContainsAt(int id)
        {
            for (int i = 0; i < _clippings.Count; i++)
            {
                if (_clippings[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public override string ToString()
        {
            return string.Format("{0} ... {1}, Count = {2}", _clippings.FirstOrDefault(), _clippings.LastOrDefault(),
                _clippings.Count);
        }
    }
}