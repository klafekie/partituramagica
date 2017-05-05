using System.Collections.Generic;
using System.Windows;
using NUnit.Framework;
using Rhino.Mocks;

namespace PartituraMagica
{
    [TestFixture]
    internal class TestUtils : ICommandParameters
    {



        protected Partitura Partitura;
        protected FakeDrawingPanel panel;
        protected IActivePartitura ActivePartitura;
        protected MockRepository Mocks;
        protected FakeFileSystem fileSystem;
        protected IMainWindow mainWindow;

        public virtual void Setup()
        {
            Partitura = new Partitura();
            panel = new FakeDrawingPanel();
            //set pagewidth and height
            Partitura.GetPage(0).PaintPage(panel, new Point(1000, 2000),
                new AbcLabelProvider(Partitura));

            Mocks = new MockRepository();
            fileSystem = Mocks.PartialMock<FakeFileSystem>();
            mainWindow = Mocks.DynamicMock<IMainWindow>();
        }


        protected void Update(double oldX, double oldY, double newX, double newY)
        {
            new UpdateBoxCommand(new Point(oldX, oldY), new Point(newX, newY), this).Execute();
        }

        protected void Update(Shape type, double oldX, double oldY, double newX, double newY)
        {
            AssertShapeAt(oldX, oldY, type);
            new UpdateBoxCommand(new Point(oldX, oldY), new Point(newX, newY), this).Execute();
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
            {
                if (entry.Value == r)
                {
                    id = entry.Key;
                    break;
                }
            }

            Assert.AreEqual(label, panel.Labels[id]);
        }

        protected bool CanUpdate(double oldX, double oldY, double newX, double newY)
        {
            return new UpdateBoxCommand(new Point(oldX, oldY), new Point(newX, newY), this).CanExecute();
        }

        protected bool CanUpdate(Shape type,
            double oldX, double oldY, double newX, double newY)
        {
            AssertShapeAt(oldX, oldY, type);
            return CanUpdate(oldX, oldY, newX, newY);
        }

        protected void AssertShapeAt(double x, double y, Shape type)
        {
            Assert.AreEqual(
                Active.CurrentPage.GetShapeTypeAt(new Point(x, y)),
                type);
        }

        public IDrawingPanel Panel
        {
            get { return panel; }
        }

        public IActivePartitura Active
        {
            get { return ActivePartitura; }
        }

        public IFileSystem FileSystem
        {
            get { return fileSystem; }
        }


        public IMainWindow MainWindow
        {
            get { return mainWindow; }
        }


        public void DisposePlayerView()
        {
            /* Problem: NotImplementedException blocked unit tests during execution of PartituraTests.TestNewPartituraAndSave().
             * Solution: do nothing in this method? [jed 20141226]
             */
            //throw new NotImplementedException();
        }
    }
}