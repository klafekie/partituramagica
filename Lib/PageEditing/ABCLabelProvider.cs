#region usings

using System.Collections.Generic;
using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	/**
	 * first part is labelled "A", second part "B" etc
	 */
	internal class AbcLabelProvider : ILabelProvider
	{
		private readonly Partitura _partitura;
		private int _pageIndex;

		public AbcLabelProvider(Partitura partitura)
		{
			_partitura = partitura;
			_pageIndex = -1;
		}

		public string GetLabel(int id)
		{
			Debug.Assert(_pageIndex >= 0);
			IList<Part> allParts = _partitura.GetAllParts(_pageIndex + 1);
			int pos = -1;
			int partIndex = 0;
			for (; partIndex < allParts.Count; partIndex++)
			{
				pos = allParts[partIndex].ContainsAt(id);
				if (pos != -1) break;
			}

			if (pos == 0)
			{
				//only label the first rectangle in each part
				char result = (char) ('A' + partIndex);
				return result.ToString();
			}

			return null;
		}

		public int PageIndex
		{
			set => _pageIndex = value;
		}
	}
}