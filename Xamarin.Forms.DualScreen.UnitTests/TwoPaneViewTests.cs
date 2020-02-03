using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.DualScreen.UnitTests
{
	[TestFixture]
    public class TwoPaneViewTests : BaseTestFixture
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();
			Device.PlatformServices = new MockPlatformServices();
			Device.info = new TestDeviceInfo();
		}

		[Test]
		public void GettersAndSetters()
		{
			var Pane1 = new StackLayout();
			var	Pane2 = new Grid();

			TwoPaneView twoPaneView = new TwoPaneView()
			{
				TallModeConfiguration = TwoPaneViewTallModeConfiguration.SinglePane,
				WideModeConfiguration = TwoPaneViewWideModeConfiguration.SinglePane,
				Pane1 = Pane1,
				Pane2 = Pane2,
				PanePriority = TwoPaneViewPriority.Pane2,
				MinTallModeHeight = 1000,
				MinWideModeWidth = 2000,
			};

			Assert.AreEqual(TwoPaneViewTallModeConfiguration.SinglePane, twoPaneView.TallModeConfiguration);
			Assert.AreEqual(TwoPaneViewWideModeConfiguration.SinglePane, twoPaneView.WideModeConfiguration);
			Assert.AreEqual(Pane1, twoPaneView.Pane1);
			Assert.AreEqual(Pane2, twoPaneView.Pane2);
			Assert.AreEqual(TwoPaneViewPriority.Pane2, twoPaneView.PanePriority);
			Assert.AreEqual(1000, twoPaneView.MinTallModeHeight);
			Assert.AreEqual(2000, twoPaneView.MinWideModeWidth);
		}

		[Test]
		public void BasicLayoutTest()
		{
			TwoPaneView twoPaneView = new TwoPaneView();
			twoPaneView.Layout(new Rectangle(0, 0, 300, 300));

			Assert.AreEqual(300, twoPaneView.Height);
			Assert.AreEqual(300, twoPaneView.Width);
		}

		[Test]
		public void ModeSwitchesWithMinWideModeWidth()
		{
			TwoPaneView twoPaneView = new TwoPaneView();
			twoPaneView.Layout(new Rectangle(0, 0, 300, 300));

			twoPaneView.MinWideModeWidth = 400;
			Assert.AreEqual(TwoPaneViewMode.SinglePane, twoPaneView.Mode);
			twoPaneView.MinWideModeWidth = 100;
			Assert.AreEqual(TwoPaneViewMode.Wide, twoPaneView.Mode);
		}

		[Test]
		public void ModeSwitchesWithMinTallModeHeight()
		{
			TwoPaneView twoPaneView = new TwoPaneView();
			twoPaneView.Layout(new Rectangle(0, 0, 300, 300));

			twoPaneView.MinTallModeHeight = 400;
			Assert.AreEqual(TwoPaneViewMode.SinglePane, twoPaneView.Mode);
			twoPaneView.MinTallModeHeight = 100;
			Assert.AreEqual(TwoPaneViewMode.Tall, twoPaneView.Mode);
		}

		public class Thing : StackLayout
		{
			protected override void LayoutChildren(double x, double y, double width, double height)
			{
				base.LayoutChildren(x, y, width, height);
			}

			protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
			{
				return base.OnMeasure(widthConstraint, heightConstraint);
			}
		}
		
		public class ThingPaneView : TwoPaneView
		{
			protected override void LayoutChildren(double x, double y, double width, double height)
			{
				base.LayoutChildren(x, y, width, height);
			}

			protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
			{
				return base.OnMeasure(widthConstraint, heightConstraint);
			}
		}

		[Test]
		//public void PaneLengthTallMode()
		//{
		//	TwoPaneView twoPaneView = new ThingPaneView()
		//	{
		//		Pane1 = new BoxView()
		//		{
		//			IsPlatformEnabled = true
		//		},
		//		Pane2 = new BoxView()
		//		{
		//			IsPlatformEnabled = true
		//		},
		//		IsPlatformEnabled = true
		//	};

		//	twoPaneView.Layout(new Rectangle(0, 0, 300, 300));
		//	twoPaneView.MinTallModeHeight = 100;
		//	twoPaneView.Pane1Length = 100;
		//	Assert.AreEqual(100, twoPaneView.Pane1.Height);
		//}


		internal class TestDeviceInfo : DeviceInfo
		{
			public TestDeviceInfo()
			{
				CurrentOrientation = DeviceOrientation.Portrait;
			}
			public override Size PixelScreenSize
			{
				get { return new Size(1000, 2000); }
			}

			public override Size ScaledScreenSize
			{
				get { return new Size(500, 1000); }
			}

			public override double ScalingFactor
			{
				get { return 2; }
			}
		}

	}
}
