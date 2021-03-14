#region usings

using System.Collections.Generic;
using System.Text.Json;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.Lib.PartituraCommands
{
	public class OpenCommand : PartituraCommandBase, ICommand
	{
		private readonly ICommandFactory _commandFactory;
		private readonly IMainWindow _mainWindow;
		private readonly ITextureCache _textureCache;
		public string Path { get; set; }

		public OpenCommand(ICommandParameters p, IMainWindow mainWindow, ICommandFactory commandFactory,
			ITextureCache textureCache)
			: base(p)
		{
			_mainWindow = mainWindow;
			_commandFactory = commandFactory;
			_textureCache = textureCache;
		}

		public void Execute()
		{
			string chosenFile = Path ?? P.FileSystem.ShowOpenPartituraDialog();
			if (chosenFile != null)
			{
				_commandFactory.Create<ClosePartituraCommand>().TryExecute();
				if (!P.Active.IsOpen)
				{
					string aString = P.FileSystem.ReadFile(chosenFile);
					PartituraJsonMemento mem = JsonSerializer.Deserialize<PartituraJsonMemento>(aString);
					Partitura partitura = new Partitura(mem);
					P.Active.Open(chosenFile, partitura);

					var allImages = new List<string>();
					int pageCount = P.Active.GetPageCount();
					for (int i = 0; i < pageCount; i++) allImages.Add(P.Active.GetImagePath(i));
					foreach (string image in allImages) _textureCache.AddImage(image);

					_commandFactory.Create<ShowFirstPageCommand>().TryExecute();
					_mainWindow.PartPattern = P.Active.Partitura.PartPattern;
					_mainWindow.ShowPartituraInfo(P.Active);
				}
			}
		}

		public bool CanExecute()
		{
			return true;
		}
	}
}