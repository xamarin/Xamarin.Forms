using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.FlexLayoutTests
{
	[TestFixture]
	public class FlexLayoutTests : FlexLayoutBaseTestFixture
	{

		[Test]
		public void ThrowsOnNullAdd()
		{
			var layout = new FlexLayout();

			Assert.Throws<ArgumentNullException>(() => layout.Children.Add(null));
		}

		[Test]
		public void ThrowsOnNullRemove()
		{
			var layout = new FlexLayout();

			Assert.Throws<ArgumentNullException>(() => layout.Children.Remove(null));
		}

		[Test]
		public void TestBasicLayout()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true };
			var label2 = new Label { Platform = platform, IsPlatformEnabled = true };

			layout.Children.Add(label1);
			layout.Children.Add(label2);

			layout.Layout(new Rectangle(0, 0, 912, 912));

			Assert.AreEqual(912, layout.Width);
			Assert.AreEqual(912, layout.Height);

			Assert.AreEqual(new Rectangle(0, 0, 100, 912), label1.Bounds);
			Assert.AreEqual(new Rectangle(100, 0, 100, 912), label2.Bounds);
		}

		[Test]
		public void TestBasicLayoutWithElementsWidth()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true, WidthRequest = 100 };
			var label2 = new Label { Platform = platform, IsPlatformEnabled = true, WidthRequest = 100 };

			layout.Children.Add(label1);
			layout.Children.Add(label2);

			layout.Layout(new Rectangle(0, 0, 912, 912));

			Assert.AreEqual(912, layout.Width);
			Assert.AreEqual(912, layout.Height);

			Assert.AreEqual(new Rectangle(0, 0, 100, 912), label1.Bounds);
			Assert.AreEqual(new Rectangle(100, 0, 100, 912), label2.Bounds);
		}

		[Test]
		public void TestBasicLayoutWithElementsWidthAndMargin()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true, WidthRequest = 100, Margin = new Thickness(5, 0, 0, 0) };
			var label2 = new Label { Platform = platform, IsPlatformEnabled = true, WidthRequest = 100, Margin = new Thickness(5, 0, 0, 0) };

			layout.Children.Add(label1);
			layout.Children.Add(label2);

			layout.Layout(new Rectangle(0, 0, 912, 912));

			Assert.AreEqual(912, layout.Width);
			Assert.AreEqual(912, layout.Height);

			Assert.AreEqual(new Rectangle(5, 0, 100, 912), label1.Bounds);
			Assert.AreEqual(new Rectangle(110, 0, 100, 912), label2.Bounds);
		}

		[Test]
		public void TestAttachingViews()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout { FlexDirection = FlexDirection.Column };
			layout.Platform = platform;
			layout.IsPlatformEnabled = true;

			var subView1 = new StackLayout();
			subView1.Platform = platform;
			subView1.IsPlatformEnabled = true;
			subView1.WidthRequest = 100;
			FlexLayout.SetGrow(subView1, 1);
			layout.Children.Add(subView1);

			var subView2 = new StackLayout();
			subView2.Platform = platform;
			subView2.IsPlatformEnabled = true;
			subView2.WidthRequest = 150;
			FlexLayout.SetGrow(subView2, 1);
			layout.Children.Add(subView2);

			foreach (var view in new[] { subView1, subView2 })
			{
				var someView = new Label { Text = "Hello", IsPlatformEnabled = true };
				view.Children.Add(someView);
			}

			layout.Layout(new Rectangle(0, 0, 300, 50));

			foreach (var view in new[] { subView1, subView2 })
			{
				var someView = new Label { Text = "Hello", IsPlatformEnabled = true };
				view.Children.Add(someView);
			}

			layout.Layout(new Rectangle(0, 0, 300, 20));

			Assert.AreEqual(300, subView1.Bounds.Size.Width);
			Assert.AreEqual(20, subView1.Bounds.Size.Height);

			foreach (var subview in subView1.Children)
			{
				var subviewSize = subview.Bounds.Size;
				Assert.AreNotEqual(subviewSize.Width, 0);
				Assert.AreNotEqual(subviewSize.Height, 0);
				Assert.IsFalse(double.IsNaN(subviewSize.Width));
				Assert.IsFalse(double.IsNaN(subviewSize.Height));
			}

			Assert.AreEqual(300, subView2.Bounds.Size.Width);
			Assert.AreEqual(20, subView2.Bounds.Size.Height);

			foreach (var subview in subView2.Children)
			{
				var subviewSize = subview.Bounds.Size;
				Assert.AreNotEqual(subviewSize.Width, 0);
				Assert.AreNotEqual(subviewSize.Height, 0);
				Assert.IsFalse(double.IsNaN(subviewSize.Width));
				Assert.IsFalse(double.IsNaN(subviewSize.Height));
			}
		}

		[Test]
		public void TestIsLeafFlag()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout();
			layout.Platform = platform;

			Assert.True(layout.IsLeaf());

			for (int i = 0; i < 10; i++)
			{
				var children = new Label();
				FlexLayout.SetIsIncluded(children, false);
				layout.Children.Add(children);

			}

			Assert.True(layout.IsLeaf());

			var subview = layout.Children[0];
			FlexLayout.SetIsIncluded(subview, true);

			Assert.False(layout.IsLeaf());
			Assert.True(subview.IsLeaf());

			layout.Children.Clear();

			Assert.True(layout.IsLeaf());
		}


		[Test]
		public void TestIsLeafWithNestedFlexLayouts()
		{
			var platform = new UnitPlatform();
			var layout = new FlexLayout { FlexDirection = FlexDirection.Column };
			layout.WidthRequest = 300;
			layout.HeightRequest = 50;
			layout.Platform = platform;
			layout.IsPlatformEnabled = true;

			var layout2 = new FlexLayout();

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true };

			layout2.Children.Add(label1);

			layout.Children.Add(layout2);

			Assert.IsFalse(layout2.IsLeaf());

			Assert.IsTrue(label1.IsLeaf());

			layout2.Children.Remove(label1);

			Assert.IsTrue(layout2.IsLeaf());
		}

		[Test]
		public void TestSetBounds()
		{
			var layoutSize = new Size(320, 50);
			var platform = new UnitPlatform((arg1, arg2, arg3) =>
			{
				return new SizeRequest(new Size(-1, -1));
			});
			var layout = new FlexLayout { FlexDirection = FlexDirection.Row, AlignItems = Align.FlexStart };
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label1, 1);
			layout.Children.Add(label1);

			var label2 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label2, 1);
			layout.Children.Add(label2);

			var label3 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label3, 1);
			layout.Children.Add(label3);

			layout.Layout(new Rectangle(0, 0, layoutSize.Width, layoutSize.Height));

			Assert.AreEqual(label2.Bounds.Left, Math.Max(label1.Bounds.Left, label1.Bounds.Right), 1);
			Assert.AreEqual(label3.Bounds.Left, Math.Max(label2.Bounds.Left, label2.Bounds.Right), 1);

			double totalWidth = 0;
			foreach (var view in layout.Children)
			{
				totalWidth += view.Bounds.Width;
			}

			Assert.AreEqual(layoutSize.Width, totalWidth, 2);
		}

		[Test]
		public void TestFlexLayoutIsInclude()
		{
			var layoutSize = new Size(300, 50);

			var platform = new UnitPlatform((arg1, arg2, arg3) =>
			{
				return new SizeRequest(new Size(-1, -1));
			});
			var layout = new FlexLayout { FlexDirection = FlexDirection.Row };
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label1, 1);
			layout.Children.Add(label1);

			var label2 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label2, 1);
			layout.Children.Add(label2);

			var label3 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label3, 1);
			layout.Children.Add(label3);

			layout.Layout(new Rectangle(0, 0, layoutSize.Width, layoutSize.Height));

			foreach (var view in layout.Children)
			{
				Assert.AreEqual(100, view.Bounds.Width);
			}
			FlexLayout.SetIsIncluded(label3, false);

			layout.Layout(new Rectangle(0, 0, layoutSize.Width, layoutSize.Height));
			Assert.AreEqual(150, label1.Bounds.Width);
			Assert.AreEqual(150, label2.Bounds.Width);
			Assert.AreEqual(100, label3.Bounds.Width);
		}

		[Test]
		public void TestFlexLayoutIsIncludeChangeWorksOnSecondPass()
		{
			var layoutSize = new Size(300, 50);
			var platform = new UnitPlatform();
			var layout = new FlexLayout { FlexDirection = FlexDirection.Row };
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label1, 1);
			layout.Children.Add(label1);

			var label2 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label2, 1);
			layout.Children.Add(label2);

			var label3 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label3, 1);
			FlexLayout.SetIsIncluded(label3, false);
			layout.Children.Add(label3);

			layout.Layout(new Rectangle(0, 0, layoutSize.Width, layoutSize.Height));

			Assert.AreEqual(150, label1.Bounds.Width);
			Assert.AreEqual(150, label2.Bounds.Width);
			Assert.AreEqual(-1, label3.Bounds.Width);

			FlexLayout.SetIsIncluded(label3, true);
			layout.Layout(new Rectangle(0, 0, layoutSize.Width, layoutSize.Height));
			Assert.AreEqual(100, label1.Bounds.Width);
			Assert.AreEqual(100, label2.Bounds.Width);
			Assert.AreEqual(100, label3.Bounds.Width);
		}

		[Test]
		public void TestSwapChildrenOrder()
		{
			var layoutSize = new Size(300, 50);

			var platform = new UnitPlatform();

			var layout = new FlexLayout { FlexDirection = FlexDirection.Row };
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label1, 1);
			layout.Children.Add(label1);

			var label2 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label2, 1);
			layout.Children.Add(label2);

			var label3 = new Label { Platform = platform, IsPlatformEnabled = true };
			FlexLayout.SetGrow(label3, 1);
			layout.Children.Add(label3);

			layout.Layout(new Rectangle(0, 0, layoutSize.Width, layoutSize.Height));

			Assert.AreEqual(new Rectangle(0, 0, 100, 50), label1.Bounds);
			Assert.AreEqual(new Rectangle(100, 0, 100, 50), label2.Bounds);
			Assert.AreEqual(new Rectangle(200, 0, 100, 50), label3.Bounds);

			FlexLayout.SetIsIncluded(label2, false);
			var lastItem = layout.Children[2];
			layout.Children.Remove(lastItem);
			layout.Children.Insert(0, lastItem);
			layout.Layout(new Rectangle(0, 0, layoutSize.Width, layoutSize.Height));

			Assert.AreEqual(new Rectangle(0, 0, 150, 50), label3.Bounds);
			Assert.AreEqual(new Rectangle(150, 0, 150, 50), label1.Bounds);

			Assert.AreEqual(new Rectangle(100, 0, 100, 50), label2.Bounds);
		}

		[Test]
		public void TestSizeThatFits()
		{
			//var layoutSize = new Size(300, 50);

			var platform = new UnitPlatform(useRealisticLabelMeasure: true);

			var layout = new FlexLayout { FlexDirection = FlexDirection.Row, AlignItems = Align.FlexStart };
			layout.Platform = platform;

			var label1 = new Label { Platform = platform, IsPlatformEnabled = true };
			label1.LineBreakMode = LineBreakMode.TailTruncation;
			label1.Text = @"This is a very very very very very very very very long piece of text.";
			FlexLayout.SetShrink(label1, 1);
			layout.Children.Add(label1);

			var label2 = new Label { Text = "", Platform = platform, IsPlatformEnabled = true };
			label2.WidthRequest = 10;
			label2.HeightRequest = 10;
			layout.Children.Add(label2);
			layout.Layout(new Rectangle(0, 0, 320, 50));

			var label2Size = label2.Measure(double.PositiveInfinity, double.PositiveInfinity);
			Assert.AreEqual(10, label2Size.Request.Height);
			Assert.AreEqual(10, label2Size.Request.Width);

			var label1Size = label1.Measure(double.PositiveInfinity, double.PositiveInfinity);
			//	var layoutSize = layout.Measure(-1, -1);

			//need to mock this with linetruncation
			//Assert.AreEqual(layoutSize.Height, label1Size.Request.Height);
			//Assert.AreEqual(layoutSize.Request.Width, label1Size.Request.Width + label2Size.Request.Width);
		}
	}

	public class BaseTestFixture
	{
		[SetUp]
		public virtual void Setup()
		{
#if !WINDOWS_PHONE
			var culture = Environment.GetEnvironmentVariable("UNIT_TEST_CULTURE");

			if (!string.IsNullOrEmpty(culture))
			{
				var thead = Thread.CurrentThread;
				thead.CurrentCulture = new CultureInfo(culture);
			}
#endif
		}

		[TearDown]
		public virtual void TearDown()
		{

		}
	}

}