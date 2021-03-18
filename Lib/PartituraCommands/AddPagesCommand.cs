#region usings

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.PageEditing;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class AddPagesCommand : PartituraCommandBase, ICommand
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IMainWindow _mainWindow;
		private readonly ITextureCache _textureCache;

		public AddPagesCommand(ICommandParameters p, ICommandFactory commandFactory, IMainWindow mainWindow,
			ITextureCache textureCache)
			: base(p)
		{
			_commandFactory = commandFactory;
			_mainWindow = mainWindow;
			_textureCache = textureCache;
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
					string destinationFile = P.Active.GetImagePath(index);
					Debug.Assert(sourceFile != destinationFile,
						"cause the index of destFile is always higher than the index of all existing image files");
					P.FileSystem.Copy(sourceFile, destinationFile);
					_textureCache.AddImage(destinationFile);
					_commandFactory.Create<ShowPageCommand>(c => c.Index = index).TryExecute();

					if (P.Active.CurrentPageIndex == index)
						//show was successfull
						_commandFactory.Create<AutoFillCommand>().TryExecute();
				}

				//go back to the first added page
				_commandFactory.Create<ShowPageCommand>(c => c.Index = firstAddedIndex).TryExecute();
			}

			_mainWindow.ShowPartituraInfo(P.Active);
		}
	}
}