#region usings

using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PartituraMagica.Domain;
using PartituraMagica.PageEditing;

#endregion

namespace PartituraMagica
{
	internal class FakeImage : IGreyImage
	{
		public int Width => 1000;

		public int Height => 3000;

		public BrightnessGetter Brightness { get; set; } = (x, y) => 0D;


		public double DpiX => 96D;

		public double DpiY => 96D;


		public string Path => "any.jpg";
	}

	[TestFixture]
	internal class BoxFinderTests
	{
		[SetUp]
		public void Setup()
		{
			_mocks = new MockRepository(MockBehavior.Default);
			_image = new FakeImage();
		}

		[TearDown]
		public void TearDown()
		{
			_mocks.VerifyAll();
		}

		private MockRepository _mocks;
		private FakeImage _image;

		public void RunAll()
		{
			Setup();
			Blanc();
			TearDown();
			Setup();
			Black();
			TearDown();
			Setup();
			Chess();
			TearDown();
			Setup();
			OneSystem();
			TearDown();
			Setup();
			ThreeSystems();
			TearDown();
			Setup();
			OneBox();
			TearDown();
		}

		public double OneSystemImage(int x, int y)
		{
			//first line at y=200
			//image width should be 1000
			if (y < 200 || y > 282 || x < 100 || x > 900)
				return 1F;
			int mod = y % 20;
			if (mod == 0)
				return 0.45F;
			if (mod == 1)
				return 0.3F;
			if (mod == 2)
				return 0.42F;
			return 1F;
		}

		public double OneSystem6LinesImage(int x, int y)
		{
			//first line at y=200
			if (y < 200 || y > 302)
				return 1F;
			int mod = y % 20;
			if (mod == 0)
				return 0.45F;
			if (mod == 1)
				return 0.3F;
			if (mod == 2)
				return 0.42F;
			return 1F;
		}

		public double ThreeSystemsImage(int x, int y)
		{
			//first line at y=200
			//distance between systems == 100
			//some systems have 6 lines

			if (y < 303)
				return OneSystemImage(x, y);
			if (y < 606)
				return OneSystem6LinesImage(x, y - 303);
			return OneSystemImage(x, y - 606);
		}

		[Test]
		public void Black()
		{
			_image.Brightness = (x, y) => 0F;

			BoxFinder f = new BoxFinder(_image);
			IEnumerable<Rect> rects = f.Execute();
			Assert.AreEqual(rects.Count(), 0);
		}


		[Test]
		public void Blanc()
		{
			_image.Brightness = (x, y) => 1F;

			BoxFinder f = new BoxFinder(_image);
			IEnumerable<Rect> rects = f.Execute();
			Assert.AreEqual(rects.Count(), 0);
		}

		[Test]
		public void BoxesAtTopAndBottom()
		{
			BoxFinder f = new BoxFinder(_image);
			Rect[] systems = new Rect[2]
			{
				new Rect(100, 100, 600, 10),
				new Rect(100, 2900, 600, 13)
			};
			List<Rect> boxes = f.GetBoxes(systems).ToList();
			Assert.AreEqual(boxes.Count, 2);
			Assert.AreEqual(boxes[0].Y, 0D);
			Assert.AreEqual(boxes[0].Bottom, 305.0);

			Assert.AreEqual(boxes[1].Y, 2705.0);
			Assert.AreEqual(boxes[1].Bottom, 2999);
		}

		[Test]
		public void Chess()
		{
			_image.Brightness = (x, y) => (x % 2 == 0) ^ (y % 2 == 0) ? 1F : 0F;

			BoxFinder f = new BoxFinder(_image);
			IEnumerable<Rect> rects = f.Execute();
			Assert.AreEqual(rects.Count(), 0);
		}

		[Test]
		public void FourBoxes()
		{
			BoxFinder f = new BoxFinder(_image);
			Rect[] systems = new Rect[4]
			{
				new Rect(100, 300, 600, 10),
				new Rect(100, 500, 600, 11),
				new Rect(100, 1000, 600, 12),
				new Rect(100, 1200, 600, 13)
			};
			List<Rect> boxes = f.GetBoxes(systems).ToList();
			Assert.AreEqual(boxes.Count, 4);
			Assert.AreEqual(boxes[0].Y, 150);
			Assert.AreEqual(boxes[0].Bottom, 396);

			Assert.AreEqual(boxes[1].Y, 414);
			Assert.AreEqual(boxes[1].Bottom, 706.0);

			Assert.AreEqual(boxes[2].Y, 805);
			Assert.AreEqual(boxes[2].Bottom, 1097);

			Assert.AreEqual(boxes[3].Y, 1115);
			Assert.AreEqual(boxes[3].Bottom, 1363.0);
		}

		[Test]
		public void OneBox()
		{
			BoxFinder f = new BoxFinder(_image);
			Rect[] systems = new Rect[1] {new Rect(100, 300, 600, 10)};
			List<Rect> boxes = f.GetBoxes(systems).ToList();
			Assert.AreEqual(boxes.Count, 1);
			Assert.AreEqual(boxes[0].Y, 300 - 150);
			Assert.AreEqual(boxes[0].Bottom, 300 + 10 + 150);

			//one system, but right at the top
			systems = new Rect[1] {new Rect(100, 100, 600, 10)};
			boxes = f.GetBoxes(systems).ToList();
			Assert.AreEqual(boxes.Count, 1);
			Assert.AreEqual(boxes[0].Y, 0);
			Assert.AreEqual(boxes[0].Bottom, 100 + 10 + 150);

			//one system, but right at the bottom of the page
			systems = new Rect[1] {new Rect(100, 2700, 600, 200)};
			boxes = f.GetBoxes(systems).ToList();
			Assert.AreEqual(boxes.Count, 1);
			Assert.AreEqual(boxes[0].Y, 2700 - 150);
			Assert.AreEqual(boxes[0].Bottom, _image.Height - 1);
		}

		[Test]
		public void OneSystem()
		{
			_image.Brightness = OneSystemImage;

			BoxFinder f = new BoxFinder(_image);
			List<Rect> lines = f.DetectSystems().ToList();
			Assert.AreEqual(lines.Count, 1);

			Assert.AreEqual(lines[0].Y, 201);
			Assert.AreEqual(lines[0].Height, 82);
			Assert.AreEqual(lines[0].X, 94);
			Assert.AreEqual(lines[0].Right, 907);
		}

		[Test]
		public void ThreeSystems()
		{
			_image.Brightness = ThreeSystemsImage;

			BoxFinder f = new BoxFinder(_image);
			List<Rect> lines = f.DetectSystems().ToList();
			Assert.AreEqual(lines.Count, 3);

			Assert.AreEqual(lines[1].X, 0, "No edge found, use whole image width");
			Assert.AreEqual(lines[1].Right, 1000);
			Assert.AreEqual(lines[1].Height, 102);

			Assert.AreEqual(lines[2].Height, 82);
		}
	}
}