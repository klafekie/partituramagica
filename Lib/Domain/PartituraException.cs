#region usings

using System;

#endregion

namespace PartituraMagica.Lib.Domain
{
	public class PartituraException : Exception
	{
		public PartituraException(string message) : base(message)
		{
		}
	}
}