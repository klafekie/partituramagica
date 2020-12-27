#region usings

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using PartituraMagica.Domain;
using PartituraMagica.PartituraCommands;

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
			TestedPartitura = new ActivePartitura(FileSystem);
		}

		[TearDown]
		public virtual void TearDown()
		{
			//TODO verify moc?
		}

		public override IActivePartitura Active => TestedPartitura;


		private IActivePartitura TestedPartitura;


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

			fileSystem.Setup(fs => fs.ShowSavePartituraDialog())
				.Returns("c:\\partituraFile.partitura");
			//NewCommand calls AddPageCommand
			fileSystem.Setup(fs => fs.ShowOpenImagesDialog())
				.Returns(new List<string> {"d:\\image1.jpg", "d:\\image2.jpg"});

			//fileSystem.Setup(fs => fs.GetMatchingFileNames(It.IsAny<string>(), It.IsAny<string>()))
			//	.Returns(new List<string>());

			new NewPartituraCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-000.jpg",
				"first added file is immediately displayed");
			Assert.NotNull(TestedPartitura.Partitura);

			//fileSystem.Setup(fs => fs.GetMatchingFileNames(It.IsAny<string>(), It.IsAny<string>()))
			//	.Returns(new List<string> {"c:\\partituraFile-000.jpg", "c:\\partituraFile-001.jpg"});
			Assert.AreEqual(TestedPartitura.GetPageCount(), 2);
			Assert.AreEqual(TestedPartitura.Path, "c:\\partituraFile.partitura");

			//manually add two more pages
			fileSystem.Setup(fs => fs.ShowOpenImagesDialog())
				.Returns(new List<string> {"d:\\image3.jpg", "d:\\image4.jpg"});
			new AddPagesCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-002.jpg",
				"last added page is immediately displayed");
			Assert.AreEqual(TestedPartitura.GetPageCount(), 4);

			//switch between the pages to see that they have the correcct indices
			new ShowFirstPageCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-000.jpg");
			new ShowNextPageCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-001.jpg");
			new ShowNextPageCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-002.jpg");
			new ShowNextPageCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-003.jpg");

			//now safe to another location and reopen to see that there are still the same pages
			fileSystem.Setup(fs => fs.ShowSavePartituraDialog())
				.Returns("c:\\otherDir\\partituraFile.partitura");
			byte[] buffer = new byte[100000]; //uah, only ok cause it is test code...
			Stream s1 = new MemoryStream(buffer);

			fileSystem.Setup(fs => fs.OpenWrite("c:\\otherDir\\partituraFile.partitura"))
				.Returns(s1);

			new SaveAsCommand(this).Execute();
			Assert.False(TestedPartitura.Partitura.Dirty);
			new ClosePartituraCommand(this).Execute();
			new CloseApplicationCommand(this).Execute();

			//OpenCommand
			fileSystem.Setup(fs => fs.ShowOpenPartituraDialog())
				.Returns("c:\\otherDir\\partituraFile.partitura");
			//use a new stream just because s1 is already read to the end.
			Stream s2 = new MemoryStream(buffer);
			fileSystem.Setup(fs => fs.OpenRead("c:\\otherDir\\partituraFile.partitura"))
				.Returns(s2);

			new OpenCommand(this).Execute();
			new ShowFirstPageCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-000.jpg");
			new ShowNextPageCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-001.jpg");
			new ShowNextPageCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-002.jpg");
			new ShowNextPageCommand(this).Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-003.jpg");
		}

		[Test]
		public void TestNewPartituraCancelled()
		{
			//fileSystem.Setup(fs => fs.ShowSavePartituraDialog())
			//    .Returns(null);


			//user presses cancel in fileSystem for the partitura file
			new NewPartituraCommand(this).Execute();
			Assert.AreEqual(panel.Rectangles.Count, 0, "panel is cleared");
			Assert.Null(TestedPartitura.Partitura);
			Assert.Null(TestedPartitura.Path);
			Assert.False(new SaveAsCommand(this).CanExecute());
		}

		[Test]
		public void TestWrongImageFileIndicesOnNew()
		{
			//user chooses a partitura file and the directory contains 
			//some matching image files, but with wrong indices

			fileSystem.Object.GetNames("c:\\").Add("c:\\partituraFile-005.jpg");
			fileSystem.Object.GetNames("c:\\").Add("c:\\partituraFile-000.jpg");

			//NewCommand
			fileSystem.Setup(fs => fs.ShowSavePartituraDialog())
				.Returns("c:\\partituraFile.partitura");


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