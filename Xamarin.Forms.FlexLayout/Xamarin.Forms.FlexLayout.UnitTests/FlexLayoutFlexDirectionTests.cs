using System;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.FlexLayoutTests
{
	[TestFixture]
	public class FlexLayoutFlexDirectionTests : FlexLayoutBaseTestFixture
	{
		[Test]
		public void TestFlexDirectionColumnWithoutHeight()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.AlignItems = Align.FlexStart;
			layout.Platform = platform;

			var label1 = new View { Platform = platform, IsPlatformEnabled = true };
			label1.HeightRequest = 10;
			var label2 = new View { Platform = platform, IsPlatformEnabled = true };
			label2.HeightRequest = 10;
			var label3 = new View { Platform = platform, IsPlatformEnabled = true };
			label3.HeightRequest = 10;

			layout.Children.Add(label1);
			layout.Children.Add(label2);
			layout.Children.Add(label3);

			var sizeRequest = layout.Measure(100, double.PositiveInfinity);
			layout.Layout(new Rectangle(0, 0, sizeRequest.Request.Width, sizeRequest.Request.Height));

			Assert.AreEqual(0f, layout.X);
			Assert.AreEqual(0f, layout.Y);
			Assert.AreEqual(100f, layout.Width);
			Assert.AreEqual(30f, layout.Height);

			Assert.AreEqual(0f, label1.X);
			Assert.AreEqual(0f, label1.Y);
			Assert.AreEqual(100f, label1.Width);
			Assert.AreEqual(10f, label1.Height);

			Assert.AreEqual(0f, label2.X);
			Assert.AreEqual(10f, label2.Y);
			Assert.AreEqual(100f, label2.Width);
			Assert.AreEqual(10f, label2.Height);

			Assert.AreEqual(0f, label3.X);
			Assert.AreEqual(20f, label3.Y);
			Assert.AreEqual(100f, label3.Width);
			Assert.AreEqual(10f, label3.Height);

			//TODO: change direction
			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, double.PositiveInfinity));

			//Assert.AreEqual(0f, layout.X);
			//Assert.AreEqual(0f, layout.Y);
			//Assert.AreEqual(100f, layout.Width);
			////Assert.AreEqual(30f, layout.Height);

			//Assert.AreEqual(0f, label1.X);
			//Assert.AreEqual(0f, label1.Y);
			//Assert.AreEqual(100f, label1.Width);
			//Assert.AreEqual(10f, label1.Height);

			//Assert.AreEqual(0f, label2.X);
			//Assert.AreEqual(10f, label2.Y);
			//Assert.AreEqual(100f, label2.Width);
			//Assert.AreEqual(10f, label2.Height);

			//Assert.AreEqual(0f, label3.X);
			//Assert.AreEqual(20f, label3.Y);
			//Assert.AreEqual(100f, label3.Width);
			//Assert.AreEqual(10f, label3.Height);
		}

		[Test]
		public void TestFlexDirectionRowNoWidth()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Row;
			layout.Platform = platform;

			var label1 = new View { Platform = platform, IsPlatformEnabled = true };
			label1.WidthRequest = 10;
			var label2 = new View { Platform = platform, IsPlatformEnabled = true };
			label2.WidthRequest = 10;
			var label3 = new View { Platform = platform, IsPlatformEnabled = true };
			label3.WidthRequest = 10;

			layout.Children.Add(label1);
			layout.Children.Add(label2);
			layout.Children.Add(label3);


			var measure = layout.Measure(double.PositiveInfinity, 100);
			layout.Layout(new Rectangle(0, 0, measure.Request.Width, measure.Request.Height));

			Assert.AreEqual(0f, layout.X);
			Assert.AreEqual(0f, layout.Y);
			Assert.AreEqual(30f, layout.Width);
			Assert.AreEqual(100f, layout.Height);

			Assert.AreEqual(0f, label1.X);
			Assert.AreEqual(0f, label1.Y);
			Assert.AreEqual(10f, label1.Width);
			Assert.AreEqual(100f, label1.Height);

			Assert.AreEqual(10f, label2.X);
			Assert.AreEqual(0f, label2.Y);
			Assert.AreEqual(10f, label2.Width);
			Assert.AreEqual(100f, label2.Height);

			Assert.AreEqual(20f, label3.X);
			Assert.AreEqual(0f, label3.Y);
			Assert.AreEqual(10f, label3.Width);
			Assert.AreEqual(100f, label3.Height);

			//TODO: change direction
			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, double.PositiveInfinity, 100));

			//Assert.AreEqual(0f, label1.X);
			//Assert.AreEqual(0f, label1.Y);
			//Assert.AreEqual(10f, label1.Width);
			//Assert.AreEqual(100f, label1.Height);

			//Assert.AreEqual(10f, label2.X);
			//Assert.AreEqual(0f, label2.Y);
			//Assert.AreEqual(10f, label2.Width);
			//Assert.AreEqual(100f, label2.Height);

			//Assert.AreEqual(20f, label3.X);
			//Assert.AreEqual(0f, label3.Y);
			//Assert.AreEqual(10f, label3.Width);
			//Assert.AreEqual(100f, label3.Height);
		}

		[Test]
		public void TestFlexDirectionColumn()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Column;
			layout.Platform = platform;

			var label1 = new View { Platform = platform, IsPlatformEnabled = true };
			label1.HeightRequest = 10;
			var label2 = new View { Platform = platform, IsPlatformEnabled = true };
			label2.HeightRequest = 10;
			var label3 = new View { Platform = platform, IsPlatformEnabled = true };
			label3.HeightRequest = 10;

			layout.Children.Add(label1);
			layout.Children.Add(label2);
			layout.Children.Add(label3);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0f, layout.X);
			Assert.AreEqual(0f, layout.Y);
			Assert.AreEqual(100f, layout.Width);
			Assert.AreEqual(100f, layout.Height);

			Assert.AreEqual(0f, label1.X);
			Assert.AreEqual(0f, label1.Y);
			Assert.AreEqual(100f, label1.Width);
			Assert.AreEqual(10f, label1.Height);

			Assert.AreEqual(0f, label2.X);
			Assert.AreEqual(10f, label2.Y);
			Assert.AreEqual(100f, label2.Width);
			Assert.AreEqual(10f, label2.Height);

			Assert.AreEqual(0f, label3.X);
			Assert.AreEqual(20f, label3.Y);
			Assert.AreEqual(100f, label3.Width);
			Assert.AreEqual(10f, label3.Height);

			//TODO: change direction
			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0f, layout.X);
			//Assert.AreEqual(0f, layout.Y);
			//Assert.AreEqual(100f, layout.Width);
			//Assert.AreEqual(100f, layout.Height);

			//Assert.AreEqual(0f, label1.X);
			//Assert.AreEqual(0f, label1.Y);
			//Assert.AreEqual(100f, label1.Width);
			//Assert.AreEqual(10f, label1.Height);

			//Assert.AreEqual(0f, label2.X);
			//Assert.AreEqual(10f, label2.Y);
			//Assert.AreEqual(100f, label2.Width);
			//Assert.AreEqual(10f, label2.Height);

			//Assert.AreEqual(0f, label3.X);
			//Assert.AreEqual(20f, label3.Y);
			//Assert.AreEqual(100f, label3.Width);
			//Assert.AreEqual(10f, label3.Height);

		}

		[Test]
		public void TestFlexDirectionRow()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.Row;
			layout.Platform = platform;

			var label1 = new View { Platform = platform, IsPlatformEnabled = true };
			label1.WidthRequest = 10;
			var label2 = new View { Platform = platform, IsPlatformEnabled = true };
			label2.WidthRequest = 10;
			var label3 = new View { Platform = platform, IsPlatformEnabled = true };
			label3.WidthRequest = 10;

			layout.Children.Add(label1);
			layout.Children.Add(label2);
			layout.Children.Add(label3);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(0f, label1.X);
			Assert.AreEqual(0f, label1.Y);
			Assert.AreEqual(10f, label1.Width);
			Assert.AreEqual(100f, label1.Height);

			Assert.AreEqual(10f, label2.X);
			Assert.AreEqual(0f, label2.Y);
			Assert.AreEqual(10f, label2.Width);
			Assert.AreEqual(100f, label2.Height);

			Assert.AreEqual(20f, label3.X);
			Assert.AreEqual(0f, label3.Y);
			Assert.AreEqual(10f, label3.Width);
			Assert.AreEqual(100f, label3.Height);

			//TODO: change direction
			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(90, label1.X);
			//Assert.AreEqual(0, label1.Y);
			//Assert.AreEqual(10, label1.Width);
			//Assert.AreEqual(100, label1.Height);

			//Assert.AreEqual(80, label2.X);
			//Assert.AreEqual(0, label2.Y);
			//Assert.AreEqual(10, label2.Width);
			//Assert.AreEqual(100, label2.Height);

			//Assert.AreEqual(70, label3.X);
			//Assert.AreEqual(0, label3.Y);
			//Assert.AreEqual(10, label3.Width);
			//Assert.AreEqual(100, label3.Height);
		}

		[Test]
		public void TestFlexDirectionColumnReverse()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.ColumnReverse;
			layout.Platform = platform;

			var label1 = new View { Platform = platform, IsPlatformEnabled = true };
			label1.HeightRequest = 10;
			var label2 = new View { Platform = platform, IsPlatformEnabled = true };
			label2.HeightRequest = 10;
			var label3 = new View { Platform = platform, IsPlatformEnabled = true };
			label3.HeightRequest = 10;

			layout.Children.Add(label1);
			layout.Children.Add(label2);
			layout.Children.Add(label3);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(0, label1.X);
			Assert.AreEqual(90, label1.Y);
			Assert.AreEqual(100, label1.Width);
			Assert.AreEqual(10, label1.Height);

			Assert.AreEqual(0, label2.X);
			Assert.AreEqual(80, label2.Y);
			Assert.AreEqual(100, label2.Width);
			Assert.AreEqual(10, label2.Height);

			Assert.AreEqual(0, label3.X);
			Assert.AreEqual(70, label3.Y);
			Assert.AreEqual(100, label3.Width);
			Assert.AreEqual(10, label3.Height);

			//TODO: change direction
			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(0, label1.X);
			//Assert.AreEqual(90, label1.Y);
			//Assert.AreEqual(100, label1.Width);
			//Assert.AreEqual(10, label1.Height);

			//Assert.AreEqual(0, label2.X);
			//Assert.AreEqual(80, label2.Y);
			//Assert.AreEqual(100, label2.Width);
			//Assert.AreEqual(10, label2.Height);

			//Assert.AreEqual(0, label3.X);
			//Assert.AreEqual(70, label3.Y);
			//Assert.AreEqual(100, label3.Width);
			//Assert.AreEqual(10, label3.Height);
		}

		[Test]
		public void TestFlexDirectionRowReverse()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.FlexDirection = FlexDirection.RowReverse;
			layout.Platform = platform;

			var label1 = new View { Platform = platform, IsPlatformEnabled = true };
			label1.WidthRequest = 10;
			var label2 = new View { Platform = platform, IsPlatformEnabled = true };
			label2.WidthRequest = 10;
			var label3 = new View { Platform = platform, IsPlatformEnabled = true };
			label3.WidthRequest = 10;

			layout.Children.Add(label1);
			layout.Children.Add(label2);
			layout.Children.Add(label3);

			layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(0, layout.X);
			Assert.AreEqual(0, layout.Y);
			Assert.AreEqual(100, layout.Width);
			Assert.AreEqual(100, layout.Height);

			Assert.AreEqual(90, label1.X);
			Assert.AreEqual(0, label1.Y);
			Assert.AreEqual(10, label1.Width);
			Assert.AreEqual(100, label1.Height);

			Assert.AreEqual(80, label2.X);
			Assert.AreEqual(0, label2.Y);
			Assert.AreEqual(10, label2.Width);
			Assert.AreEqual(100, label2.Height);

			Assert.AreEqual(70, label3.X);
			Assert.AreEqual(0, label3.Y);
			Assert.AreEqual(10, label3.Width);
			Assert.AreEqual(100, label3.Height);

			//TODO: change direction
			//layout.FlowDirection = FlowDirection.RightToLeft;
			//layout.Layout(new Rectangle(0, 0, 100, 100));

			//Assert.AreEqual(0, layout.X);
			//Assert.AreEqual(0, layout.Y);
			//Assert.AreEqual(100, layout.Width);
			//Assert.AreEqual(100, layout.Height);

			//Assert.AreEqual(0, label1.X);
			//Assert.AreEqual(0, label1.Y);
			//Assert.AreEqual(10, label1.Width);
			//Assert.AreEqual(100, label1.Height);

			//Assert.AreEqual(10, label2.X);
			//Assert.AreEqual(0, label2.Y);
			//Assert.AreEqual(10, label2.Width);
			//Assert.AreEqual(100, label2.Height);

			//Assert.AreEqual(20, label3.X);
			//Assert.AreEqual(0, label3.Y);
			//Assert.AreEqual(10, label3.Width);
			//Assert.AreEqual(100, label3.Height);        
		}
	}
}

