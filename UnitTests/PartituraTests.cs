#region usings

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using PartituraMagica.Domain;
using PartituraMagica.PartituraCommands;
using Rhino.Mocks;

#endregion

namespace PartituraMagica
{
	[TestFixture]
	internal class PartituraTests : TestUtils
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();
			ActivePartitura = new ActivePartitura(fileSystem);
		}

		[TearDown]
		public virtual void TearDown()
		{
			Mocks.VerifyAll();
		}


		public void RunAll()
		{
			Setup();
			TestNewPartituraCancelled();
			TearDown();
			Setup();
			TestNewPartituraAndSave();
			TearDown();
			Setup();
			TearDown();
			Setup();
			TestWrongImageFileIndicesOnNew();
			TearDown();
		}

		[Test]
		public void TestNewPartituraAndSave()
		{
			//user chooses a partitura file he is queried for the image files

			//NewCommand
			Expect.Call(fileSystem.ShowSavePartituraDialog(false))
				.Return("c:\\partituraFile.partitura");

			//NewCommand calls AddCommand
			Expect.Call(fileSystem.ShowOpenImagesDialog())
				.Return(new List<string> {"d:\\image1.jpg", "d:\\image2.jpg"});

			//manually add two more pages
			Expect.Call(fileSystem.ShowOpenImagesDialog())
				.Return(new List<string> {"d:\\image3.jpg", "d:\\image4.jpg"});

			//SaveAsCommand
			Expect.Call(fileSystem.ShowSavePartituraDialog(false))
				.Return("c:\\otherDir\\partituraFile.partitura");
			byte[] buffer = new byte[100000]; //uah, only ok cause it is test code...
			Stream s1 = new MemoryStream(buffer);
			Stream s2 = new MemoryStream(buffer);

			Expect.Call(fileSystem.OpenWrite("c:\\otherDir\\partituraFile.partitura"))
				.Return(s1);

			//OpenCommand
			Expect.Call(fileSystem.ShowOpenPartituraDialog())
				.Return("c:\\otherDir\\partituraFile.partitura");
			Expect.Call(fileSystem.OpenRead("c:\\otherDir\\partituraFile.partitura"))
				.Return(s2);

			Mocks.ReplayAll();

			new NewPartituraCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-000.jpg",
				"first added file is immediately displayed");
			Assert.NotNull(ActivePartitura.Partitura);
			Assert.AreEqual(ActivePartitura.GetPageCount(), 2);
			Assert.AreEqual(ActivePartitura.Path, "c:\\partituraFile.partitura");

			new AddPagesCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-002.jpg",
				"last added page is immediately displayed");
			Assert.AreEqual(ActivePartitura.GetPageCount(), 4);

			////switch between the pages to see that they have the correcct indices
			new ShowPageCommand(0, this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-000.jpg");
			new ShowPageCommand(1, this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-001.jpg");
			new ShowPageCommand(2, this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-002.jpg");
			new ShowPageCommand(3, this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-003.jpg");

			//now safe to another location and reopen to see that there are still the same pages
			new SaveCommand(true, this).Execute();
			Assert.False(ActivePartitura.Partitura.Dirty);
			new ClosePartituraCommand(this).Execute();
			new CloseApplicationCommand(this).Execute();

			new OpenCommand(this).Execute();
			new ShowPageCommand(0, this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-000.jpg");
			new ShowPageCommand(1, this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-001.jpg");
			new ShowPageCommand(2, this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-002.jpg");
			new ShowPageCommand(3, this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-003.jpg");
		}

		[Test]
		public void TestNewPartituraCancelled()
		{
			//Expect.Call(fileSystem.ShowSavePartituraDialog())
			//    .Return(null);

			Mocks.ReplayAll();
			//user presses cancel in fileSystem for the partitura file
			new NewPartituraCommand(this).Execute();
			Assert.AreEqual(panel.Rectangles.Count, 0, "panel is cleared");
			Assert.Null(ActivePartitura.Partitura);
			Assert.Null(ActivePartitura.Path);
			Assert.False(new SaveCommand(true, this).CanExecute());
		}

		[Test]
		public void TestWrongImageFileIndicesOnNew()
		{
			//user chooses a partitura file and the directory contains 
			//some matching image files, but with wrong indices

			fileSystem.GetNames("c:\\").Add("c:\\partituraFile-005.jpg");
			fileSystem.GetNames("c:\\").Add("c:\\partituraFile-000.jpg");

			//NewCommand
			Expect.Call(fileSystem.ShowSavePartituraDialog(false))
				.Return("c:\\partituraFile.partitura");

			Mocks.ReplayAll();

			try
			{
				new NewPartituraCommand(this).Execute();
				Assert.Fail("expected exception");
			}
			catch (PartituraException pe)
			{
				StringAssert.Contains("index", pe.Message);
			}
		}

		//TestExceptions during copy
	}
}