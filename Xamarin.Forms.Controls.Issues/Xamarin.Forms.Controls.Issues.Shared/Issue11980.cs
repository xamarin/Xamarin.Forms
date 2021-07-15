using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11980, "[iOS] ScaledScreenSize Height return wrong value when the Device Orientation is Faceup",
		PlatformAffected.iOS)]
	public class Issue11980 : TestContentPage
	{
		public Issue11980()
		{
		}

		protected override void Init()
		{
			Title = "Issue 11980";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Using Face Up DeviceOrientation, tap the button. If the width and height values are different, the test has passed."
			};

			var button = new Button
			{
				Text = "Get ScaledScreenSize"
			};

			var heightLabel = new Label();
			var widthLabel = new Label();

			layout.Children.Add(instructions);
			layout.Children.Add(button);
			layout.Children.Add(heightLabel);
			layout.Children.Add(widthLabel);

			Content = layout;

			button.Clicked += (sender, args) =>
			{
				heightLabel.Text = $"Height {Device.Info.ScaledScreenSize.Height}";
				widthLabel.Text = $"Width {Device.Info.ScaledScreenSize.Width}";
			};
		}
	}
}