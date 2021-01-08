#region usings

using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.PartituraCommands;

#endregion

namespace PartituraMagica.Test
{
	[TestFixture]
	internal class PartituraTests : TestUtils
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();
			TestedPartitura = new EditedPartitura(FileSystem);
		}

		[TearDown]
		public virtual void TearDown()
		{
			//TODO p2 mocks
		}

		public override IEditedPartitura Active => TestedPartitura;


		private IEditedPartitura TestedPartitura;


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
				.Returns("c:\\partituraFile.partitura.json");
			//NewCommand calls AddPageCommand
			fileSystem.Setup(fs => fs.ShowOpenImagesDialog())
				.Returns(new List<string> {"d:\\image1.jpg", "d:\\image2.jpg"});

			Factory.Create<NewPartituraCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-000.jpg",
				"first added file is immediately displayed");
			Assert.NotNull(TestedPartitura.Partitura);

			Assert.AreEqual(TestedPartitura.GetPageCount(), 2);
			Assert.AreEqual(TestedPartitura.Path, "c:\\partituraFile.partitura.json");

			//manually add two more pages
			fileSystem.Setup(fs => fs.ShowOpenImagesDialog())
				.Returns(new List<string> {"d:\\image3.jpg", "d:\\image4.jpg"});
			Factory.Create<AddPagesCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-002.jpg",
				"last added page is immediately displayed");
			Assert.AreEqual(TestedPartitura.GetPageCount(), 4);

			//switch between the pages to see that they have the correcct indices
			Factory.Create<ShowFirstPageCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-000.jpg");
			Factory.Create<ShowNextPageCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-001.jpg");
			Factory.Create<ShowNextPageCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-002.jpg");
			Factory.Create<ShowNextPageCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\partituraFile-003.jpg");

			//now safe to another location and reopen to see that there are still the same pages
			fileSystem.Setup(fs => fs.ShowSavePartituraDialog())
				.Returns("c:\\otherDir\\partituraFile.partitura.json");

			StringBuilder sb = new StringBuilder();
			fileSystem.Setup(fs => fs.WriteFile("c:\\otherDir\\partituraFile.partitura.json", It.IsAny<string>()))
				.Callback((string path, string aString) => sb.Append(aString));

			Factory.Create<SaveAsCommand>().Execute();
			Assert.False(TestedPartitura.Partitura.HasChanges);
			Factory.Create<ClosePartituraCommand>().Execute();
			Factory.Create<CloseApplicationCommand>().Execute();

			//OpenCommand
			fileSystem.Setup(fs => fs.ShowOpenPartituraDialog())
				.Returns("c:\\otherDir\\partituraFile.partitura.json");

			fileSystem.Setup(fs => fs.ReadFile("c:\\otherDir\\partituraFile.partitura.json"))
				.Returns(sb.ToString);

			Factory.Create<OpenCommand>().Execute();
			Factory.Create<ShowFirstPageCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-000.jpg");
			Factory.Create<ShowNextPageCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-001.jpg");
			Factory.Create<ShowNextPageCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-002.jpg");
			Factory.Create<ShowNextPageCommand>().Execute();
			Assert.AreEqual(panel.ImagePath, "c:\\otherDir\\partituraFile-003.jpg");
		}

		[Test]
		public void TestNewPartituraCancelled()
		{
			//user presses cancel in fileSystem for the partitura file
			Factory.Create<NewPartituraCommand>().Execute();
			Assert.AreEqual(panel.Rectangles.Count, 0, "panel is cleared");
			Assert.Null(TestedPartitura.Partitura);
			Assert.Null(TestedPartitura.Path);
			Assert.False(Factory.Create<SaveAsCommand>().CanExecute());
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
				.Returns("c:\\partituraFile.partitura.json");


			try
			{
				Factory.Create<NewPartituraCommand>().Execute();
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