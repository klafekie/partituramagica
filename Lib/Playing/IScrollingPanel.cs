#region usings

using System;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	public interface IScrollingPanel : IDisposable
	{
		void Init(IntPtr handle);
		void StartScrolling(IPlayedPartitura partitura);
	}
}