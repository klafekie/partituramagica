#region usings

using System;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.App.Player
{
	internal class PlayerViewFactory : IPlayerViewFactory, IDisposable
	{
		private readonly Func<IPlayerView> _getViewInstance;
		private readonly IPlayerController _playerController;
		private IPlayerView _playerView;

		public PlayerViewFactory(Func<IPlayerView> getViewInstance,
			IPlayerController playerController)
		{
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

			try
			{
				_playerView.StartScrolling(partitura, _playerController);
			}
			catch
			{
				_playerView.Close();
				throw;
			}
		}

		public bool IsExpectedError(Exception exception)
		{
			return _playerView != null &&
			       _playerView.IsExpectedError(exception);
		}
	}
}