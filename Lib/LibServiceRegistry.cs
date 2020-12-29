#region usings

using Lamar;
using PartituraMagica.Domain;
using PartituraMagica.Playing;

#endregion

namespace PartituraMagica
{
	public class LibServiceRegistry : ServiceRegistry
	{
		public LibServiceRegistry()
		{
			For<IPlayerViewFactory>().Use<PlayerViewFactory>().Singleton();
			For<ICommandFactory>().Use<CommandFactory>().Singleton();
		}
	}
}