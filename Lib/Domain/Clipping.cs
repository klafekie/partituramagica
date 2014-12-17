using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{
	/**
	 * A clipping represents a rectangular area on a certain page.
	 * The id is used to make the connection between the corresponding
	 * boxes and the objects drawn on the drawing panel
	 */
	public class Clipping
	{
		//c'tor for productive use
		public Clipping(int pageIndex, Rect coords, int id) {
			this.pageIndex = pageIndex;
			this.coords = coords;
			this.id = id;
		}

		public Clipping(Clipping rhs) {
			this.pageIndex = rhs.pageIndex;
			this.coords = new Rect(rhs.Coords);
			this.id = rhs.id;
		}

		//c'tor for testing
		public Clipping(int pageIndex, double x, double y, double width, double height) {
			this.pageIndex = pageIndex;
			this.coords = new Rect(x, y, width, height);
			this.id = -1;
		}


		private int pageIndex;
		public int PageIndex {
			get {
				return this.pageIndex;
			}
			set {
				this.pageIndex = value;
			}
		}
		private Rect coords;
		public Rect Coords {
			get {
				return this.coords;
			}
		}
		private int id;
		public int Id {
			get {
				return this.id;
			}
		}

		public void AddAboveAndBelow(double deltaY) {
			this.coords = this.coords.AddAboveAndBelow(deltaY);
		}

        public void MoveTo(double xPos, double yPos) {
			this.coords = this.coords.MoveTo(xPos, yPos);
		}

		public override string ToString() {
			return this.coords.ToString();
		}

	}
}
