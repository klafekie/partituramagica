namespace PartituraMagica
{
    public class PartituraCommandBase : CommandBase
    {
        protected ICommandParameters p;

        protected PartituraCommandBase(ICommandParameters p)
        {
            this.p = p;
        }
    }

    public static class CommandExtensions
    {
        public static bool TryExecute(this ICommand cmd)
        {
            if (cmd != null && cmd.CanExecute())
            {
                cmd.Execute();
                return true;
            }
            return false;
        }
    }

    //@todo p3 this common base class is currently not needed.
    public class CommandBase
    {
        //public static void TryAll(params ICommand[] cmds) {
        //    foreach (ICommand cmd in cmds) {
        //        TryExecute(cmd);
        //    }
        //}

        //public static void ExecuteFirst(ICommand cmd1, ICommand cmd2) {
        //    if (cmd1 != null && cmd1.CanExecute()) {
        //        cmd1.Execute();
        //    } else {
        //        CommandBase.TryExecute(cmd2);
        //    }
        //}
    }
}