#region usings

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PartituraMagica.Domain;
using PartituraMagica.Playing;

#endregion

#pragma warning disable 414

namespace PartituraMagica
{
	[TestFixture]
	internal class NavigatorTests
	{
		[SetUp]
		public void Setup()
		{
			_mocks = new MockRepository(MockBehavior.Default);
			_view = new Mock<IPlayerView>();
		}

		[TearDown]
		public void TearDown()
		{
			_mocks.VerifyAll();
		}

		private MockRepository _mocks;
		private Mock<IPlayerView> _view;
		private static readonly List<Part> ThreeParts;
		private static readonly string Abccbba = "ABCCBBA";
		private static readonly double LA;
		private static readonly double LB;
		private static readonly double LC;

		static NavigatorTests()
		{
			Part a = new Part();
			a.Add(new Clipping(0, 10, 10, 100, 10));
			a.Add(new Clipping(0, 110, 10, 100, 10)); //box is split, but not labelled
			a.Add(new Clipping(0, 10, 20, 100, 10));
			LA = 300;

			Part b = new Part(); //part b stretches over two pages
			b.Add(new Clipping(0, 10, 30, 10, 10));
			b.Add(new Clipping(1, 11, 10, 10, 10));
			b.Add(new Clipping(1, 11, 30, 10, 10));
			b.Add(new Clipping(2, 12, 30, 10, 10));
			LB = 40;

			Part c = new Part(); //part c consists of a single split box
			c.Add(new Clipping(2, 22, 30, 10, 10));
			LC = 10;

			ThreeParts = new List<Part>();
			ThreeParts.Add(a);
			ThreeParts.Add(b);
			ThreeParts.Add(c);
		}

		public void RunAll()
		{
			Setup();
			EmptyPartsTest();
			TearDown();
			Setup();
			OnePartPatternTest();
			TearDown();
			Setup();
			NextPartTest();
			TearDown();
		}

		[Test]
		public void EmptyPartsTest()
		{
			try
			{
				PartNavigator n = new PartNavigator(Abccbba, new List<Part>(), new PlayerPositionHolder());
				n.PlayerOpened(_view.Object);
				Assert.Fail("Expected Exception");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		[Test]
		[Ignore]
		public void NextPartTest()
		{
			//view.AppendLine(null);
			//TODO LastCall.IgnoreArguments().Repeat.Times(3 + 4 + 2 * 1 + 2 * 4 + 3);


			//we "scroll" to the middle of each part before jumping to the next
			//double pos = 0;
			//view.StartScrolling(pos);//A
			//Expect.Call(view.XPos).Return(pos + lA / 2.0);
			//pos += lA;
			//view.StartScrolling(pos);//AB
			//Expect.Call(view.XPos).Return(pos + lB / 2.0);
			//pos += lB;
			//view.StartScrolling(pos);//ABC
			//Expect.Call(view.XPos).Return(pos + lC / 2.0);
			//pos += lC;
			//view.StartScrolling(pos);//ABCC
			//Expect.Call(view.XPos).Return(pos + lC / 2.0);
			//pos += lC;
			//view.StartScrolling(pos);//ABCCB
			//Expect.Call(view.XPos).Return(pos + lC / 2.0);
			//pos += lB;
			//view.StartScrolling(pos);//ABCCBB
			//Expect.Call(view.XPos).Return(pos + lB / 2.0);
			//pos += lB;
			//view.StartScrolling(pos);//ABCCBBA			
			////trying to navigate after last part, no effect...			
			//Expect.Call(view.XPos).Return(pos + lA / 2.0);

			//Expect.Call(view.XPos).Return(pos + lA / 2.0 + lA / 3.0);

			////now we go backwards
			//view.StartScrolling(pos);//ABCCBBA
			////after ten pixels user goes backward again
			//Expect.Call(view.XPos).Return(pos + 10);
			//pos -= lB;
			//view.StartScrolling(pos);//ABCCBB
			//Expect.Call(view.XPos).Return(pos + 10);
			//pos -= lB;
			//view.StartScrolling(pos);//ABCCB
			//Expect.Call(view.XPos).Return(pos + 4);
			//pos -= lC;
			//view.StartScrolling(pos);//ABCC
			//Expect.Call(view.XPos).Return(pos + 4);//you have to be fast, cause part C has only 10 pixels
			//pos -= lC;
			//view.StartScrolling(pos);//ABC
			//Expect.Call(view.XPos).Return(pos + 4);
			//pos -= lB;
			//view.StartScrolling(pos);//AB
			//Expect.Call(view.XPos).Return(pos + 10);
			//pos -= lA;
			//view.StartScrolling(0);//A


			//
			//Navigator n = new Navigator(ABCCBBA, threeParts, this.view);
			//n.Start();
			//n.GoToNextPart();
			//n.GoToNextPart();
			//n.GoToNextPart();
			//n.GoToNextPart();
			//n.GoToNextPart();
			//n.GoToNextPart();

			////try to go after last part -> no effect
			//n.GoToNextPart();

			//n.GoToPreviousPart();
			//n.GoToPreviousPart();
			//n.GoToPreviousPart();
			//n.GoToPreviousPart();
			//n.GoToPreviousPart();
			//n.GoToPreviousPart();

			//////try to go before first part -> no effect
			//n.GoToPreviousPart();
		}

		[Test]
		[Ignore]
		public void OnePartPatternTest()
		{
			//view.AppendLine(null);
			//TODO LastCall.IgnoreArguments().Repeat.Times(3);

			PartNavigator n = new PartNavigator("A", ThreeParts, new PlayerPositionHolder());
			n.PlayerOpened(_view.Object);
		}
	}
}