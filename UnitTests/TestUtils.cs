using System.Collections.Generic;
using System.Windows;
using NUnit.Framework;
using Rhino.Mocks;

namespace PartituraMagica
{
    [TestFixture]
    internal class TestUtils : ICommandParameters
    {
        protected Partitura partitura;
        protected FakeDrawingPanel panel;
        protected IActivePartitura activePartitura;
        protected MockRepository mocks;
        protected FakeFileSystem fileSystem;
        protected IMainWindow mainWindow;

        public virtual void Setup()
        {
            partitura = new Partitura();
            panel = new FakeDrawingPanel();
            //set pagewidth and height
            partitura.GetPage(0).PaintPage(panel, new Point(1000, 2000),
                new ABCLabelProvider(partitura));

            mocks = new MockRepository();
            fileSystem = mocks.PartialMock<FakeFileSystem>();
            mainWindow = mocks.DynamicMock<IMainWindow>();
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
            CollectionAssert.Contains(panel.rectangles.Values, r);
        }

        protected void AssertPanelContains(Rect r, string label)
        {
            CollectionAssert.Contains(panel.rectangles.Values, r);
            int id = -1;
            foreach (KeyValuePair<int, Rect> entry in panel.rectangles)
            {
                if (entry.Value == r)
                {
                    id = entry.Key;
                    break;
                }
            }

            Assert.AreEqual(label, panel.labels[id]);
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
            get { return activePartitura; }
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