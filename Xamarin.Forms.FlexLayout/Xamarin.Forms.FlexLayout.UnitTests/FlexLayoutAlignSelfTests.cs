using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.FlexLayoutTests
{

	[TestFixture]
	public class FlexLayoutAlignSelfTest : FlexLayoutBaseTestFixture
	{

		[Test]
		public void TestAlignSelfCenter()
		{
			var platform = new UnitPlatform();
			var layout = new Xamarin.Forms.FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true };
			view1.HeightRequest = 10;
			view1.WidthRequest = 10;
			FlexLayout.SetAlignSelf(view1, Align.Center);

			layout.Children.Add(view1);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(45, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(10, view1.Width);
			Assert.AreEqual(10, view1.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(45, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(10, view1.Width);
			Assert.AreEqual(10, view1.Height);
		}

		[Test]
		public void TestAlignSelfFlexEnd()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true };
			view1.HeightRequest = 10;
			view1.WidthRequest = 10;
			FlexLayout.SetAlignSelf(view1, Align.FlexEnd);

			layout.Children.Add(view1);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(90, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(10, view1.Width);
			Assert.AreEqual(10, view1.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(0, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(10, view1.Width);
			//Assert.AreEqual(10, view1.Height);          
		}

		[Test]
		public void TestAlignSelfFlexStart()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true };
			view1.HeightRequest = 10;
			view1.WidthRequest = 10;
			FlexLayout.SetAlignSelf(view1, Align.FlexStart);

			layout.Children.Add(view1);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(0, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(10, view1.Width);
			Assert.AreEqual(10, view1.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(90, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(10, view1.Width);
			//Assert.AreEqual(10, view1.Height);   
		}

		[Test]
		public void TestAlignSelfFlexEndOverrideFlexStart()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.AlignItems = Align.FlexStart;
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;

			var view1 = new View { IsPlatformEnabled = true };
			view1.HeightRequest = 10;
			view1.WidthRequest = 10;
			FlexLayout.SetAlignSelf(view1, Align.FlexEnd);

			layout.Children.Add(view1);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(90, view1.X);
			Assert.AreEqual(0, view1.Y);
			Assert.AreEqual(10, view1.Width);
			Assert.AreEqual(10, view1.Height);

			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(0, view1.X);
			//Assert.AreEqual(0, view1.Y);
			//Assert.AreEqual(10, view1.Width);
			//Assert.AreEqual(10, view1.Height);   

		}
	}
}
