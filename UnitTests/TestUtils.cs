#region usings

using System;
using System.Collections.Generic;
using System.Windows;
using Moq;
using NUnit.Framework;
using PartituraMagica.Domain;
using PartituraMagica.PageEditing;

#endregion

namespace PartituraMagica
{
	[TestFixture]
	internal class TestUtils : ICommandParameters
	{
		protected Partitura Partitura;
		protected FakeDrawingPanel panel;
		protected Mock<IActivePartitura> activePartitura;
		protected Mock<FakeFileSystem> fileSystem;
		protected Mock<IMainWindow> mainWindow;

		public virtual void Setup()
		{
			Partitura = new Partitura();
			panel = new FakeDrawingPanel();
			//set pagewidth and height
			Partitura.GetPage(0).PaintPage(panel, new Point(1000, 2000),
				new AbcLabelProvider(Partitura));

			fileSystem = new Mock<FakeFileSystem> {CallBase = true};
			mainWindow = new Mock<IMainWindow>();
		}

		protected void Update(double oldX, double oldY, double newX, double newY)
		{
			mainWindow.Setup(mw => mw.CurrentMousePosition).Returns(new Point(newX, newY));
			mainWindow.Setup(w => w.PreviousMousePosition).Returns(new Point(oldX, oldY));

			new UpdateBoxCommand(this).Execute();
		}

		protected void Update(Shape type, double oldX, double oldY, double newX, double newY)
		{
			AssertShapeAt(oldX, oldY, type);

			mainWindow.Setup(w => w.CurrentMousePosition).Returns(new Point(newX, newY));
			mainWindow.Setup(w => w.PreviousMousePosition).Returns(new Point(oldX, oldY));

			new UpdateBoxCommand(this).Execute();
		}

		protected void AssertPanelContains(Rect r)
		{
			CollectionAssert.Contains(panel.Rectangles.Values, r);
		}

		protected void AssertPanelContains(Rect r, string label)
		{
			CollectionAssert.Contains(panel.Rectangles.Values, r);
			int id = -1;
			foreach (KeyValuePair<int, Rect> entry in panel.Rectangles)
				if (entry.Value == r)
				{
					id = entry.Key;
					break;
				}

			Assert.AreEqual(label, panel.Labels[id]);
		}

		protected bool CanUpdate(double oldX, double oldY, double newX, double newY)
		{
			mainWindow.Setup(w => w.CurrentMousePosition).Returns(new Point(newX, newY));
			mainWindow.Setup(w => w.PreviousMousePosition).Returns(new Point(oldX, oldY));

			return new UpdateBoxCommand(this).CanExecute();
		}

		protected bool CanUpdate(Shape type,
			double oldX, double oldY, double newX, double newY)
		{
			AssertShapeAt(oldX, oldY, type);
			return CanUpdate(oldX, oldY, newX, newY);
		}

		protected AddBoxCommand CreateAddBoxCommand(Point pos)
		{
			mainWindow.Setup(w => w.CurrentMousePosition).Returns(pos);

			return new AddBoxCommand(this);
		}

		protected UpdateBoxCommand CreateUpdateBoxCommand(Point oldPos, Point newPos)
		{
			mainWindow.Setup(w => w.CurrentMousePosition).Returns(newPos);
			mainWindow.Setup(w => w.PreviousMousePosition).Returns(oldPos);

			return new UpdateBoxCommand(this);
		}

		protected SplitBoxCommand CreateSplitBoxCommand(Point pos)
		{
			mainWindow.Setup(w => w.CurrentMousePosition).Returns(pos);

			return new SplitBoxCommand(this);
		}


		protected SelectItemCommand CreateSelectItemCommand(Point pos)
		{
			mainWindow.Setup(w => w.CurrentMousePosition).Returns(pos);

			return new SelectItemCommand(this);
		}

		protected void AssertShapeAt(double x, double y, Shape type)
		{
			Assert.AreEqual(
				Active.CurrentPage.GetShapeTypeAt(new Point(x, y)),
				type);
		}

		public IDrawingPanel Panel => panel;

		public virtual IActivePartitura Active => activePartitura.Object;

		public IFileSystem FileSystem => fileSystem.Object;


		public IMainWindow MainWindow => mainWindow.Object;


		public void DisposePlayerView()
		{
			//do nothing
		}

		public Shape GetShapeAt(Point currentPos)
		{
			throw new NotImplementedException();
		}
	}
}