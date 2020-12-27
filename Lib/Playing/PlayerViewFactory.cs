#region usings

using System;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.Playing
{
	internal class PlayerViewFactory : IPlayerViewFactory
	{
		public IPlayerView CreateView(PlayerPositionHolder positionHolder, IPartNavigator navigator)
		{
			//Func<PlayerPositionHolder, IPartNavigator, IPlayerView> createView =
			//	(positionProvider, playerView) =>
			//	{
			//		WindowInteropHelper h = new WindowInteropHelper(_mainWindow);

			//		_playerView = new PlayerViewD3D(Active, Panel, h.Handle, LoadPlayer(), positionProvider,
			//			playerView);
			//		_playerView.Closed += (o, args) =>
			//		{
			//			SavePlayer(_playerView);
			//			_playerView = null;
			//		};
			//		return _playerView;
			//	};
			throw new NotImplementedException();
		}
	}
}