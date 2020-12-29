#region usings

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using PartituraMagica.Domain;
using PartituraMagica.Player;

#endregion

namespace PartituraMagica
{
	public class EditingController : ICommandParameters, IDisposable
	{
		private PlayerViewD3D _playerView;

		public Page CurrentPage
		{
			get
			{
				if (Active.Partitura != null)
				{
					int index = Active.CurrentPageIndex;
					return Active.Partitura.GetPage(index);
				}

				return null;
			}
		}

		private string PlayerRuntimePath => Active.Path + ".runtime";

		public EditingController()
		{
			FileSystem = new FileSystem();
			Active = new ActivePartitura(FileSystem);
		}

		public IActivePartitura Active { get; }

		public IDrawingPanel Panel { get; set; }

		public IFileSystem FileSystem { get; }

		public void DisposePlayerView()
		{
			if (_playerView != null)
			{
				_playerView.Dispose();
				_playerView = null;
			}
		}

		public Shape GetShapeAt(Point currentPos)
		{
			return Active.IsOpen ? CurrentPage.GetShapeTypeAt(currentPos) : Shape.NothingSpecial;
		}

		public void Dispose()
		{
			//DisposePlayerView();
		}

		private PlayerMemento LoadPlayer()
		{
			if (!FileSystem.FileExists(PlayerRuntimePath)) return new PlayerMemento();
			using (Stream str = FileSystem.OpenRead(PlayerRuntimePath))
			{
				BinaryFormatter sf = new BinaryFormatter();
				return (PlayerMemento) sf.Deserialize(str);
			}
		}

		private void SavePlayer(IPlayerView view)
		{
			using (Stream str = FileSystem.OpenWrite(PlayerRuntimePath))
			{
				BinaryFormatter sf = new BinaryFormatter();
				sf.Serialize(str, view.ToMemento());
			}
		}
	}
}