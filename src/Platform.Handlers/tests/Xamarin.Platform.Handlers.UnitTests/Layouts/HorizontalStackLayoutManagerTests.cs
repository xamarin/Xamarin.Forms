﻿using System.Collections.Generic;
using NSubstitute;
using Xamarin.Forms;
using Xamarin.Platform.Layouts;
using Xunit;

namespace Xamarin.Platform.Handlers.UnitTests.Layouts
{
	[Category(TestCategory.Core, TestCategory.Layout)]
	public class HorizontalStackLayoutManagerTests : StackLayoutManagerTests
	{
		[Theory]
		[InlineData(0, 100, 0, 0)]
		[InlineData(1, 100, 0, 100)]
		[InlineData(1, 100, 13, 100)]
		[InlineData(2, 100, 13, 213)]
		[InlineData(3, 100, 13, 326)]
		[InlineData(3, 100, -13, 274)]
		public void SpacingMeasurement(int viewCount, double viewWidth, int spacing, double expectedWidth)
		{
			var stack = BuildStack(viewCount, viewWidth, 100);
			stack.Spacing.Returns(spacing);

			var manager = new HorizontalStackLayoutManager(stack);
			var measuredSize = manager.Measure(double.PositiveInfinity, 100);

			Assert.Equal(expectedWidth, measuredSize.Width);
		}

		[Theory("Spacing should not affect arrangement with only one item")]
		[InlineData(0), InlineData(26), InlineData(-54)]
		public void SpacingArrangementOneItem(int spacing)
		{
			var stack = BuildStack(1, 100, 100);
			stack.Spacing.Returns(spacing);

			var manager = new HorizontalStackLayoutManager(stack);

			var measuredSize = manager.Measure(double.PositiveInfinity, 100);
			manager.Arrange(new Rectangle(Point.Zero, measuredSize));

			var expectedRectangle = new Rectangle(0, 0, 100, 100);
			stack.Children[0].Received().Arrange(Arg.Is(expectedRectangle));
		}

		[Theory("Spacing should affect arrangement with more than one item")]
		[InlineData(26), InlineData(-54)]
		public void SpacingArrangementTwoItems(int spacing)
		{
			var stack = BuildStack(2, 100, 100);
			stack.Spacing.Returns(spacing);

			var manager = new HorizontalStackLayoutManager(stack);

			var measuredSize = manager.Measure(double.PositiveInfinity, 100);
			manager.Arrange(new Rectangle(Point.Zero, measuredSize));

			var expectedRectangle0 = new Rectangle(0, 0, 100, 100);
			stack.Children[0].Received().Arrange(Arg.Is(expectedRectangle0));

			var expectedRectangle1 = new Rectangle(100 + spacing, 0, 100, 100);
			stack.Children[1].Received().Arrange(Arg.Is(expectedRectangle1));
		}

		[Theory]
		[InlineData(150, 100, 100)]
		[InlineData(150, 200, 200)]
		[InlineData(1250, -1, 1250)]
		public void StackAppliesWidth(double viewWidth, double stackWidth, double expectedWidth)
		{
			var stack = CreateTestLayout();

			var view = CreateTestView(new Size(viewWidth, 100));

			var children = new List<IView>() { view }.AsReadOnly();

			stack.Children.Returns(children);
			stack.Width.Returns(stackWidth);

			var manager = new HorizontalStackLayoutManager(stack);
			var measurement = manager.Measure(double.PositiveInfinity, 100);
			Assert.Equal(expectedWidth, measurement.Width);
		}

		[Fact]
		public void ViewsArrangedWithDesiredHeights()
		{
			var stack = CreateTestLayout();
			var manager = new HorizontalStackLayoutManager(stack);

			var view1 = CreateTestView(new Size(100, 200));
			var view2 = CreateTestView(new Size(100, 150));

			var children = new List<IView>() { view1, view2 }.AsReadOnly();
			stack.Children.Returns(children);

			var measurement = manager.Measure(double.PositiveInfinity, double.PositiveInfinity);
			manager.Arrange(new Rectangle(Point.Zero, measurement));

			// The tallest IView is 200, so the stack should be that tall
			Assert.Equal(200, measurement.Height);

			// We expect the first IView to be at 0,0 with a width of 100 and a height of 200
			var expectedRectangle1 = new Rectangle(0, 0, 100, 200);
			view1.Received().Arrange(Arg.Is(expectedRectangle1));

			// We expect the second IView to be at 100, 0 with a width of 100 and a height of 150
			var expectedRectangle2 = new Rectangle(100, 0, 100, 150);
			view2.Received().Arrange(Arg.Is(expectedRectangle2));
		}
	}
}
