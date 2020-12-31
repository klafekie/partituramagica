#region usings

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Interop;
using PartituraMagica.Domain;
using PartituraMagica.Playing;

#endregion

namespace PartituraMagica.Player
{
	internal class PlayerViewFactory : IPlayerViewFactory, IDisposable
	{
		private readonly IFileSystem _fileSystem;
		private readonly IImageLoader _imageLoader;
		private readonly MainWindow _mainWindow;
		private PlayerViewD3D _playerView;

		public PlayerViewFactory(MainWindow mainWindow, IImageLoader imageLoader,
			IFileSystem fileSystem)
		{
			_mainWindow = mainWindow;
			_imageLoader = imageLoader;
			_fileSystem = fileSystem;
		}

		public void Dispose()
		{
			_playerView?.Dispose();
		}

		public void CreateAndShowView(IPlayedPartitura partitura)
		{
			WindowInteropHelper h = new WindowInteropHelper(_mainWindow);

			var positionHolder = new PlayerPositionHolder();
			var navigator = new PartNavigator(partitura, positionHolder);

			_playerView = new PlayerViewD3D(partitura, _imageLoader, h.Handle, LoadPlayer(partitura),
				positionHolder, navigator);
			_playerView.Closed += (o, args) => { SavePlayer(_playerView, partitura.RuntimePath); };

			navigator.PlayerOpened(_playerView);
		}

		private PlayerMemento LoadPlayer(IPlayedPartitura partitura)
		{
			if (!_fileSystem.FileExists(partitura.RuntimePath)) return new PlayerMemento();
			using (Stream str = _fileSystem.OpenRead(partitura.RuntimePath))
			{
				BinaryFormatter sf = new BinaryFormatter();
				return (PlayerMemento) sf.Deserialize(str);
			}
		}

		private void SavePlayer(IPlayerView view, string runtimePath)
		{
			using (Stream str = _fileSystem.OpenWrite(runtimePath))
			{
				BinaryFormatter sf = new BinaryFormatter();
				sf.Serialize(str, view.ToMemento());
			}
		}
	}
}