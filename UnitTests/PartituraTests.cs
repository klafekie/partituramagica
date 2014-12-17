using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;
using System.IO;

namespace PartituraMagica
{
	[TestFixture]
	class PartituraTests : TestUtils
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();
			activePartitura = new ActivePartitura(fileSystem);
		}

		[TearDown]
		public virtual void TearDown()
		{
			mocks.VerifyAll();
		}

		
		public void RunAll()
		{
			this.Setup();
			this.TestNewPartituraCancelled();
			this.TearDown();
			this.Setup();
			this.TestNewPartituraAndSave();
			this.TearDown();
			this.Setup();
			this.TestNewPartituraImagesAlreadyPresent();
			this.TearDown();
			this.Setup();
			this.TestWrongImageFileIndicesOnNew();
			this.TearDown();			
		}

		[Test]
		public void TestNewPartituraCancelled()
		{
			Expect.Call(fileSystem.ShowSaveDialog(FileType.Partitura))
				.Return(null);

			mocks.ReplayAll();
			//user presses cancel in fileSystem for the partitura file
			new NewPartituraCommand(this).Execute();
			Assert.AreEqual(panel.rectangles.Count, 0, "panel is cleared");
			Assert.Null(activePartitura.Partitura);
			Assert.Null(activePartitura.Path);
			Assert.False(new SaveCommand(true, this).CanExecute());
		}

		[Test]
		public void TestNewPartituraAndSave()
		{
			//user chooses a partitura file and the directory contains no
			//matching image files, so he is queried for the first image file		

			//NewCommand
			Expect.Call(fileSystem.ShowSaveDialog(FileType.Partitura))
				.Return("c:\\partituraFile.partitura");

			//NewCommand calls AddCommand
			Expect.Call(fileSystem.ShowOpenDialog(FileType.Image))
				.Return("d:\\image1.jpg");

			//two AddCommand
			Expect.Call(fileSystem.ShowOpenDialog(FileType.Image))
				.Return("d:\\image2.jpg");

			//third AddCommand
			Expect.Call(fileSystem.ShowOpenDialog(FileType.Image))
				.Return("d:\\image3.jpg");

			//SaveAsCommand
			Expect.Call(fileSystem.ShowSaveDialog(FileType.Partitura))
				.Return("c:\\otherDir\\partituraFile.partitura");
			byte[] buffer = new byte[100000];//uah, only ok cause it is test code...
			Stream s1 = new MemoryStream(buffer);
			Stream s2 = new MemoryStream(buffer);

			Expect.Call(fileSystem.OpenWrite("c:\\otherDir\\partituraFile.partitura"))
				.Return(s1);

			//OpenCommand
			Expect.Call(fileSystem.ShowOpenDialog(FileType.Partitura))
				.Return("c:\\otherDir\\partituraFile.partitura");
			Expect.Call(fileSystem.OpenRead("c:\\otherDir\\partituraFile.partitura"))
				.Return(s2);

			mocks.ReplayAll();

			new NewPartituraCommand(this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-000.jpg",
				"first added file is immediately displayed");
			Assert.NotNull(activePartitura.Partitura);
			Assert.AreEqual(activePartitura.GetPageCount(), 1);
			Assert.AreEqual(activePartitura.Path, "c:\\partituraFile.partitura");

			new AddPagesCommand(this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-001.jpg",
				"last added page is immediately displayed");

			new AddPagesCommand(this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-002.jpg",
				"last added page is immediately displayed");

			////switch between the pages to see that they have the correcct indices
			new ShowPageCommand(0, this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-000.jpg");
			new ShowPageCommand(1, this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-001.jpg");
			new ShowPageCommand(2, this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-002.jpg");

			//now safe to another location and reopen to see that there are still the same pages
			new SaveCommand(true, this).Execute();
			Assert.False(activePartitura.Partitura.Dirty);
			new ClosePartituraCommand(this).Execute(); 
			new CloseApplicationCommand(this).Execute();			

			new OpenCommand(this).Execute();
			new ShowPageCommand(0, this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\otherDir\\partituraFile-000.jpg");
			new ShowPageCommand(1, this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\otherDir\\partituraFile-001.jpg");
			new ShowPageCommand(2, this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\otherDir\\partituraFile-002.jpg");
		}

		[Test]
		public void TestNewPartituraImagesAlreadyPresent()
		{
			//user chooses a partitura file and the directory contains 
			//some matching image files

			fileSystem.GetNames("c:\\").Add("c:\\partituraFile-000.jpg");
			fileSystem.GetNames("c:\\").Add("c:\\partituraFile-001.jpg");

			//NewCommand
			Expect.Call(fileSystem.ShowSaveDialog(FileType.Partitura))
				.Return("c:\\partituraFile.partitura");

			//adding a third page
			Expect.Call(fileSystem.ShowOpenDialog(FileType.Image))
				.Return("d:\\image.jpg");

			mocks.ReplayAll();

			new NewPartituraCommand(this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-000.jpg",
				"first file is immediately displayed");
			Assert.NotNull(activePartitura.Partitura);
			Assert.AreEqual(activePartitura.GetPageCount(), 2);
			Assert.AreEqual(activePartitura.Path, "c:\\partituraFile.partitura");

			new AddPagesCommand(this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-002.jpg",
				"last added page is immediately displayed");

			////switch between the pages to see that they have the correcct indices
			new ShowPageCommand(0, this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-000.jpg");
			new ShowPageCommand(1, this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-001.jpg");
			new ShowPageCommand(2, this).Execute();
			Assert.AreEqual(panel.imagePath, "c:\\partituraFile-002.jpg");
		}

		[Test]
		public void TestWrongImageFileIndicesOnNew()
		{
			//user chooses a partitura file and the directory contains 
			//some matching image files, but with wrong indices

			fileSystem.GetNames("c:\\").Add("c:\\partituraFile-005.jpg");
			fileSystem.GetNames("c:\\").Add("c:\\partituraFile-000.jpg");

			//NewCommand
			Expect.Call(fileSystem.ShowSaveDialog(FileType.Partitura))
				.Return("c:\\partituraFile.partitura");

			mocks.ReplayAll();

			try {
				new NewPartituraCommand(this).Execute();
				Assert.Fail("expected exception");
			} catch (PartituraException pe) {
				StringAssert.Contains("index", pe.Message);
			}
		}
		
		//TestExceptions during copy
	}
}