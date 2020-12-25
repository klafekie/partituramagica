#region usings

using BaselineTypeDiscovery;
using Lamar;
using Lamar.Scanning.Conventions;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica
{
	public class BasicRegistry : ServiceRegistry
	{
		public class CommandRegistrationConvention : IRegistrationConvention
		{
			public void ScanTypes(TypeSet types, ServiceRegistry services)
			{
				//foreach (var type in types.FindTypes(TypeClassification.Concretes))
				//{
				//	foreach (var itf in type.FindInterfaces((t, criteria) => t.Name == "ICommand", null))
				//	{
				//		services.AddTransient(itf, type,).   .Named(type.Name.Replace("Command", ""));
				//	}
				//}
			}
		}

		public BasicRegistry()
		{
			Scan(_ =>
			{
				// Declare which assemblies to scan
				_.Assembly("PartituraMagica");
				_.Assembly("ClassLibrary1");

				// Filter types
				_.Include(type => type.Name.EndsWith("Command"));
				//_.Include(type => type.Name == "NewPartituraCommand" || type.Name == "AddBoxCommand");
				//_.Exclude(type => type.Name == "AddBoxCommand");
				//		"A") || type.FullName.StartsWith("System.Windows") || type.FullName == "PartituraMagica.Rect");

				// A custom registration convention
				//_.Convention<CommandRegistrationConvention>();

				// Built in registration conventions
				_.AddAllTypesOf<ICommand>().NameBy(x => x.Name.Replace("Command", ""));
				//_.WithDefaultConventions(ServiceLifetime.Singleton);
			});
		}
	}
}