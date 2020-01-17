using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.ControlGallery.Android.Tests
{
	[TestFixture]
	public class CornerRadiusTests : PlatformTestFixture
	{
		[Test, Category("CornerRadius"), Category("BoxView")]
		public void BoxviewCornerRadius()
		{
			var nativeBackground = Color.White.ToAndroid();
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
				var bitmap = ToBitmap(view);

				// The corners should show the background color
				AssertColorAtPoint(bitmap, nativeBackground, 0, 0);
				AssertColorAtPoint(bitmap, nativeBackground, width, 0);
				AssertColorAtPoint(bitmap, nativeBackground, width, height);
				AssertColorAtPoint(bitmap, nativeBackground, 0, height);

				// The center should be the foreground color
				AssertColorAtPoint(bitmap, nativeForeground, width/2, height/2);
			}
		}
	}
}