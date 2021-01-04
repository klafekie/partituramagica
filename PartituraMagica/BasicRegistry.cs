#region usings

using Lamar;
using Microsoft.Extensions.DependencyInjection;
using PartituraMagica.App.Player;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.App
{
	public class BasicRegistry : ServiceRegistry
	{
		public BasicRegistry()
		{
			For<MainWindow>().Use<MainWindow>().Singleton();
			For<IMainWindow>().Use(s => s.GetInstance<MainWindow>());
			For<ICommandParameters>().Use<EditingController>().Singleton();
			For<IPlayerViewFactory>().Use<PlayerViewFactory>().Singleton();
			For<IPlayerView>().Use<PlayerViewD3D>();
			For<IImageLoader>().Use(s => s.GetInstance<ICommandParameters>().Panel);
			For<IFileSystem>().Use<FileSystem>().Singleton();
			For<PlayerPositionHolder>().Use<PlayerPositionHolder>().Singleton();

			Scan(_ =>
			{
				_.LookForRegistries();

				_.Assembly("PartituraMagica");
				_.Assembly("ClassLibrary1"); //Lib

				_.Include(type => type.Name.EndsWith("Command") || type.Name.EndsWith("ServiceRegistry"));
				_.AddAllTypesOf<ICommand>().NameBy(x => x.Name.Replace("Command", ""));
				_.WithDefaultConventions(ServiceLifetime.Singleton);
			});
		}
	}
}