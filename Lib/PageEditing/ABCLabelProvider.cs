using System.Collections.Generic;
using System.Diagnostics;

namespace PartituraMagica
{
    /** first part is labelled "A", second part "B" etc */

    internal class ABCLabelProvider : ILabelProvider
    {
        private int pageIndex;
        private readonly Partitura partitura;

        public ABCLabelProvider(Partitura partitura)
        {
            this.partitura = partitura;
            pageIndex = -1;
        }

        public string GetLabel(int id)
        {
            Debug.Assert(pageIndex >= 0);
            IList<Part> allParts = partitura.GetAllParts(pageIndex + 1);
            int pos = -1;
            int partIndex = 0;
            for (; partIndex < allParts.Count; partIndex++)
            {
                pos = allParts[partIndex].ContainsAt(id);
                if (pos != -1)
                {
                    break;
                }
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
            set { pageIndex = value; }
        }
    }
}