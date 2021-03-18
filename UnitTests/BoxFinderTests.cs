#region usings

using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PartituraMagica.Lib.PageEditing;

#endregion

namespace PartituraMagica.Test
{
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

		[Test]
		public void DefaultValues()
		{
			BoxLayouter f = new BoxLayouter(_image);
			Assert.AreEqual(300, f.DefaultBoxHeight);
			Assert.AreEqual(360, f.MaxBoxHeight);
			Assert.AreEqual(240, f.MinBoxHeight);
		}

		[Test]
		public void OneBox_default()
		{
			BoxLayouter f = new BoxLayouter(_image);
			List<Rect> systems = new List<Rect>
			{
				new Rect(100, 300, 600, 10)
			};
			List<Rect> boxes = f.GetBoxes(systems).ToList();

			Assert.AreEqual(300, f.DefaultBoxHeight);

			Assert.AreEqual(1, boxes.Count);
			Assert.AreEqual(155, boxes[0].CenterY);
			Assert.AreEqual(300 - f.DefaultBoxHeight / 2, boxes[0].Y);
			Assert.AreEqual(300 + 10 + f.DefaultBoxHeight / 2, boxes[0].Bottom);
		}

		[Test]
		public void OneBox_top()
		{
			//one system, but close to the top
			BoxLayouter f = new BoxLayouter(_image);
			var systems = new List<Rect>
			{
				new Rect(100, 100, 600, 10)
			};
			var boxes = f.GetBoxes(systems).ToList();

			Assert.AreEqual(1, boxes.Count);
			Assert.AreEqual(0, boxes[0].Y);
			Assert.AreEqual(100 + 10 + 150, boxes[0].Bottom);
		}

		[Test]
		public void OneBox_bottom()
		{
			//one system, but right at the bottom of the page
			BoxLayouter f = new BoxLayouter(_image);
			var systems = new List<Rect>
			{
				new Rect(100, 2700, 600, 200)
			};
			var boxes = f.GetBoxes(systems).ToList();
			Assert.AreEqual(1, boxes.Count);
			Assert.AreEqual(2700 - 150, boxes[0].Y);
			Assert.AreEqual(_image.Height, boxes[0].Bottom);
		}

		[Test]
		public void TwoBoxes_default()
		{
			//boxes are more than defaultBoxheight appart
			BoxLayouter f = new BoxLayouter(_image);
			List<Rect> systems = new List<Rect>
			{
				new Rect(100, 400, 600, 10),
				new Rect(100, 800, 600, 13)
			};
			List<Rect> boxes = f.GetBoxes(systems).ToList();

			//-> both boxes have default size
			Assert.AreEqual(2, boxes.Count);
			Assert.AreEqual(400D - 150D, boxes[0].Y);
			Assert.AreEqual(400D + 10D + 150D, boxes[0].Bottom);

			Assert.AreEqual(800D - 150D, boxes[1].Y);
			Assert.AreEqual(800D + 13D + 150D, boxes[1].Bottom);
		}

		[Test]
		public void TwoBoxes_near()
		{
			//boxes are too near to each other to use DefaultHeight
			BoxLayouter f = new BoxLayouter(_image);
			List<Rect> systems = new List<Rect>
			{
				new Rect(100, 400, 600, 10),
				new Rect(100, 600, 600, 13)
			};
			var boxes = f.GetBoxes(systems).ToList();

			Assert.AreEqual(2, boxes.Count);
			//-> device available space evenly
			Assert.AreEqual(400 - 150, boxes[0].Y);
			Assert.AreEqual(400 + 10 + 95 - 2 * Page.ClickTolerance, boxes[0].Bottom);

			Assert.AreEqual(600 - 95 + 2d * Page.ClickTolerance, boxes[1].Y);
			Assert.AreEqual(600 + 13 + 150, boxes[1].Bottom);
		}
	}
}