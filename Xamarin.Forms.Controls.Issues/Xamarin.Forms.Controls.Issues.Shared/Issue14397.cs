using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14397, "[Bug] iOS Background appears in front of image",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Image)]
#endif
	public class Issue14397 : TestContentPage
	{
		public Issue14397()
		{
			Title = "Issue 14397";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				Text = "If can see the icon with the gradient background, the test has passed."
			};

			var image = new Image
			{
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 100,
				WidthRequest = 100,
				Source = "coffee.png",
				Margin = new Thickness(12, 0)
			};

			image.Background = new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0),
				GradientStops = new GradientStopCollection
				{
					new GradientStop { Color = Color.OrangeRed, Offset = 0.0f },
					new GradientStop { Color = Color.PaleVioletRed, Offset = 0.9f },
				}
			};

			layout.Children.Add(instructions);
			layout.Children.Add(image);

			Content = layout;
		}

		protected override void Init()
		{

		}
	}
}