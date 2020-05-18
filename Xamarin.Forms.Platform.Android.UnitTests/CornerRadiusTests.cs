﻿using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.Platform.Android.UnitTests
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
			var screenshot = await Device.InvokeOnMainThreadAsync(() => { 

				using (var renderer = GetRenderer(visualElement))
				{
					var view = renderer.View;
					Layout(visualElement, view);

					// Need to parent the Frame for it to work on lower APIs (below Marshmallow)
					ParentView(view);
					var image = view.ToBitmap();
					UnparentView(view);

					return image;
				}
			});

			// The corners should show the background color
			screenshot.AssertColorAtTopLeft(EmptyBackground)
				.AssertColorAtTopRight(EmptyBackground)
				.AssertColorAtBottomLeft(EmptyBackground)
				.AssertColorAtBottomRight(EmptyBackground)
				.AssertColorAtCenter(visualElement.BackgroundColor.ToAndroid());
		}
	}
}