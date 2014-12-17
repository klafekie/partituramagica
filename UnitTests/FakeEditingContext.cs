using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{
	class FakeEditingContext : IEditingContext
	{
		private Partitura partitura;
		private string currentPath;
		public string CurrentPath
		{
			get {return currentPath;}
			set {currentPath = value; }
		}

		public Partitura CurrentPartitura
		{
			get
			{
				return partitura;
			}
			set
			{
				partitura = value;
			}
		}
	}
}
