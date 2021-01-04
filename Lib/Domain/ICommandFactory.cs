#region usings

using System;

#endregion

namespace PartituraMagica.Lib.Domain
{
	public interface ICommandFactory
	{
		ICommand Create(string name);
		T Create<T>(Action<T> initAction = null) where T : ICommand;
	}
}