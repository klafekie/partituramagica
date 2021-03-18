#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	public class AutoFillCommand : PartituraCommandBase, ICommand
	{
		private readonly BoxLayouter _boxLayouer;
		private readonly ICommandFactory _commandFactory;
		private readonly SystemFinder _systemFinder;

		public AutoFillCommand(ICommandParameters p, ICommandFactory commandFactory, BoxLayouter boxLayouer,
			SystemFinder systemFinder)
			: base(p)
		{
			_commandFactory = commandFactory;
			_boxLayouer = boxLayouer;
			_systemFinder = systemFinder;
		}

		public bool CanExecute()
		{
			return P.Active.IsOpen && P.Active.GetPageCount() > 0;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			var systems = _systemFinder.DetectSystems();
			foreach (var system in systems) Trace.WriteLine("AutoFillCommand Systems:" + system);

			var boxes = _boxLayouer.Execute(systems);
			foreach (var box in boxes) Trace.WriteLine("AutoFillCommand boxes:" + box);

			foreach (Rect r in boxes)
				_commandFactory.Create<AddBoxCommand>(c => c.Rectangle = r).TryExecute();
		}
	}
}