#region usings

using System;

#endregion

namespace PartituraMagica.Domain
{
	public interface ICommandFactory
	{
		ICommand Create(string name);
		T Create<T>(Action<T> initAction = null) where T : ICommand;
	}
}