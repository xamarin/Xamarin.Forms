using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Core.UnitTests;
using Xamarin.Forms.FlexLayoutTests;

namespace Xamarin.Forms.FlexLayoutTests
{
	[TestFixture]
	public class FlexLayoutAlignItemsTests : FlexLayoutBaseTestFixture
	{

		[Test]
		public void TestAlignItemsStretch()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.Platform = platform;
			layout.FlexDirection = FlexDirection.Column;
			layout.WidthRequest = 100;
			layout.HeightRequest = 100;

			var view0 = new View { IsPlatformEnabled = true };
			view0.HeightRequest = 10;
			layout.Children.Add(view0);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0f, layout.X);
			Assert.AreEqual(0f, layout.Y);
			Assert.AreEqual(100f, layout.Width);
			Assert.AreEqual(100f, layout.Height);

			Assert.AreEqual(0f, view0.X);
			Assert.AreEqual(0f, view0.Y);
			Assert.AreEqual(100f, view0.Width);
			Assert.AreEqual(10f, view0.Height);
		}

		[Test]
		public void TestAlignItemsCenter()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;
			layout.AlignItems = Align.Center;
			layout.WidthRequest = 100;
			layout.HeightRequest = 100;

			var view0 = new View { IsPlatformEnabled = true };
			view0.WidthRequest = 10;
			view0.HeightRequest = 10;
			layout.Children.Add(view0);


			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0f, layout.X);
			Assert.AreEqual(0f, layout.Y);
			Assert.AreEqual(100f, layout.Width);
			Assert.AreEqual(100f, layout.Height);

			Assert.AreEqual(45f, view0.X);
			Assert.AreEqual(0f, view0.Y);
			Assert.AreEqual(10f, view0.Width);
			Assert.AreEqual(10f, view0.Height);
		}

		[Test]
		public void TestAlignItemsFlexStart()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.Platform = platform;
			layout.FlexDirection = FlexDirection.Column;
			layout.AlignItems = Align.FlexStart;
			layout.WidthRequest = 100;
			layout.HeightRequest = 100;

			var view0 = new View { IsPlatformEnabled = true };
			view0.WidthRequest = 10;
			view0.HeightRequest = 10;
			layout.Children.Add(view0);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0f, layout.X);
			Assert.AreEqual(0f, layout.Y);
			Assert.AreEqual(100f, layout.Width);
			Assert.AreEqual(100f, layout.Height);

			Assert.AreEqual(0f, view0.X);
			Assert.AreEqual(0f, view0.Y);
			Assert.AreEqual(10f, view0.Width);
			Assert.AreEqual(10f, view0.Height);
		}

		[Test]
		public void TestAlignItemsFlexEnd()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;
			layout.AlignItems = Align.FlexEnd;
			layout.WidthRequest = 100;
			layout.HeightRequest = 100;

			var view0 = new View { IsPlatformEnabled = true };
			view0.WidthRequest = 10;
			view0.HeightRequest = 10;
			layout.Children.Add(view0);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0f, layout.X);
			Assert.AreEqual(0f, layout.Y);
			Assert.AreEqual(100f, layout.Width);
			Assert.AreEqual(100f, layout.Height);

			Assert.AreEqual(90f, view0.X);
			Assert.AreEqual(0f, view0.Y);
			Assert.AreEqual(10f, view0.Width);
			Assert.AreEqual(10f, view0.Height);
		}
	}
}
