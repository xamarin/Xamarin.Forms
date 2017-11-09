using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.FlexLayoutTests
{
	[TestFixture]
	public class FlexLayoutFlexTests : FlexLayoutBaseTestFixture
	{
		[Test]
		public void TestFlexBasisFlexGrowColumn()
		{
			var platform = new UnitPlatform((view, width, height) => { return new SizeRequest(new Size(0, 0)); });
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetGrow(view1, 1);
			FlexLayout.SetBasis(view1, 50);

			var view2 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetGrow(view2, 1);

			layout.Children.Add(view1);
			layout.Children.Add(view2);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(0, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(100, view1.Width);
			Assert.AreEqual(75, view1.Height);

			Assert.AreEqual(0, view2.X);
			Assert.AreEqual(75, view2.Y);
			Assert.AreEqual(100, view2.Width);
			Assert.AreEqual(25, view2.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(0, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(100, view1.Width);
			//Assert.AreEqual(75, view1.Height);

			//Assert.AreEqual(0, view2.X);
			//Assert.AreEqual(75, view2.Y);
			//Assert.AreEqual(100, view2.Width);
			//Assert.AreEqual(25, view2.Height);
		}

		[Test]
		public void TestFlexBasisflexGrowRow()
		{

			var platform = new UnitPlatform((view, width, height) => { return new SizeRequest(new Size(0, 0)); });
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Row;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetGrow(view1, 1);
			FlexLayout.SetBasis(view1, 50);

			var view2 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetGrow(view2, 1);

			layout.Children.Add(view1);
			layout.Children.Add(view2);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(0, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(75, view1.Width);
			Assert.AreEqual(100, view1.Height);

			Assert.AreEqual(75, view2.X);
			Assert.AreEqual(0, view2.Y);
			Assert.AreEqual(25, view2.Width);
			Assert.AreEqual(100, view2.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(25, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(75, view1.Width);
			//Assert.AreEqual(100, view1.Height);

			//Assert.AreEqual(0, view2.X);
			//Assert.AreEqual(0, view2.Y);
			//Assert.AreEqual(25, view2.Width);
			//Assert.AreEqual(100, view2.Height);

		}


		[Test]
		public void TestFlexBasisFlexShrinkColumn()
		{
			var platform = new UnitPlatform((view, width, height) => { return new SizeRequest(new Size(0, 0)); });
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetShrink(view1, 1);
			FlexLayout.SetBasis(view1, 100);

			var view2 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetBasis(view2, 50);

			layout.Children.Add(view1);
			layout.Children.Add(view2);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(0, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(100, view1.Width);
			Assert.AreEqual(50, view1.Height);

			//Assert.AreEqual(0, view2.X);
			//Assert.AreEqual(50, view2.Y);
			//Assert.AreEqual(100, view2.Width);
			//Assert.AreEqual(50, view2.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(0, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(100, view1.Width);
			//Assert.AreEqual(50, view1.Height);

			//Assert.AreEqual(0, view2.X);
			//Assert.AreEqual(50, view2.Y);
			//Assert.AreEqual(100, view2.Width);
			//Assert.AreEqual(50, view2.Height);
		}

		[Test]
		public void TestFlexBasisFlexShrinkRow()
		{
			var platform = new UnitPlatform((view, width, height) => { return new SizeRequest(new Size(width, height)); });
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Row;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetShrink(view1, 1);
			FlexLayout.SetBasis(view1, 100);

			var view2 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetBasis(view2, 50);

			layout.Children.Add(view1);
			layout.Children.Add(view2);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(0, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(50, view1.Width);
			Assert.AreEqual(100, view1.Height);

			Assert.AreEqual(50, view2.X);
			Assert.AreEqual(0, view2.Y);
			Assert.AreEqual(50, view2.Width);
			Assert.AreEqual(100, view2.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(50, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(50, view1.Width);
			//Assert.AreEqual(100, view1.Height);

			//Assert.AreEqual(0, view2.X);
			//Assert.AreEqual(0, view2.Y);
			//Assert.AreEqual(50, view2.Width);
			//Assert.AreEqual(100, view2.Height);
		}

		[Test]
		public void TestFlexShrinkToZero()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true, Platform = platform };
			view1.WidthRequest = 50;
			view1.HeightRequest = 50;

			var view2 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetShrink(view2, 1);
			view2.WidthRequest = 50;
			view2.HeightRequest = 50;

			var view3 = new View { IsPlatformEnabled = true, Platform = platform };
			view3.WidthRequest = 50;
			view3.HeightRequest = 50;

			layout.Children.Add(view1);
			layout.Children.Add(view2);
			layout.Children.Add(view3);

			layout.Layout(new Rectangle(0, 0, 50, 75));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(50, layout.Width);
			Assert.AreEqual(75, layout.Height);

			Assert.AreEqual(0, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(50, view1.Width);
			Assert.AreEqual(50, view1.Height);

			Assert.AreEqual(0, view2.X);
			Assert.AreEqual(50, view2.Y);
			Assert.AreEqual(50, view2.Width);
			Assert.AreEqual(0, view2.Height);

			Assert.AreEqual(0, view3.X);
			Assert.AreEqual(50, view3.Y);
			Assert.AreEqual(50, view3.Width);
			Assert.AreEqual(50, view3.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0,  double.PositiveInfinity, 75));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(50, layout.Width);
			//Assert.AreEqual(75, layout.Height);

			//Assert.AreEqual(0, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(50, view1.Width);
			//Assert.AreEqual(50, view1.Height);

			//Assert.AreEqual(0, view2.X);
			//Assert.AreEqual(50, view2.Y);
			//Assert.AreEqual(50, view2.Width);
			//Assert.AreEqual(0, view2.Height);

			//Assert.AreEqual(0, view3.X);
			//Assert.AreEqual(50, view3.Y);
			//Assert.AreEqual(50, view3.Width);
			//Assert.AreEqual(50, view3.Height);
		}

		[Test]
		public void TestFlexBasisOverridesMainSize()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true, Platform = platform };
			view1.HeightRequest = 20;
			FlexLayout.SetGrow(view1, 1);
			FlexLayout.SetBasis(view1, 50);

			var view2 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetGrow(view2, 1);
			view2.HeightRequest = 10;

			var view3 = new View { IsPlatformEnabled = true, Platform = platform };
			FlexLayout.SetGrow(view3, 1);
			view3.HeightRequest = 10;

			layout.Children.Add(view1);
			layout.Children.Add(view2);
			layout.Children.Add(view3);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(0, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(100, view1.Width);
			Assert.AreEqual(60, view1.Height);

			Assert.AreEqual(0, view2.X);
			Assert.AreEqual(60, view2.Y);
			Assert.AreEqual(100, view2.Width);
			Assert.AreEqual(20, view2.Height);

			Assert.AreEqual(0, view3.X);
			Assert.AreEqual(80, view3.Y);
			Assert.AreEqual(100, view3.Width);
			Assert.AreEqual(20, view3.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0,  100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(0, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(100, view1.Width);
			//Assert.AreEqual(60, view1.Height);

			//Assert.AreEqual(0, view2.X);
			//Assert.AreEqual(60, view2.Y);
			//Assert.AreEqual(100, view2.Width);
			//Assert.AreEqual(20, view2.Height);

			//Assert.AreEqual(0, view3.X);
			//Assert.AreEqual(80, view3.Y);
			//Assert.AreEqual(100, view3.Width);
			//Assert.AreEqual(20, view3.Height);

		}



		[Test]
		public void TestFlexGrowShrinkAtMost()
		{
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.WidthRequest = 100;
			layout.HeightRequest = 100;

			var layout2 = new FlexLayout();
			layout2.FlexDirection = FlexDirection.Column;
			layout2.Platform = new UnitPlatform((visual, width, height) =>
			{
				return new SizeRequest(new Size(0, 0));
			});
			layout2.IsPlatformEnabled = true;
			layout.Children.Add(layout2);

			var view1 = new View
			{
				IsPlatformEnabled = true,
				Platform = new UnitPlatform((visual, width, height) =>
				{
					return new SizeRequest(new Size(0, 0));
				})
			};
			FlexLayout.SetGrow(view1, 1);
			FlexLayout.SetShrink(view1, 1);
			layout2.Children.Add(view1);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(0, layout2.X);
			Assert.AreEqual(0, layout2.Y);
			Assert.AreEqual(100, layout2.Width);
			Assert.AreEqual(0, layout2.Height);

			Assert.AreEqual(0, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(100, view1.Width);
			Assert.AreEqual(0, view1.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0,  100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(0, layout2.X);
			//Assert.AreEqual(0, layout2.Y);
			//Assert.AreEqual(100, layout2.Width);
			//Assert.AreEqual(0, layout2.Height);

			//Assert.AreEqual(0, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(100, view1.Width);
			//Assert.AreEqual(0, view1.Height);
		}
	}
}
