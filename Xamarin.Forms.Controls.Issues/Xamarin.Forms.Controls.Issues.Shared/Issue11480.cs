using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11480, "Scale on shapes are blurry (iOS)",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shape)]
#endif
	public class Issue11480 : TestContentPage
	{
		public Issue11480()
		{

		}

		protected override void Init()
		{
			Title = "11480";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If the ellipse is not blurry, the test has passed."
			};

			var ellipse = new Ellipse
			{
				Stroke = Brush.Red,
				StrokeThickness = 5,
				HeightRequest = 50,
				WidthRequest = 50,
				HorizontalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 120, 0, 0)
			};

			ellipse.Scale = 5;

			layout.Children.Add(instructions);
			layout.Children.Add(ellipse);

			Content = layout;
		}
	}
}