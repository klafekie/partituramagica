#region usings

using System;
using System.Windows;
using NUnit.Framework;
using PartituraMagica.Domain;
using PartituraMagica.PageEditing;
using Rhino.Mocks;

#endregion

namespace PartituraMagica
{
	internal class EditingTests : TestUtils
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();
			CurrentPageIndex = 0;
			ActivePartitura = Mocks.DynamicMock<IActivePartitura>();
			Expect.Call(ActivePartitura.Partitura).Repeat.Any().Return(Partitura);
			Expect.Call(ActivePartitura.IsOpen).Repeat.Any().Return(true);
			Expect.Call(ActivePartitura.CurrentPage).Repeat.Any().Do(
				(Func<Page>) delegate { return Partitura.GetPage(CurrentPageIndex); });
		}

		[TearDown]
		public virtual void TearDown()
		{
			Mocks.VerifyAll();
		}

		protected int CurrentPageIndex;


		public void RunAll()
		{
			Setup();
			TestAddRemoveBoxes();
			Setup();
			TestUpdateRightMargin();
			Setup();
			TestUpdateMultipleBoxes();
			Setup();
			TestSplitBox();
			Setup();
			TestUpdateSplittedBox();
			Setup();
			TestAddRemoveManyBoxes();
			Setup();
			TestMoveSplitters();
			Setup();
			TestLabel();
			TearDown();
		}

		//@todo p3 currently there is no reverse op for Splitting a Box

		private void LabelAt(double x, double y)
		{
			new SelectItemCommand(new Point(x, y), this).Execute();
			new LabelPartCommand(this).Execute(); // This test fails on this.Active.IsOpen() being false [jed 20141227] 
		}

		private void UnLabelAt(double x, double y)
		{
			new SelectItemCommand(new Point(x, y), this).Execute();
			new UnLabelPartCommand(this).Execute();
		}

		[Test]
		public void TestAddRemoveBoxes()
		{
			Mocks.ReplayAll();
			AssertShapeAt(50, 50, Shape.NothingSpecial);
			//A new rect of size 10/10 is created with current pos
			//as lower right corner
			//we assume a tolerance of 4.5 dots arround the border.
			new AddBoxCommand(new Point(50, 50), this).Execute();
			AssertShapeAt(35, 35, Shape.NothingSpecial);
			AssertShapeAt(35.5, 35.5, Shape.BoxLeftTopCorner);
			AssertShapeAt(44, 44, Shape.BoxLeftTopCorner);
			AssertShapeAt(45, 45, Shape.BoxContent);
			AssertShapeAt(46, 46, Shape.BoxRightBottomCorner);
			AssertShapeAt(54.4, 54.4, Shape.BoxRightBottomCorner);
			AssertShapeAt(55, 55, Shape.NothingSpecial);
			Assert.True(panel.IsSelected(new Rect(40, 40, 10, 10)));

			//adding a box in the same "row" is not allowed
			Assert.False(new AddBoxCommand(new Point(25, 45), this).CanExecute());
			Assert.False(new AddBoxCommand(new Point(75, 55), this).CanExecute());

			//last added box is always the selected box
			new RemoveBoxCommand(this).Execute();
			AssertShapeAt(45, 45, Shape.NothingSpecial);
			Assert.AreEqual(panel.Selection.Count, 0); //selection is cleared
		}


		[Test]
		public void TestAddRemoveManyBoxes()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 100), this).Execute();
			new AddBoxCommand(new Point(100, 120), this).Execute();
			new AddBoxCommand(new Point(100, 140), this).Execute();
			new AddBoxCommand(new Point(100, 160), this).Execute();
			new AddBoxCommand(new Point(100, 180), this).Execute();

			//delete the second and fourth box
			new SelectItemCommand(new Point(100, 120), this).Execute();
			new RemoveBoxCommand(this).Execute();

			new SelectItemCommand(new Point(100, 160), this).Execute();
			new RemoveBoxCommand(this).Execute();

			//Add new box where the second box was
			new AddBoxCommand(new Point(100, 120), this).Execute();

			Assert.False(new AddBoxCommand(new Point(100, 100), this).CanExecute());
			Assert.False(new AddBoxCommand(new Point(100, 120), this).CanExecute());
			Assert.False(new AddBoxCommand(new Point(100, 140), this).CanExecute());
			Assert.True(new AddBoxCommand(new Point(100, 160), this).CanExecute());
			Assert.False(new AddBoxCommand(new Point(100, 180), this).CanExecute());
		}

		[Test]
		public void TestLabel()
		{
			Mocks.ReplayAll();
			//three boxes, first has one splitter, second has no splitter, third has two splitters
			new AddBoxCommand(new Point(100, 100), this).Execute();
			Update(Shape.BoxRightMargin, 100, 95, 140, 95);
			new SplitBoxCommand(new Point(110, 95), this).Execute();
			new AddBoxCommand(new Point(100, 120), this).Execute();
			new AddBoxCommand(new Point(100, 140), this).Execute();
			Update(Shape.BoxRightMargin, 100, 135, 140, 135);
			new SplitBoxCommand(new Point(110, 135), this).Execute();
			new SplitBoxCommand(new Point(130, 135), this).Execute();

			AssertPanelContains(new Rect(90, 90, 20, 10), "A"); //implicit labelled
			//the implicit 'A' cannot be altered
			new SelectItemCommand(new Point(90, 90), this).Execute();
			Assert.False(new LabelPartCommand(this).CanExecute());
			Assert.False(new UnLabelPartCommand(this).CanExecute());

			//label second part of box1
			LabelAt(120, 95);
			//label second box
			//LabelAt(100, 115);
			//label first and last part of third box
			LabelAt(102, 135);
			LabelAt(136, 135);

			AssertPanelContains(new Rect(90, 90, 20, 10), "A"); //first box on first page cannot be unlabelled
			AssertPanelContains(new Rect(110, 90, 30, 10), "B");
			AssertPanelContains(new Rect(90, 110, 10, 10), null); //no label
			AssertPanelContains(new Rect(90, 130, 20, 10), "C");
			AssertPanelContains(new Rect(110, 130, 20, 10), null);
			AssertPanelContains(new Rect(130, 130, 10, 10), "D");

			//remove first box, second box must be labelled A			
			new SelectItemCommand(new Point(90, 90), this).Execute();
			new RemoveBoxCommand(this).Execute();
			AssertPanelContains(new Rect(90, 110, 10, 10), "A");
			AssertPanelContains(new Rect(90, 130, 20, 10), "B");
			AssertPanelContains(new Rect(110, 130, 20, 10), null);
			AssertPanelContains(new Rect(130, 130, 10, 10), "C");

			//remove second box
			new SelectItemCommand(new Point(90, 110), this).Execute();
			new RemoveBoxCommand(this).Execute();
			Assert.AreEqual(3, panel.Rectangles.Count);
			AssertPanelContains(new Rect(90, 130, 20, 10), "A");
			AssertPanelContains(new Rect(110, 130, 20, 10), null);
			AssertPanelContains(new Rect(130, 130, 10, 10), "B");
		}

		[Test]
		public void TestMoveLeftTop()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 200), this).Execute();

			//try to move over left/top border
			Update(Shape.BoxContent, 95, 195, -10, -10);
			Assert.AreEqual(panel.LastRect, new Rect(0, 0, 10, 10));
		}

		[Test]
		public void TestMoveRightBottom()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 200), this).Execute();

			//try to move over right/bottom border
			Update(Shape.BoxContent, 95, 195, 1005, 2005);
			Assert.AreEqual(panel.LastRect, new Rect(990, 1990, 10, 10));
		}


		[Test]
		public void TestMoveSplitters()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 200), this).Execute();

			//enlarge box to be 50 pixels wide.
			new UpdateBoxCommand(new Point(100, 200), new Point(140, 200), this).Execute();
			CollectionAssert.Contains(panel.Rectangles.Values, new Rect(90, 190, 50, 10));

			new SplitBoxCommand(new Point(114, 200), this).Execute();
			new SplitBoxCommand(new Point(125, 200), this).Execute();
			AssertPanelContains(new Rect(90, 190, 24, 10));
			AssertPanelContains(new Rect(114, 190, 11, 10));
			AssertPanelContains(new Rect(125, 190, 15, 10));

			//move first splitter to leftmost position (100)
			Update(Shape.BoxSplitter, 110, 195, 110 - 14, 666);
			AssertPanelContains(new Rect(90, 190, 10, 10));
			AssertPanelContains(new Rect(100, 190, 25, 10));
			AssertPanelContains(new Rect(125, 190, 15, 10));

			//try to move over left box border
			Assert.False(CanUpdate(Shape.BoxSplitter, 96, 195, 95, 195));
			Assert.False(CanUpdate(Shape.BoxSplitter, 96, 195, 80, 195)); //outside the box

			//try move over the second splitter
			Assert.True(CanUpdate(Shape.BoxSplitter, 96, 195, 96 + 15, 195));
			Assert.False(CanUpdate(Shape.BoxSplitter, 96, 195, 96 + 16, 195));
			Assert.False(CanUpdate(Shape.BoxSplitter, 96, 195, 145, 195)); //outside the box

			//try move rightmost splitter over the right border
			Assert.True(CanUpdate(Shape.BoxSplitter, 126, 195, 126 + 5, 195));
			Assert.False(CanUpdate(Shape.BoxSplitter, 126, 195, 126 + 6, 195));
			Assert.False(CanUpdate(Shape.BoxSplitter, 126, 195, 145, 195)); //outside the box
		}

		[Test]
		public void TestResizeLeftTop()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 200), this).Execute();

			//try to resize over left/top border -> results in 0, 0
			Update(Shape.BoxLeftTopCorner, 90, 190, -1, -1);
			Assert.AreEqual(panel.LastRect, new Rect(0, 0, 100, 200));
		}

		[Test]
		public void TestResizeMove()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 200), this).Execute();
			//since we have only a single box, update can be executed 
			// everywhere within page borders
			Assert.True(CanUpdate(90, 190, 0, 0));
			Assert.True(CanUpdate(95, 195, 990, 1950));

			Assert.False(CanUpdate(Shape.BoxLeftTopCorner, 94, 194, 95, 194),
				"cannot reduce height more, cause we're already at the minimum");

			Assert.False(CanUpdate(Shape.BoxLeftTopCorner, 94, 194, 94, 195),
				"Cannot reduce width, cause we're already at the min");

			//enlarge by 2,3
			Update(Shape.BoxLeftTopCorner, 94, 194, 92, 191);
			Assert.AreEqual(panel.LastRect, new Rect(88, 187, 12, 13));

			//move
			Update(Shape.BoxContent, 95, 195, 115, 205);
			Assert.AreEqual(panel.LastRect, new Rect(108, 197, 12, 13));
		}

		[Test]
		public void TestResizeRightBottom()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 200), this).Execute();

			//try to resize over the right/bottom corner
			Update(Shape.BoxRightBottomCorner, 100, 200, 1005, 2005);
			Assert.AreEqual(panel.LastRect, new Rect(90, 190, 1000 - 90, 2000 - 190));
		}

		[Test]
		public void TestSplitBox()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 200), this).Execute();

			//enlarge second box to be 50 pixels wide (90,200,50,10)
			new UpdateBoxCommand(new Point(100, 200), new Point(140, 200), this).Execute();
			AssertPanelContains(new Rect(90, 190, 50, 10));

			new SplitBoxCommand(new Point(125, 200), this).Execute();
			AssertPanelContains(new Rect(90, 190, 35, 10));
			AssertPanelContains(new Rect(125, 190, 15, 10));

			//Split with point outside the second box, results in "middle" (115)
			new SplitBoxCommand(new Point(100, 100), this).Execute();
			AssertPanelContains(new Rect(90, 190, 25, 10));
			AssertPanelContains(new Rect(115, 190, 10, 10));
			AssertPanelContains(new Rect(125, 190, 15, 10));
			Assert.AreEqual(3, panel.Rectangles.Count);

			//splitting again at same location has no effect
			new SplitBoxCommand(new Point(100, 100), this).Execute();
			AssertPanelContains(new Rect(90, 190, 25, 10));
			AssertPanelContains(new Rect(115, 190, 10, 10));
			AssertPanelContains(new Rect(125, 190, 15, 10));
			Assert.AreEqual(3, panel.Rectangles.Count);

			//nothing happens, cause there is already a splitter at 115
			new SplitBoxCommand(new Point(120, 200), this).Execute();
			AssertPanelContains(new Rect(90, 190, 25, 10));
			AssertPanelContains(new Rect(115, 190, 10, 10));
			AssertPanelContains(new Rect(125, 190, 15, 10));
			Assert.AreEqual(3, panel.Rectangles.Count);
		}

		[Test]
		public void TestUpdateMultipleBoxes()
		{
			Mocks.ReplayAll();
			//overlapping on X axis assuming initial size of 10x10
			new AddBoxCommand(new Point(105, 50), this).Execute();
			new AddBoxCommand(new Point(100, 100), this).Execute();
			//resize lower box into or over upper box
			Assert.False(CanUpdate(95, 90, 95, 45));
			Assert.False(CanUpdate(95, 90, 95, 5));
			//move lower box into or over upper box
			Assert.False(CanUpdate(95, 95, 95, 45));
			//moving over the upper box in a single step would work,
			//but this will never happen when done with the mouse...
			Assert.True(CanUpdate(95, 95, 95, 5));
		}

		[Test]
		public void TestUpdateRightMargin()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 100), this).Execute();
			Assert.AreEqual(panel.LastRect, new Rect(90, 90, 10, 10));

			Assert.False(CanUpdate(Shape.BoxRightMargin, 104, 95, 103, 95),
				"cannot reduce width more, cause we're already at the minimum");

			//enlarge
			Update(Shape.BoxRightMargin, 104, 95, 105, 9999);
			Assert.AreEqual(panel.LastRect, new Rect(90, 90, 11, 10));

			//move
			Update(Shape.BoxContent, 95, 95, 10, 20);
			Assert.AreEqual(panel.LastRect, new Rect(90 - 85, 90 - 75, 11, 10));
		}

		[Test]
		public void TestUpdateSplittedBox()
		{
			Mocks.ReplayAll();
			new AddBoxCommand(new Point(100, 200), this).Execute();

			//enlarge box to be 50 pixels wide.
			new UpdateBoxCommand(new Point(100, 200), new Point(140, 200), this).Execute();
			AssertPanelContains(new Rect(90, 190, 50, 10));

			new SplitBoxCommand(new Point(120, 200), this).Execute();
			AssertShapeAt(120, 195, Shape.BoxSplitter);
			AssertPanelContains(new Rect(90, 190, 30, 10));
			AssertPanelContains(new Rect(120, 190, 20, 10));

			//move the box by +10,+20
			Update(Shape.BoxContent, 100, 195, 110, 215);
			AssertPanelContains(new Rect(100, 210, 30, 10));
			AssertPanelContains(new Rect(130, 210, 20, 10));
			AssertShapeAt(130, 215, Shape.BoxSplitter);

			//resize the box by 20,30 tearing at the left top corner
			Update(Shape.BoxLeftTopCorner, 100, 210, 80, 180);
			//the splitter moves with the left margin
			AssertShapeAt(110, 215, Shape.BoxSplitter);
			AssertPanelContains(new Rect(80, 180, 30, 40));
			AssertPanelContains(new Rect(110, 180, 40, 40));

			//try resize the box to less than rightmost splitter			
			Assert.False(CanUpdate(Shape.BoxRightMargin,
				80 + 70, 210, 80 + 70 - 41, 210));

			//resize exactly to minimum
			Update(Shape.BoxRightBottomCorner,
				80 + 70, 220, 80 + 70 - 30, 190);
			AssertPanelContains(new Rect(80, 180, 30, 10));
			AssertPanelContains(new Rect(110, 180, 10, 10));
			AssertShapeAt(84, 185, Shape.BoxLeftMargin);
			AssertShapeAt(85, 185, Shape.BoxContent);
			AssertShapeAt(106, 185, Shape.BoxSplitter);
			AssertShapeAt(107, 185, Shape.BoxSplitter);
			AssertShapeAt(115, 185, Shape.BoxContent);
			AssertShapeAt(116, 185, Shape.BoxRightMargin);

			Assert.False(CanUpdate(Shape.BoxLeftMargin,
					80, 185, 81, 185),
				"we cannot reduce size anymore now, even from leftmargin");
		}
	}
}