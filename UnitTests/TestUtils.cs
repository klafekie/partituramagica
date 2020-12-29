#region usings

using System;
using System.Collections.Generic;
using System.Windows;
using Lamar;
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
		protected ICommandFactory Factory;
		protected Partitura Partitura;
		public FakeDrawingPanel panel;
		public Mock<IActivePartitura> activePartitura;
		public Mock<FakeFileSystem> fileSystem;
		public Mock<IMainWindow> mainWindow;

		public virtual void Setup()
		{
			Partitura = new Partitura();
			panel = new FakeDrawingPanel();
			//set pagewidth and height
			Partitura.GetPage(0).PaintPage(Panel, new Point(1000, 2000),
				new AbcLabelProvider(Partitura));

			fileSystem = new Mock<FakeFileSystem> {CallBase = true};
			mainWindow = new Mock<IMainWindow>();

			var container = new Container(new TestServiceRegistry(this));
			Factory = new CommandFactory(container);
		}

		protected void Update(double oldX, double oldY, double newX, double newY)
		{
			mainWindow.Setup(mw => mw.CurrentMousePosition).Returns(new Point(newX, newY));
			mainWindow.Setup(w => w.PreviousMousePosition).Returns(new Point(oldX, oldY));

			Factory.Create<UpdateBoxCommand>().Execute();
		}

		protected void Update(Shape type, double oldX, double oldY, double newX, double newY)
		{
			AssertShapeAt(oldX, oldY, type);

			mainWindow.Setup(w => w.CurrentMousePosition).Returns(new Point(newX, newY));
			mainWindow.Setup(w => w.PreviousMousePosition).Returns(new Point(oldX, oldY));

			Factory.Create<UpdateBoxCommand>().Execute();
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

			return Factory.Create<UpdateBoxCommand>().CanExecute();
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
			return Factory.Create<AddBoxCommand>();
		}

		protected UpdateBoxCommand CreateUpdateBoxCommand(Point oldPos, Point newPos)
		{
			mainWindow.Setup(w => w.CurrentMousePosition).Returns(newPos);
			mainWindow.Setup(w => w.PreviousMousePosition).Returns(oldPos);

			return Factory.Create<UpdateBoxCommand>();
		}

		protected SplitBoxCommand CreateSplitBoxCommand(Point pos)
		{
			mainWindow.Setup(w => w.CurrentMousePosition).Returns(pos);
			return Factory.Create<SplitBoxCommand>();
		}

		protected SelectBoxCommand CreateSelectItemCommand(Point pos)
		{
			mainWindow.Setup(w => w.CurrentMousePosition).Returns(pos);
			return Factory.Create<SelectBoxCommand>();
		}

		protected void AssertShapeAt(double x, double y, Shape type)
		{
			Assert.AreEqual(
				Active.CurrentPage.GetShapeTypeAt(new Point(x, y)),
				type);
		}

		public IDrawingPanel Panel
		{
			get => panel;
			set => throw new NotImplementedException();
		}

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

	internal class TestServiceRegistry : ServiceRegistry
	{
		public TestServiceRegistry(TestUtils utils)
		{
			For<IMainWindow>().Use(utils.MainWindow).Singleton();
			For<ICommandParameters>().Use(utils).Singleton();
			Scan(_ =>
			{
				_.LookForRegistries();

				_.Assembly("ClassLibrary1");
				_.Assembly("UnitTests");

				_.Include(type => type.Name.EndsWith("Command") || type.Name.EndsWith("ServiceRegistry"));
				_.AddAllTypesOf<ICommand>().NameBy(x => x.Name.Replace("Command", ""));
				//_.WithDefaultConventions(ServiceLifetime.Singleton);
			});
		}
	}
}