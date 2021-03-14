#region usings

using System.Diagnostics;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class ClosePartituraCommand : PartituraCommandBase, ICommand
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IMainWindow _mainWindow;
		private readonly ITextureCache _textureCache;

		public ClosePartituraCommand(ICommandParameters p, IMainWindow mainWindow, ICommandFactory commandFactory,
			ITextureCache textureCache)
			: base(p)
		{
			_mainWindow = mainWindow;
			_commandFactory = commandFactory;
			_textureCache = textureCache;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			DialogResult c = DialogResult.No;
			ICommand cmd = _commandFactory.Create<SaveCommand>();
			if (cmd.CanExecute())
			{
				c = P.FileSystem.AskToSave();
				if (c == DialogResult.Cancel) return;
				if (c == DialogResult.Yes) cmd.Execute();
			}

			if (P.Active.IsOpen && (!P.Active.Partitura.HasChanges || c == DialogResult.No))
			{
				//file was really safed or user doesn't care
				P.Panel.Clear();
				P.Active.Close();
				_textureCache.ClearAllImages();

				_mainWindow.ClearPartituraInfo();
				_mainWindow.PartPattern = string.Empty;
			}
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen;
		}
	}
}