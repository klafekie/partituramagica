#region usings

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PartituraMagica.Domain;
using PartituraMagica.Playing;

#endregion

namespace PartituraMagica.Player
{
	internal class PlayerViewFactory : IPlayerViewFactory, IDisposable
	{
		private readonly IFileSystem _fileSystem;
		private readonly Func<IPlayerView> _getInstance;
		private IPlayerView _playerView;

		public PlayerViewFactory(MainWindow mainWindow, IImageLoader imageLoader,
			IFileSystem fileSystem, Func<IPlayerView> getInstance)
		{
			_fileSystem = fileSystem;
			_getInstance = getInstance;
			_getInstance = getInstance;
		}

		public void Dispose()
		{
			_playerView?.Dispose();
			_playerView = null;
		}

		public void CreateAndShowView(IPlayedPartitura partitura)
		{
			_playerView = _getInstance();
			_playerView.Closed += (o, args) => { SavePlayer(_playerView, partitura.RuntimePath); };

			_playerView.StartScrolling(partitura, LoadPlayer(partitura));
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