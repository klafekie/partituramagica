//using System;
//using System.Windows;
//using System.Collections.Generic;
//using System.Text;

//using NUnit.Framework;
//using Rhino.Mocks;
//using System.IO;
//using PartituraMagica.Playing;

//namespace PartituraMagica.Playing
//{
//    [TestFixture]
//    class PlayingTests : IPlayCommandParameters
//    {		
//        private MockRepository mocks;
//        private IPlayerView view;
//        private Partitura partitura;
//        private IPlayingContext context;
//        private IFileSystem fileSystem;
//        private IDrawingPanel panel;

//        private Page page = null;

//        private Rect r1, r2, r3, r4, r5;

//        [SetUp]
//        public void Setup()
//        {
//            this.mocks = new MockRepository();
//            this.view = mocks.StrictMock<IPlayerView>();
//            this.context = mocks.DynamicMock<IPlayingContext>();
//            this.fileSystem = mocks.DynamicMock<IFileSystem>();
//            this.panel = mocks.DynamicMock<IDrawingPanel>();

//            this.partitura = new Partitura();

//            Expect.Call(this.context.GetPageCount()).Repeat.Any().Return(3);
//            this.partitura.GetPage(0).PaintPage(panel, new Point(1000, 2000),
//                null);
//            this.partitura.GetPage(1).PaintPage(panel, new Point(1000, 2000),
//                null);
//            this.partitura.GetPage(2).PaintPage(panel, new Point(1000, 2000),
//                null);


//            r1 = new Rect(90,90,50,10);
//            r2 = new Rect(90,110,50,10);
//            r3 = new Rect(90,130,50,10);
//            r4 = new Rect(90,150,50,10);
//            r5 = new Rect(90,170,50,10);						
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            mocks.VerifyAll();
//        }

//        public void RunAll()
//        {
//            this.Setup();
//            this.BoxesAreSortedTest();
//            this.TearDown();
//            this.Setup();
//            this.MultiplePagesTest();
//            this.TearDown();
//            this.Setup();
//            this.EachBoxIsAPart();
//            this.TearDown();
//        }

//        [Test]
//        public void BoxesAreSortedTest() 
//        {		

//            this.mocks.ReplayAll();

//            //boxes are not added in ascending x order
//            this.partitura.AddBox(0, r2, this.panel);
//            this.partitura.AddBox(0, r1, this.panel);
//            this.partitura.AddBox(0, r5, this.panel);
//            this.partitura.AddBox(0, r3, this.panel);
//            this.partitura.AddBox(0, r4, this.panel);

//            IList<Part> parts = this.partitura.GetAllParts(2);

//            // rects are returned in order top to bottom
//            Assert.AreEqual(r1, parts[0][0].Coords);
//            Assert.AreEqual(r2, parts[0][1].Coords);
//            Assert.AreEqual(r3, parts[0][2].Coords);
//            Assert.AreEqual(r4, parts[0][3].Coords);
//            Assert.AreEqual(r5, parts[0][4].Coords);
//        }

//        [Test]
//        public void MultiplePagesTest() 
//        {			
//            this.mocks.ReplayAll();

//            //boxes are added on different pages without order
//            this.partitura.AddBox(0, r2, this.panel);
//            this.partitura.AddBox(1,  r1, this.panel);
//            this.partitura.AddBox(1,  r5, this.panel);
//            this.partitura.AddBox(1,  r3, this.panel);
//            this.partitura.AddBox(0, r4, this.panel);

//            IList<Part> parts = this.partitura.GetAllParts(2);

//            // rects are played in order of pages and top to bottom
//            Assert.AreEqual(r2, parts[0][0].Coords);
//            Assert.AreEqual(0, parts[0][0].PageIndex);

//            Assert.AreEqual(r4, parts[0][1].Coords);
//            Assert.AreEqual(0, parts[0][1].PageIndex);

//            Assert.AreEqual(r1, parts[0][2].Coords);
//            Assert.AreEqual(1, parts[0][2].PageIndex);

//            Assert.AreEqual(r3, parts[0][3].Coords);
//            Assert.AreEqual(1, parts[0][3].PageIndex);

//            Assert.AreEqual(r5, parts[0][4].Coords);
//            Assert.AreEqual(1, parts[0][4].PageIndex);
//        }

//        [Test]
//        public void EachBoxIsAPart() 
//        {
//            mocks.ReplayAll();

//            ILabelProvider labeller = new ABCLabelProvider(this.partitura);

//            this.partitura.AddBox(0, r1, this.panel);
//            this.partitura.AddBox(0, r2, this.panel);
//            this.partitura.AddBox(1,  r3, this.panel);
//            this.partitura.AddBox(1,  r4, this.panel);
//            this.partitura.AddBox(2, r5, this.panel);


//            Label(0, new Point(r1.X, r1.Y + 5), labeller);
//            Label(0, new Point(r2.X, r2.Y + 5), labeller);
//            Label(1, new Point(r3.X, r3.Y + 5), labeller);
//            Label(1, new Point(r4.X, r4.Y + 5), labeller);
//            Label(2, new Point(r5.X, r5.Y + 5), labeller);

//            IList<Part> parts = this.partitura.GetAllParts(3);

//            Assert.AreEqual(5, parts.Count);
//            Assert.AreEqual(r1, parts[0][0].Coords);
//            Assert.AreEqual(0, parts[0][0].PageIndex);

//            Assert.AreEqual(r2, parts[1][0].Coords);
//            Assert.AreEqual(0, parts[1][0].PageIndex);

//            Assert.AreEqual(r3, parts[2][0].Coords);
//            Assert.AreEqual(1, parts[2][0].PageIndex);

//            Assert.AreEqual(r4, parts[3][0].Coords);
//            Assert.AreEqual(1, parts[3][0].PageIndex);

//            Assert.AreEqual(r5, parts[4][0].Coords);
//            Assert.AreEqual(2, parts[4][0].PageIndex);
//        }		

//        private void Label(int pageIndex, Point pos, ILabelProvider labeller)
//        {	
//            Page p = this.partitura.GetPage(pageIndex);
//            p.SelectBoxAt(pos, this.panel);
//            labeller.PageIndex = pageIndex;
//            p.LabelSelection(panel,	labeller);
//        }

//        public IPlayerView View
//        {
//            get
//            {
//                return this.view;
//            }
//        }

//        public IPlayablePartitura Partitura
//        {
//            get
//            {
//                return this.partitura;
//            }
//        }

//        public IPlayingContext Context
//        {
//            get
//            {
//                return this.context;
//            }
//        }

//        public IFileSystem FileSystem
//        {
//            get
//            {
//                return this.fileSystem;
//            }
//        }

//        public IDrawingPanel Panel
//        {
//            get
//            {
//                return this.panel;
//            }
//        }

//        public Page CurrentPage
//        {
//            get
//            {
//                return this.page;
//            }
//        }


//        public INavigator Navigator
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }
//    }
//}

