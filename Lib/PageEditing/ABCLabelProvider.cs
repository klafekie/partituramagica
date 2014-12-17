using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PartituraMagica
{
	/** first part is labelled "A", second part "B" etc */
	class ABCLabelProvider : ILabelProvider
	{
		private Partitura partitura;
		private int pageIndex;

		public ABCLabelProvider(Partitura partitura)
		{
			this.partitura = partitura;
			this.pageIndex = -1;
		}

		public string GetLabel(int id)
		{
			Debug.Assert(this.pageIndex >= 0);
			IList<Part> allParts = this.partitura.GetAllParts(this.pageIndex + 1);
			int pos = -1;
			int partIndex = 0;
			for (; partIndex < allParts.Count; partIndex++) {
				pos = allParts[partIndex].ContainsAt(id);
				if (pos != -1) {
					break;
				}		
			}			

			if (pos == 0) {//only label the first rectangle in each part
				char result = (char)('A' + partIndex);
				return result.ToString(); 				
			} else {
				return null;
			}
		}

		public int PageIndex
		{
			set
			{
				this.pageIndex = value;
			}
		}
	}
}
