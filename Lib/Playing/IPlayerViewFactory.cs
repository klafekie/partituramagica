#region usings

using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.Playing
{
	public interface IPlayerViewFactory
	{
		IPlayerView CreateView(PlayerPositionHolder positionHolder, IPartNavigator navigator);
	}
}