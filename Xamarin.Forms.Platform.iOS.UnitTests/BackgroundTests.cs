﻿using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Platform.iOS.UnitTests
{
	[TestFixture]
	public class BackgroundTests : PlatformTestFixture
	{
		static readonly int Tolerance = 40;

		static LinearGradientBrush LinearGradientBrush
		{
			get
			{
				return new LinearGradientBrush
				{
					StartPoint = new Point(0, 0),
					EndPoint = new Point(0, 1),
					GradientStops = new GradientStopCollection
					{
						new GradientStop { Color = Color.Red, Offset = 0.5f },
						new GradientStop { Color = Color.Green, Offset = 1.0f }
					}
				};
			}
		}

		[SetUp]
		public override void Setup()
		{
			base.Setup();

			Device.SetFlags(new[] { "Brush_Experimental" });
		}

		[Test, Category("Background"), Category("Frame")]
		[Description("Frame background should match renderer background")]
		public async Task FrameLinearGradientBrushConsistent()
		{
			var frame = new Frame { HeightRequest = 50, WidthRequest = 50, Background = LinearGradientBrush };
			var screenshot = await GetRendererProperty(frame, (ver) => ver.NativeView.ToBitmap(), requiresLayout: true);

			var screenshotHeight = (int)screenshot.Size.Height;
			var screenshotWidth = (int)screenshot.Size.Width;

			var expectedTopColor = Color.Red.ToUIColor();
			var resultTopColor = screenshot.ColorAtPoint(screenshotWidth / 2, 1);

			Assert.IsTrue(AreColorsSimilar(expectedTopColor, resultTopColor, Tolerance));

			var expectedBottomColor = Color.Green.ToUIColor();
			var resultBottomColor = screenshot.ColorAtPoint(screenshotWidth / 2, screenshotHeight - 1);

			Assert.IsTrue(AreColorsSimilar(expectedBottomColor, resultBottomColor, Tolerance));
		}

		[Test, Category("Background"), Category("BoxView")]
		[Description("BoxView background should match renderer background")]
		public async Task BoxViewLinearGradientBrushConsistent()
		{
			var boxView = new BoxView { HeightRequest = 50, WidthRequest = 50, Background = LinearGradientBrush };
			var screenshot = await GetRendererProperty(boxView, (ver) => ver.NativeView.ToBitmap(), requiresLayout: true);

			var screenshotHeight = (int)screenshot.Size.Height;
			var screenshotWidth = (int)screenshot.Size.Width;

			var expectedTopColor = Color.Red.ToUIColor();
			var resultTopColor = screenshot.ColorAtPoint(screenshotWidth / 2, 1);

			Assert.IsTrue(AreColorsSimilar(expectedTopColor, resultTopColor, Tolerance));

			var expectedBottomColor = Color.Green.ToUIColor();
			var resultBottomColor = screenshot.ColorAtPoint(screenshotWidth / 2, screenshotHeight - 1);

			Assert.IsTrue(AreColorsSimilar(expectedBottomColor, resultBottomColor, Tolerance));
		}
	}
}
