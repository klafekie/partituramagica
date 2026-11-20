#region usings

using Moq;
using NUnit.Framework;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.PageEditing;

#endregion

namespace PartituraMagica.Test
{
	internal class EditingTests : TestUtils
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();
			CurrentPageIndex = 0;
			activePartitura = new Mock<IEditedPartitura>();

			activePartitura.Setup(p => p.Partitura).Returns(Partitura);
			activePartitura.Setup(p => p.IsOpen).Returns(true);
			activePartitura.Setup(p => p.CurrentPage).Returns(() => Partitura.GetPage(CurrentPageIndex));
		}

		[TearDown]
		public virtual void TearDown()
		{
			//TODO p2 verify mocks? use mockrepository?
		}

		protected int CurrentPageIndex;

		//@todo p3 currently there is no reverse op for Splitting a Box

		private void LabelAt(double x, double y)
		{
			CreateSelectItemCommand(new Point(x, y)).Execute();
			new LabelPartCommand(this).Execute(); // This test fails on this.Active.IsOpen() being false [jed 20141227] 
		}

		private void UnLabelAt(double x, double y)
		{
			CreateSelectItemCommand(new Point(x, y)).Execute();
			new UnLabelPartCommand(this).Execute();
		}

		[Test]
		public void TestAddRemoveBoxes()
		{
			AssertShapeAt(50, 50, Shape.NothingSpecial);
			//A new rect of size BoxW/BoxH is created with current pos
			//as lower right corner
			//we assume a tolerance of 4.5 dots arround the border.
			//The new box is the minimum box: it has no BoxContent area, only the centerY band.
			CreateAddBoxCommand(new Point(50, 50)).Execute();
			AssertShapeAt(35, 35, Shape.NothingSpecial);
			AssertShapeAt(35.5, 35.5, Shape.BoxLeftTopCorner);
			AssertShapeAt(44, 39, Shape.BoxLeftTopCorner);
			AssertShapeAt(45, 39, Shape.BoxTopMargin);
			AssertShapeAt(45, 45, Shape.BoxCenterY);
			AssertShapeAt(46, 46, Shape.BoxRightBottomCorner);
			AssertShapeAt(54.4, 54.4, Shape.BoxRightBottomCorner);
			AssertShapeAt(55, 55, Shape.NothingSpecial);
			Assert.True(panel.IsSelected(new Rect(50 - BoxW, 50 - BoxH, BoxW, BoxH)));

			//adding a box in the same "row" is not allowed
			Assert.False(CreateAddBoxCommand(new Point(25, 45)).CanExecute());
			Assert.False(CreateAddBoxCommand(new Point(75, 55)).CanExecute());

			//last added box is always the selected box
			new RemoveBoxCommand(this).Execute();
			AssertShapeAt(45, 45, Shape.NothingSpecial);
			Assert.AreEqual(panel.Selection.Count, 0); //selection is cleared
		}


		[Test]
		public void TestAddRemoveManyBoxes()
		{
			CreateAddBoxCommand(new Point(100, 100)).Execute();
			CreateAddBoxCommand(new Point(100, 120)).Execute();
			CreateAddBoxCommand(new Point(100, 140)).Execute();
			CreateAddBoxCommand(new Point(100, 160)).Execute();
			CreateAddBoxCommand(new Point(100, 180)).Execute();

			//delete the second and fourth box
			CreateSelectItemCommand(new Point(100, 120)).Execute();
			new RemoveBoxCommand(this).Execute();

			CreateSelectItemCommand(new Point(100, 160)).Execute();
			new RemoveBoxCommand(this).Execute();

			//Add new box where the second box was
			CreateAddBoxCommand(new Point(100, 120)).Execute();

			Assert.False(CreateAddBoxCommand(new Point(100, 100)).CanExecute());
			Assert.False(CreateAddBoxCommand(new Point(100, 120)).CanExecute());
			Assert.False(CreateAddBoxCommand(new Point(100, 140)).CanExecute());
			Assert.True(CreateAddBoxCommand(new Point(100, 160)).CanExecute());
			Assert.False(CreateAddBoxCommand(new Point(100, 180)).CanExecute());
		}

		[Test]
		public void TestLabel()
		{
			//three boxes, first has one splitter, second has no splitter, third has two splitters
			CreateAddBoxCommand(new Point(100, 100)).Execute();
			Update(Shape.BoxRightMargin, 100, 95, 140, 95);
			CreateSplitBoxCommand(new Point(110, 95)).Execute();
			CreateAddBoxCommand(new Point(100, 120)).Execute();
			CreateAddBoxCommand(new Point(100, 140)).Execute();
			Update(Shape.BoxRightMargin, 100, 135, 140, 135);
			CreateSplitBoxCommand(new Point(110, 135)).Execute();
			CreateSplitBoxCommand(new Point(130, 135)).Execute();

			AssertPanelContains(new Rect(90, 100 - BoxH, 20, BoxH), "A"); //implicit labelled
			//the implicit 'A' cannot be altered
			CreateSelectItemCommand(new Point(90, 90)).Execute();
			Assert.False(new LabelPartCommand(this).CanExecute());
			Assert.False(new UnLabelPartCommand(this).CanExecute());

			// splits auto-label their right-hand side: no manual labelling needed

			AssertPanelContains(new Rect(90, 100 - BoxH, 20, BoxH), "A"); //first box on first page cannot be unlabelled
			AssertPanelContains(new Rect(110, 100 - BoxH, 30, BoxH), "B"); //auto-labelled by split
			AssertPanelContains(new Rect(90, 120 - BoxH, BoxW, BoxH), null); //no label
			AssertPanelContains(new Rect(90, 140 - BoxH, 20, BoxH), null); //left of box3, not auto-labelled
			AssertPanelContains(new Rect(110, 140 - BoxH, 20, BoxH), "C"); //auto-labelled by first split
			AssertPanelContains(new Rect(130, 140 - BoxH, BoxW, BoxH), "D"); //auto-labelled by second split

			//remove first box, second box must be labelled A
			CreateSelectItemCommand(new Point(90, 90)).Execute();
			new RemoveBoxCommand(this).Execute();
			AssertPanelContains(new Rect(90, 120 - BoxH, BoxW, BoxH), "A");
			AssertPanelContains(new Rect(90, 140 - BoxH, 20, BoxH), null); //box3 left still unlabelled
			AssertPanelContains(new Rect(110, 140 - BoxH, 20, BoxH), "B");
			AssertPanelContains(new Rect(130, 140 - BoxH, BoxW, BoxH), "C");

			//remove second box
			CreateSelectItemCommand(new Point(90, 110)).Execute();
			new RemoveBoxCommand(this).Execute();
			Assert.AreEqual(3, panel.Rectangles.Count);
			AssertPanelContains(new Rect(90, 140 - BoxH, 20, BoxH), "A"); //implicit, box3 left is first
			AssertPanelContains(new Rect(110, 140 - BoxH, 20, BoxH), "B");
			AssertPanelContains(new Rect(130, 140 - BoxH, BoxW, BoxH), "C");
		}

		[Test]
		public void TestMoveLeftTop()
		{
			CreateAddBoxCommand(new Point(100, 200)).Execute();

			//try to move over left/top border
			//(a box of minimum height has no BoxContent area, so we cannot hit test it)
			Update(Shape.BoxContent, 95, 195, -10, -10, checkShape: false);
			Assert.AreEqual(new Rect(0, 0, BoxW, BoxH), panel.LastRect);
		}

		[Test]
		public void TestMoveRightBottom()
		{
			CreateAddBoxCommand(new Point(100, 200)).Execute();

			//try to move over right/bottom border
			Update(Shape.BoxContent, 95, 195, 1005, 2005, checkShape: false);
			Assert.AreEqual(new Rect(1000 - BoxW, 2000 - BoxH, BoxW, BoxH), panel.LastRect);
		}


		[Test]
		public void TestMoveSplitters()
		{
			CreateAddBoxCommand(new Point(100, 200)).Execute();

			//enlarge box to be 50 pixels wide and Extra pixels higher,
			//splitters can only be hit in the BoxContent area, which a minimum box doesn't have.
			const double extra = 30;
			double top = 200 - BoxH;
			double height = BoxH + extra;
			Update(Shape.BoxRightBottomCorner, 100, 200, 140, 200 + extra);
			CollectionAssert.Contains(panel.Rectangles.Values, new Rect(90, top, 50, height));

			CreateSplitBoxCommand(new Point(114, 200)).Execute();
			CreateSplitBoxCommand(new Point(125, 200)).Execute();
			AssertPanelContains(new Rect(90, top, 24, height));
			AssertPanelContains(new Rect(114, top, 11, height));
			AssertPanelContains(new Rect(125, top, 15, height));

			//move first splitter to leftmost position (100)
			Update(Shape.BoxSplitter, 110, 195, 110 - 14, 666);
			AssertPanelContains(new Rect(90, top, 10, height));
			AssertPanelContains(new Rect(100, top, 25, height));
			AssertPanelContains(new Rect(125, top, 15, height));

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
			CreateAddBoxCommand(new Point(100, 200)).Execute();

			//try to resize over left/top border -> results in 0, 0
			Update(Shape.BoxLeftTopCorner, 90, 190, -1, -1);
			//the centerY line stays where it was (absolute position), it is not centered again
			var expected = new Rect(new Rect(0, 0, 100, 200), centerY: 200 - BoxH + BoxH / 2D);
			Assert.AreEqual(expected, panel.LastRect);
		}

		[Test]
		public void TestResizeMove()
		{
			CreateAddBoxCommand(new Point(100, 200)).Execute();
			//since we have only a single box, update can be executed 
			// everywhere within page borders
			Assert.True(CanUpdate(Shape.BoxLeftTopCorner, 90, 190, 0, 0));
			Assert.True(CanUpdate(Shape.BoxContent, 95, 195, 990, 1950, checkShape: false));

			Assert.False(CanUpdate(Shape.BoxLeftTopCorner, 94, 189, 95, 189),
				"cannot reduce width, cause we're already at the min");

			Assert.False(CanUpdate(Shape.BoxLeftTopCorner, 94, 189, 94, 190),
				"cannot reduce height more, cause we're already at the minimum");

			//enlarge by 2,3
			Update(Shape.BoxLeftTopCorner, 94, 189, 92, 186);
			//the centerY line is pushed down by the top border, keeping its absolute position
			var enlarged = new Rect(new Rect(90 - 2, 200 - BoxH - 3, BoxW + 2, BoxH + 3), centerY: BoxH / 2D + 3);
			Assert.AreEqual(enlarged, panel.LastRect);

			//move
			Update(Shape.BoxContent, 95, 195, 115, 205, checkShape: false);
			Assert.AreEqual(new Rect(enlarged, enlarged.X + 20, enlarged.Y + 10, enlarged.Width, enlarged.Height),
				panel.LastRect);
		}

		[Test]
		public void TestResizeRightBottom()
		{
			CreateAddBoxCommand(new Point(100, 200)).Execute();

			//try to resize over the right/bottom corner
			Update(Shape.BoxRightBottomCorner, 100, 200, 1005, 2005);
			//the centerY line is centered again
			Assert.AreEqual(new Rect(90, 200 - BoxH, 1000 - 90, 2000 - (200 - BoxH)), panel.LastRect);
		}

		[Test]
		public void TestSplitBox()
		{
			CreateAddBoxCommand(new Point(100, 200)).Execute();
			double top = 200 - BoxH;

			//enlarge second box to be 50 pixels wide (90, top, 50, BoxH)
			Update(Shape.BoxRightBottomCorner, 100, 200, 140, 200);
			AssertPanelContains(new Rect(90, top, 50, BoxH));

			CreateSplitBoxCommand(new Point(125, 200)).Execute();
			AssertPanelContains(new Rect(90, top, 35, BoxH));
			AssertPanelContains(new Rect(125, top, 15, BoxH));

			//Split with point outside the second box, results in "middle" (115)
			CreateSplitBoxCommand(new Point(100, 100)).Execute();
			AssertPanelContains(new Rect(90, top, 25, BoxH));
			AssertPanelContains(new Rect(115, top, 10, BoxH));
			AssertPanelContains(new Rect(125, top, 15, BoxH));
			Assert.AreEqual(3, panel.Rectangles.Count);

			//splitting again at same location has no effect
			CreateSplitBoxCommand(new Point(100, 100)).Execute();
			AssertPanelContains(new Rect(90, top, 25, BoxH));
			AssertPanelContains(new Rect(115, top, 10, BoxH));
			AssertPanelContains(new Rect(125, top, 15, BoxH));
			Assert.AreEqual(3, panel.Rectangles.Count);

			//nothing happens, cause there is already a splitter at 115
			CreateSplitBoxCommand(new Point(120, 200)).Execute();
			AssertPanelContains(new Rect(90, top, 25, BoxH));
			AssertPanelContains(new Rect(115, top, 10, BoxH));
			AssertPanelContains(new Rect(125, top, 15, BoxH));
			Assert.AreEqual(3, panel.Rectangles.Count);
		}

		[Test]
		public void TestUpdateMultipleBoxes()
		{
			//overlapping on X axis
			CreateAddBoxCommand(new Point(105, 50)).Execute();
			CreateAddBoxCommand(new Point(100, 100)).Execute();
			//resize lower box into or over upper box
			Assert.False(CanUpdate(Shape.BoxTopMargin, 95, 90, 95, 45));
			Assert.False(CanUpdate(Shape.BoxTopMargin, 95, 90, 95, 5));
			//move lower box into or over upper box
			//(a box of minimum height has no BoxContent area, so we cannot hit test it)
			Assert.False(CanUpdate(Shape.BoxContent, 95, 95, 95, 45, checkShape: false));
			//moving over the upper box in a single step would work,
			//but this will never happen when done with the mouse...
			Assert.True(CanUpdate(Shape.BoxContent, 95, 95, 95, 5, checkShape: false));
		}

		[Test]
		public void TestUpdateRightMargin()
		{
			CreateAddBoxCommand(new Point(100, 100)).Execute();
			Assert.AreEqual(new Rect(90, 100 - BoxH, BoxW, BoxH), panel.LastRect);

			Assert.False(CanUpdate(Shape.BoxRightMargin, 104, 95, 103, 95),
				"cannot reduce width more, cause we're already at the minimum");

			//enlarge
			Update(Shape.BoxRightMargin, 104, 95, 105, 9999);
			Assert.AreEqual(new Rect(90, 100 - BoxH, BoxW + 1, BoxH), panel.LastRect);

			//move
			Update(Shape.BoxContent, 95, 95, 10, 20, checkShape: false);
			Assert.AreEqual(new Rect(90 - 85, 100 - BoxH - 75, BoxW + 1, BoxH), panel.LastRect);
		}

		[Test]
		public void TestUpdateSplittedBox()
		{
			CreateAddBoxCommand(new Point(100, 200)).Execute();

			//enlarge box to be 50 pixels wide and 30 pixels higher,
			//splitters can only be hit in the BoxContent area, which a minimum box doesn't have.
			const double extra = 30;
			double height = BoxH + extra;
			Update(Shape.BoxRightBottomCorner, 100, 200, 140, 200 + extra);
			AssertPanelContains(new Rect(90, 200 - BoxH, 50, height));

			CreateSplitBoxCommand(new Point(120, 200)).Execute();
			AssertShapeAt(120, 195, Shape.BoxSplitter);
			AssertPanelContains(new Rect(90, 200 - BoxH, 30, height));
			AssertPanelContains(new Rect(120, 200 - BoxH, 20, height));

			//move the box by +10,+20
			Update(Shape.BoxContent, 100, 195, 110, 215);
			AssertPanelContains(new Rect(100, 220 - BoxH, 30, height));
			AssertPanelContains(new Rect(130, 220 - BoxH, 20, height));
			AssertShapeAt(130, 215, Shape.BoxSplitter);

			//resize the box by 20,30 tearing at the left top corner
			Update(Shape.BoxLeftTopCorner, 100, 210, 80, 180);
			//the splitter moves with the left margin
			AssertShapeAt(110, 215, Shape.BoxSplitter);
			//the centerY line keeps its absolute position, so it is not centered anymore
			double top = 220 - BoxH - 30;
			double resizedHeight = height + 30;
			double centerY = height / 2D + 30;
			AssertPanelContains(new Rect(new Rect(80, top, 30, resizedHeight), centerY: centerY));
			AssertPanelContains(new Rect(new Rect(110, top, 40, resizedHeight), centerY: centerY));

			//try resize the box to less than rightmost splitter			
			Assert.False(CanUpdate(Shape.BoxRightMargin,
				80 + 70, 210, 80 + 70 - 41, 210));

			//resize the width exactly to minimum. Tearing at the corner centers the centerY line again.
			Update(Shape.BoxRightBottomCorner,
				80 + 70, top + resizedHeight, 80 + 70 - 30, top + resizedHeight);
			AssertPanelContains(new Rect(80, top, 30, resizedHeight));
			AssertPanelContains(new Rect(110, top, 10, resizedHeight));
			AssertShapeAt(84, 230, Shape.BoxLeftMargin);
			AssertShapeAt(85, 230, Shape.BoxContent);
			AssertShapeAt(106, 230, Shape.BoxSplitter);
			AssertShapeAt(107, 230, Shape.BoxSplitter);
			AssertShapeAt(115, 230, Shape.BoxContent);
			AssertShapeAt(116, 230, Shape.BoxRightMargin);

			Assert.False(CanUpdate(Shape.BoxLeftMargin,
					80, 230, 81, 230),
				"we cannot reduce size anymore now, even from leftmargin");
		}
	}
}