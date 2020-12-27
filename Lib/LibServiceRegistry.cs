#region usings

using Lamar;
using PartituraMagica.Playing;

#endregion

namespace PartituraMagica
{
	public class LibServiceRegistry : ServiceRegistry
	{
		public LibServiceRegistry()
		{
			For<IPlayerViewFactory>().Use<PlayerViewFactory>().Singleton();
		}
	}
}