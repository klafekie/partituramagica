#region usings

using Lamar;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.PageEditing;
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
			For<ISnippetManager>().Use<SnippetManager>().Singleton();
			For<ISpeedManager>().Use<SpeedManager>().Singleton();
			For<IZoomManager>().Use<ZoomManager>().Singleton();
			For<IPlayerController>().Use<PlayerController>().Singleton();
			For<BoxLayouter>().Use<BoxLayouter>();
			For<SystemFinder>().Use<SystemFinder>();
		}
	}
}