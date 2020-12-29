namespace PartituraMagica.Domain
{
	public class PartituraCommandBase
	{
		protected ICommandParameters P;

		protected PartituraCommandBase(ICommandParameters p)
		{
			P = p;
		}
	}

	public static class CommandExtensions
	{
		public static bool TryExecute(this ICommand cmd)
		{
			if (cmd == null || !cmd.CanExecute()) return false;

			cmd.Execute();
			return true;
		}
	}
}