#region usings

using System;
using Lamar;

#endregion

namespace PartituraMagica.Domain
{
	internal class CommandFactory : ICommandFactory
	{
		private readonly IContainer _container;

		public CommandFactory(IContainer container)
		{
			_container = container;
		}

		public ICommand Create(string name)
		{
			return _container.GetInstance<ICommand>(name);
		}

		public T Create<T>(Action<T> initAction = null) where T : ICommand
		{
			//TODO p2 directly register commands via a custom convention
			string name = typeof(T).Name.Replace("Command", "");
			T result = (T) _container.GetInstance<ICommand>(name);
			initAction?.Invoke(result);
			return result;
		}
	}
}