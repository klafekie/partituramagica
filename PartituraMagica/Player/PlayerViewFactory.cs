#region usings

using System;
using System.Diagnostics;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.App.Player
{
	internal class PlayerViewFactory : IPlayerViewFactory, IDisposable
	{
		private readonly Func<IPlayerView> _getViewInstance;
		private readonly IPlayerController _playerController;
		private bool _disposed;
		private IPlayerView _playerView;

		public PlayerViewFactory(Func<IPlayerView> getViewInstance,
			IPlayerController playerController)
		{
			_getViewInstance = getViewInstance;
			_playerController = playerController;
		}

		public void Dispose()
		{
			Trace.TraceInformation("disposing PlayerFactory");
			_playerView?.Dispose();
			_playerView = null;
			_disposed = true;
		}

		public void Create()
		{
			try
			{
				if (!_disposed)
				{
					_playerView?.Dispose();
					_playerView = _getViewInstance();
					_playerView.Init();
				}
			}
			catch (Exception ex)
			{
				Trace.TraceError("Error in Create: " + ex);
			}
		}

		public void CreateAndShowView(IPlayedPartitura partitura)
		{
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