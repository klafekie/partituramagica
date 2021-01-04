#region usings

using System.Windows;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.App
{
	public class EditingController : ICommandParameters
	{
		private readonly EditedPartitura _active;

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
			_active = new EditedPartitura(FileSystem);
		}

		public IEditedPartitura Active => _active;

		public IDrawingPanel Panel { get; set; }

		public IFileSystem FileSystem { get; }

		public Shape GetShapeAt(Point currentPos)
		{
			return Active.IsOpen ? CurrentPage.GetShapeTypeAt(currentPos) : Shape.NothingSpecial;
		}

		public IPlayedPartitura CreatePlayedPartitura()
		{
			if (_active == null || !_active.IsOpen) return null;
			return _active.CreatePlayedPartitura();
		}
	}
}