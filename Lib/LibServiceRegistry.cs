#region usings

using Lamar;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica
{
	public class LibServiceRegistry : ServiceRegistry
	{
		public LibServiceRegistry()
		{
			For<ICommandFactory>().Use<CommandFactory>().Singleton();
		}
	}
}