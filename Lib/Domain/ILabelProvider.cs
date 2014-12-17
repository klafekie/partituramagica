using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PartituraMagica
{
	/**
	 * Implementors can decide about a certain labelling strategy for 
	 * the parts of a partitura. E.G: ABCADD or more complicated scenarios
	 * with named parts...
	 */
	public interface ILabelProvider
	{		
		/** returns the label to set for the element (box/splitter) with the
		 * given Id, null if there should be no label
		 */
		string GetLabel(int id);

		int PageIndex { set; }		
	}
}
