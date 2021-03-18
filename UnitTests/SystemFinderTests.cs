#region usings

using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PartituraMagica.Lib.PageEditing;

#endregion

namespace PartituraMagica.Test
{
	internal class SystemFinderTests
	{
		private FakeImage _image;

		private MockRepository _mocks;

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

			SystemFinder f = new SystemFinder(_image);
			IEnumerable<Rect> rects = f.DetectSystems();
			Assert.AreEqual(0, rects.Count());
		}

		[Test]
		public void Blanc()
		{
			_image.Brightness = (x, y) => 1F;

			SystemFinder f = new SystemFinder(_image);
			IEnumerable<Rect> rects = f.DetectSystems();
			Assert.AreEqual(0, rects.Count());
		}

		[Test]
		public void Chess()
		{
			_image.Brightness = (x, y) => (x % 2 == 0) ^ (y % 2 == 0) ? 1F : 0F;

			SystemFinder f = new SystemFinder(_image);
			IEnumerable<Rect> rects = f.DetectSystems();
			Assert.AreEqual(0, rects.Count());
		}

		[Test]
		public void OneSystem()
		{
			_image.Brightness = OneSystemImage;

			SystemFinder f = new SystemFinder(_image);
			var lines = f.DetectSystems();
			Assert.AreEqual(1, lines.Count);

			Assert.AreEqual(201, lines[0].Y);
			Assert.AreEqual(82, lines[0].Height);
			Assert.AreEqual(94, lines[0].X);
			Assert.AreEqual(907, lines[0].Right);
		}

		[Test]
		public void ThreeSystems()
		{
			_image.Brightness = ThreeSystemsImage;

			SystemFinder f = new SystemFinder(_image);
			var lines = f.DetectSystems();
			Assert.AreEqual(3, lines.Count);

			Assert.AreEqual(0, lines[1].X, "No edge found, use whole image width");
			Assert.AreEqual(1000, lines[1].Right);
			Assert.AreEqual(102, lines[1].Height);

			Assert.AreEqual(82, lines[2].Height);
		}
	}
}