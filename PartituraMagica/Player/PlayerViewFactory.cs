#region usings

using System;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.App.Player
{
	internal class PlayerViewFactory : IPlayerViewFactory, IDisposable
	{
		private readonly IFileSystem _fileSystem;
		private readonly Func<IPlayerView> _getViewInstance;
		private readonly IPlayerController _playerController;
		private IPlayerView _playerView;

		public PlayerViewFactory(MainWindow mainWindow, IImageLoader imageLoader,
			IFileSystem fileSystem, Func<IPlayerView> getViewInstance, IPlayerController playerController)
		{
			_fileSystem = fileSystem;
			_getViewInstance = getViewInstance;
			_playerController = playerController;
		}

		public void Dispose()
		{
			_playerView?.Dispose();
			_playerView = null;
		}

		public void CreateAndShowView(IPlayedPartitura partitura)
		{
			_playerView = _getViewInstance();
			_playerController.Init(partitura, _playerView);

			_playerView.StartScrolling(partitura, _playerController);
		}
	}
}