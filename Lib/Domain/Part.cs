using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		private IList<Clipping> clippings;

		public Part() {
			this.clippings = new List<Clipping>();
			//this.islabelled = isLabelled;
		}

		public Part(Part rhs) {
			this.clippings = new List<Clipping>();
			foreach (Clipping c in rhs.clippings) {
				this.clippings.Add(new Clipping(c));
			}
		}		

		internal Clipping this[int index] {
			get {
				return this.clippings[index];
			}
		}

		public IEnumerator<Clipping> GetEnumerator()
		{
			return this.clippings.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		internal void Add(Clipping clipping)
		{
			this.clippings.Add(clipping);
		}

		internal int ContainsAt(int id) {
			for (int i = 0; i < this.clippings.Count; i++) {
				if (this.clippings[i].Id == id) {
					return i;
				}
			}
			return -1;
		}

		internal bool IsEmpty {
			get {
				return this.clippings.Count == 0;
			}
		}

		internal int Count {
			get {
				return this.clippings.Count;
			}
		}

		public double Width
		{
			get
			{
				double result = 0D;
				foreach (Clipping c in this.clippings) {
					result += c.Coords.Width;
				}
				return result;
			}
		}

		public override string ToString() {
			return String.Format("{0} ... {1}, Count = {2}", this.clippings.FirstOrDefault(), this.clippings.LastOrDefault(), this.clippings.Count);
		}
	}
}
