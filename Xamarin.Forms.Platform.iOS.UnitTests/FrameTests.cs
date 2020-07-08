using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using NUnit.Framework;

namespace Xamarin.Forms.Platform.iOS.UnitTests
{
	[TestFixture]
	public class FrameTests : PlatformTestFixture
	{
		[Test, Category("Frame")]
		public void ReusingFrameRendererDoesCauseOverlapWithPreviousContent()
		{
			Frame frame1 = new Frame()
			{
				Content = new Label()
				{
					Text = "I am frame 1"
				}
			};

			var frameRenderer = GetRenderer(frame1);

			Frame frame2 = new Frame()
			{
				Content = new Label()
				{
					Text = "I am frame 2"
				}
			};

			frameRenderer.SetElement(frame2);
		}
	}
}