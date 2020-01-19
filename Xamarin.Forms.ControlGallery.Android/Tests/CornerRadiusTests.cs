using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Platform.Android;
using AColor = Android.Graphics.Color;

namespace Xamarin.Forms.ControlGallery.Android.Tests
{
	[TestFixture]
	public class CornerRadiusTests : PlatformTestFixture
	{
		[Test, Category("CornerRadius"), Category("BoxView")]
		public void BoxviewCornerRadius()
		{
			var nativeBackground = new AColor(0, 0, 0, 255);
			var nativeForeground = Color.Red.ToAndroid();
			int height = 100;
			int width = 300;

			var ve = new BoxView
			{
				HeightRequest = height,
				WidthRequest = width,
				CornerRadius = 15,
				Color = Color.Red,
			};

			using (var renderer = GetRenderer(ve))
			{
				var view = renderer.View;
				Layout(ve, view);

				// TODO Provide a way to give a view and a set of point/color expected values
				// so that we don't need bitmap at this level
				// maybe even give the values as Forms colors

				var bitmap = ToBitmap(view);

				// The corners should show the background color
				AssertColorAtPoint(bitmap, nativeBackground, 0, 0);
				AssertColorAtPoint(bitmap, nativeBackground, width - 1, 0);
				AssertColorAtPoint(bitmap, nativeBackground, width - 1, height - 1);
				AssertColorAtPoint(bitmap, nativeBackground, 0, height - 1);

				// The center should be the foreground color
				AssertColorAtPoint(bitmap, nativeForeground, width/2, height/2);
			}
		}
	}
}