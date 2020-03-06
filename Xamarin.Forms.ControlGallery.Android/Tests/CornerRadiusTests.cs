using System.Threading.Tasks;
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
		public async Task BoxviewCornerRadius()
		{
			var boxView = new BoxView
			{
				HeightRequest = 100,
				WidthRequest = 200,
				CornerRadius = 15,
				BackgroundColor = Color.Red
			};

			await CheckCornerRadius(boxView);
		}
	
		[Test, Category("CornerRadius"), Category("Button")]
		public async Task ButtonCornerRadius()
		{
			var backgroundColor = Color.Red;

			var button = new Button
			{
				HeightRequest = 100,
				WidthRequest = 200,
				CornerRadius = 15,
				BackgroundColor = backgroundColor
			};

			await CheckCornerRadius(button);
		}

		[Test, Category("CornerRadius"), Category("Frame")]
		public async Task FrameCornerRadius()
		{
			var backgroundColor = Color.Red;

			var frame = new Frame
			{
				HeightRequest = 100,
				WidthRequest = 200,
				CornerRadius = 15,
				BackgroundColor = backgroundColor
			};

			await CheckCornerRadius(frame);
		}

		[Test, Category("CornerRadius"), Category("ImageButton")]
		public async Task ImageButtonCornerRadius()
		{
			var backgroundColor = Color.Red;

			var button = new ImageButton
			{
				HeightRequest = 100,
				WidthRequest = 200,
				CornerRadius = 15,
				BackgroundColor = backgroundColor,
				BorderColor = Color.Black,
				BorderWidth = 2
			};

			await CheckCornerRadius(button);
		}

		public async Task CheckCornerRadius(VisualElement visualElement) 
		{
			var centerColor = visualElement.BackgroundColor.ToAndroid();
			var screenshot = await GetRendererProperty(visualElement, ver => ver.View.ToBitmap(), requiresLayout: true);
			screenshot.AssertColorAtTopLeft(EmptyBackground)
				.AssertColorAtTopRight(EmptyBackground)
				.AssertColorAtBottomLeft(EmptyBackground)
				.AssertColorAtBottomRight(EmptyBackground)
				.AssertColorAtCenter(centerColor);
		}
	}
}