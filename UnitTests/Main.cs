using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{
	class MainClass
	{
		public static int Main(string[] args)
		{
			//new EditingTests().RunAll();			
			new PartituraTests().RunAll();
			//new PlayingTests().RunAll();
			//new NavigatorTests().RunAll();
			new BoxFinderTests().RunAll();
			return 0;
		}
	}
}
