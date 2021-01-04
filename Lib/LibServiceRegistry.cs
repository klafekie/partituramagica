#region usings

using Lamar;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.Lib
{
	public class LibServiceRegistry : ServiceRegistry
	{
		public LibServiceRegistry()
		{
			For<ICommandFactory>().Use<CommandFactory>().Singleton();
			For<IPartNavigator>().Use<PartNavigator>().Singleton();
		}
	}
}