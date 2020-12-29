#region usings

using Lamar;
using Microsoft.Extensions.DependencyInjection;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica
{
	public class BasicRegistry : ServiceRegistry
	{
		public BasicRegistry()
		{
			For<MainWindow>().Use<MainWindow>().Singleton();
			For<IMainWindow>().Use(s => s.GetInstance<MainWindow>());
			For<ICommandParameters>().Use<EditingController>().Singleton();
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