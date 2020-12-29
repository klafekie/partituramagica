#region usings

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class AddPagesCommand : PartituraCommandBase, ICommand
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IMainWindow _mainWindow;

		public AddPagesCommand(ICommandParameters p, ICommandFactory commandFactory, IMainWindow mainWindow)
			: base(p)
		{
			_commandFactory = commandFactory;
			_mainWindow = mainWindow;
		}

		public bool CanExecute()
		{
			return P.Active != null && P.Active.IsOpen;
		}

		public void Execute()
		{
			Debug.Assert(CanExecute());
			ICollection<string> sourceFiles = P.FileSystem.ShowOpenImagesDialog();
			if (sourceFiles.Any())
			{
				int firstAddedIndex = P.Active.GetPageCount();
				foreach (var sourceFile in sourceFiles)
				{
					int index = P.Active.GetPageCount();
					string destFile = P.Active.GetImagePath(index);
					Debug.Assert(sourceFile != destFile,
						"cause the index of destFile is always higher than the index of all existing image files");
					P.FileSystem.Copy(sourceFile, destFile);
					_commandFactory.Create<ShowPageCommand>(c => c.Index = index).TryExecute();
					//if (P.Active.CurrentPageIndex == index)
					//show was successfull
					//TODO fix autofill
					//_commandFactory.Create<AutoFillCommand>().TryExecute();
				}

				//go back to the first added page
				_commandFactory.Create<ShowPageCommand>(c => c.Index = firstAddedIndex).TryExecute();
			}

			_mainWindow.ShowPartituraInfo(P.Active);
		}
	}
}