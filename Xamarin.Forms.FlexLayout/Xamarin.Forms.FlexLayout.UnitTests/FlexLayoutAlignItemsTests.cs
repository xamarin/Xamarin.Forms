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

		[Test]
		public void TestAlignBaseline()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.Platform = platform;
			layout.AlignItems = Align.Baseline;
			layout.WidthRequest = 100;
			layout.HeightRequest = 100;

			var view0 = new View { IsPlatformEnabled = true };
			view0.WidthRequest = 50;
			view0.HeightRequest = 50;
			layout.Children.Add(view0);

			var view1 = new View { IsPlatformEnabled = true };
			view1.WidthRequest = 50;
			view1.HeightRequest = 20;
			layout.Children.Add(view1);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0f, layout.X);
			Assert.AreEqual(0f, layout.Y);
			Assert.AreEqual(100f, layout.Width);
			Assert.AreEqual(100f, layout.Height);

			Assert.AreEqual(0f, view0.X);
			Assert.AreEqual(0f, view0.Y);
			Assert.AreEqual(50f, view0.Width);
			Assert.AreEqual(50f, view0.Height);

			Assert.AreEqual(50f, view1.X);
			Assert.AreEqual(30f, view1.Y);
			Assert.AreEqual(50f, view1.Width);
			Assert.AreEqual(20f, view1.Height);
		}

		[Test]
		public void TestAlignBaselineChild()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.Platform = platform;
			layout.FlexDirection = FlexDirection.Row;
			layout.AlignItems = Align.Baseline;
			layout.WidthRequest = 100;
			layout.HeightRequest = 100;

			var view0 = new View { IsPlatformEnabled = true };
			view0.WidthRequest = 50;
			view0.HeightRequest = 50;
			layout.Children.Add(view0);

			var view1 = new FlexLayout { IsPlatformEnabled = true };
			view1.FlexDirection = FlexDirection.Column;
			view1.WidthRequest = 50;
			view1.HeightRequest = 20;
			layout.Children.Add(view1);

			var view1_child0 = new View { IsPlatformEnabled = true };
			view1_child0.WidthRequest = 50;
			view1_child0.HeightRequest = 10;
			view1.Children.Add(view1_child0);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0f, layout.X);
			Assert.AreEqual(0f, layout.Y);
			Assert.AreEqual(100f, layout.Width);
			Assert.AreEqual(100f, layout.Height);

			Assert.AreEqual(0f, view0.X);
			Assert.AreEqual(0f, view0.Y);
			Assert.AreEqual(50f, view0.Width);
			Assert.AreEqual(50f, view0.Height);

			Assert.AreEqual(50f, view1.X);
			Assert.AreEqual(40f, view1.Y);
			Assert.AreEqual(50f, view1.Width);
			Assert.AreEqual(20f, view1.Height);

			Assert.AreEqual(0f, view1_child0.X);
			Assert.AreEqual(0f, view1_child0.Y);
			Assert.AreEqual(50f, view1_child0.Width);
			Assert.AreEqual(10f, view1_child0.Height);
		}

		//TODO: Missing more Baseline tests
		//[Test]
		//public void TestAlignBaselineChildMultiline()
		//{
		//	var platform = new UnitPlatform();
		//	var layout = new FlexLayout();
		//	layout.Platform = platform;
		//	layout.FlexDirection = FlexDirection.Row;
		//	layout.AlignItems = Align.Baseline;
		//	layout.WidthRequest = 100;
		//	layout.HeightRequest = 100;

		//	var view0 = new View { IsPlatformEnabled = true };
		//	view0.WidthRequest = 50;
		//	view0.HeightRequest = 60;
		//	layout.Children.Add(view0);

		//	var view1 = new FlexLayout { IsPlatformEnabled = true };
		//	view1.FlexDirection = FlexDirection.Row;
		//	view1.Wrap = Wrap.Wrap;
		//	view1.WidthRequest = 50;
		//	view1.HeightRequest = 25;
		//	layout.Children.Add(view1);

		//	var view1_child0 = new View { IsPlatformEnabled = true };		
		//	view1_child0.WidthRequest = 25;
		//	view1_child0.HeightRequest = 20;
		//	view1.Children.Add(view1_child0);

		//	var view1_child1 = new View { IsPlatformEnabled = true };
		//	view1_child1.WidthRequest = 25;
		//	view1_child1.HeightRequest = 10;
		//	view1.Children.Add(view1_child1);

		//	var view1_child2 = new View { IsPlatformEnabled = true };
		//	view1_child2.WidthRequest = 25;
		//	view1_child2.HeightRequest = 20;
		//	view1.Children.Add(view1_child2);

		//	var view1_child3 = new View { IsPlatformEnabled = true };
		//	view1_child3.WidthRequest = 25;
		//	view1_child3.HeightRequest = 10;
		//	view1.Children.Add(view1_child3);

		//	layout.Layout(new Rectangle(0, 0, 100, 100));

		//	Assert.AreEqual(0f, layout.X);
		//	Assert.AreEqual(0f, layout.Y);
		//	Assert.AreEqual(100f, layout.Width);
		//	Assert.AreEqual(100f, layout.Height);

		//	Assert.AreEqual(0f, view0.X);
		//	Assert.AreEqual(0f, view0.Y);
		//	Assert.AreEqual(50f, view0.Width);
		//	Assert.AreEqual(60f, view0.Height);

		//	Assert.AreEqual(50f, view1.X);
		//	Assert.AreEqual(40f, view1.Y);
		//	Assert.AreEqual(50f, view1.Width);
		//	Assert.AreEqual(25f, view1.Height);

		//	Assert.AreEqual(0f, view1_child0.X);
		//	Assert.AreEqual(0f, view1_child0.Y);
		//	Assert.AreEqual(25f, view1_child0.Width);
		//	Assert.AreEqual(20f, view1_child0.Height);

		//	Assert.AreEqual(25f, view1_child1.X);
		//	Assert.AreEqual(0f, view1_child1.Y);
		//	Assert.AreEqual(25f, view1_child1.Width);
		//	Assert.AreEqual(10f, view1_child1.Height);

		//	Assert.AreEqual(0f, view1_child2.X);
		//	Assert.AreEqual(20f, view1_child2.Y);
		//	Assert.AreEqual(25f, view1_child2.Width);
		//	Assert.AreEqual(20f, view1_child2.Height);

		//	Assert.AreEqual(25f, view1_child3.X);
		//	Assert.AreEqual(20f, view1_child3.Y);
		//	Assert.AreEqual(25f, view1_child3.Width);
		//	Assert.AreEqual(10f, view1_child3.Height);
		//}

		
	}
}
