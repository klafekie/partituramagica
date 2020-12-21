#region usings

using System;

#endregion

namespace PartituraMagica.Domain
{
	public class PartituraException : Exception
	{
		public PartituraException(string message) : base(message)
		{
		}
	}
}