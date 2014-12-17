using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;
using System.IO;
using PartituraMagica.Playing;

#pragma warning disable 414

namespace PartituraMagica
{
	[TestFixture]
	class NavigatorTests
	{
		private MockRepository mocks;
		private IPlayerView view;
		private static readonly List<Part> threeParts;
		private static readonly String ABCCBBA = "ABCCBBA";
		private static readonly double lA;
		private static readonly double lB;
		private static readonly double lC;

		static NavigatorTests()
		{
			Part a = new Part();
			a.Add(new Clipping(0, 10, 10, 100, 10));
			a.Add(new Clipping(0, 110, 10, 100, 10));//box is split, but not labelled
			a.Add(new Clipping(0, 10, 20, 100, 10));
			lA = 300;

			Part b = new Part();//part b stretches over two pages
			b.Add(new Clipping(0, 10, 30, 10, 10));
			b.Add(new Clipping(1, 11, 10, 10, 10));
			b.Add(new Clipping(1, 11, 30, 10, 10));
			b.Add(new Clipping(2, 12, 30, 10, 10));
			lB = 40;

			Part c = new Part();//part c consists of a single split box
			c.Add(new Clipping(2, 22, 30, 10, 10));
			lC = 10;

			threeParts = new List<Part>();
			threeParts.Add(a);
			threeParts.Add(b);
			threeParts.Add(c);

		}

		[SetUp]
		public void Setup()
		{
			this.mocks = new MockRepository();
			this.view = mocks.StrictMock<IPlayerView>();
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		public void RunAll()
		{
			this.Setup();
			this.EmptyPartsTest();
			this.TearDown();
			this.Setup();
			this.OnePartPatternTest();
			this.TearDown();
			this.Setup();
			this.NextPartTest();
			this.TearDown();
		}

		[Test]
		public void EmptyPartsTest()
		{
			mocks.ReplayAll();
			try {
				Navigator n = new Navigator(ABCCBBA, new List<Part>(), this.view);
				n.Start();
				Assert.Fail("Expected Exception");
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}

		[Test, Ignore]
		public void OnePartPatternTest()
		{			
			//view.AppendLine(null);
			LastCall.IgnoreArguments().Repeat.Times(3);
			view.GotoLine(0);
			mocks.ReplayAll();

			Navigator n = new Navigator("A", threeParts, this.view);
			n.Start();
		}

		[Test, Ignore]
		public void NextPartTest()
		{
			//view.AppendLine(null);
			LastCall.IgnoreArguments().Repeat.Times(3 + 4 + 2 * 1 + 2 * 4 + 3);

			
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


			//mocks.ReplayAll();
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
	}
}