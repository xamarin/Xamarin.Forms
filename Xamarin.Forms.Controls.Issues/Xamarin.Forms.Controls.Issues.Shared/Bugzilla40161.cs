using System;

using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40161, "Layout not invalidated when changing source of image", PlatformAffected.Android)]
	public class Bugzilla40161 : TestContentPage
	{
		const string image1 = "xamagon.png";
		const string image2 = "crimson.jpg";

		protected override void Init()
		{
			var image = new Image {
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Margin = 20
			};

			var label = new Label();

			var layout = new AbsoluteLayout {
				Padding = 0,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill
			};

			layout.Children.Add(image, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.HeightProportional);
			layout.Children.Add(label);

			Content = layout;

			image.Source = image1;
			label.Text = image1;

			Device.StartTimer(TimeSpan.FromSeconds(2), () => {
				image.Source = image2;
				label.Text = image2;
				return false;
			});
		}

#if UITEST
		[Test]
		public void Bugzilla40161Test()
		{
			RunningApp.Screenshot("I see the Xamagon. It fits the screen with a small margin.");
			RunningApp.WaitForElement(q => q.Marked(image2));
			RunningApp.Screenshot("I see the face. It fits the screen with a small margin.");
		}
#endif
	}
}
