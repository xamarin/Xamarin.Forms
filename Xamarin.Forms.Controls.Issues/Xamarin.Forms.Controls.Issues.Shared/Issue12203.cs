using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;

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
	[Issue(IssueTracker.Github, 12203, "[Bug] Inconsistent behavior when setting StrokeDashArray values on an Ellipse", PlatformAffected.iOS)]
	public class Issue12203 : TestContentPage
	{
		public Issue12203()
		{

		}

		protected override void Init()
		{
			Title = "Issue 12203";

			var layout = new StackLayout
			{
				Padding = 12
			};

			var instructions = new Label
			{
				Text = "All Ellipses must be rendered in the same way on Android, iOS, etc."
			};

			var nullLabel = new Label
			{
				Text = "Null"
			};

			var nullEllipse = new Ellipse
			{
				Fill = Brush.Red,
				Stroke = Brush.Black,
				StrokeThickness = 2,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 50,
				WidthRequest = 50
			};

			nullEllipse.StrokeDashArray = null;

			var emptyLabel = new Label
			{
				Text = "An empty DoubleCollection"
			};

			var emptyEllipse = new Ellipse
			{
				Fill = Brush.Red,
				Stroke = Brush.Black,
				StrokeThickness = 2,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 50,
				WidthRequest = 50
			};

			emptyEllipse.StrokeDashArray = new DoubleCollection();

			var singleLabel = new Label
			{
				Text = "A DoubleCollection with a single value greater than 0.0 e.g. 1.0"
			};

			var singleEllipse = new Ellipse
			{
				Fill = Brush.Red,
				Stroke = Brush.Black,
				StrokeThickness = 2,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 50,
				WidthRequest = 50
			};

			singleEllipse.StrokeDashArray = new DoubleCollection { 1.0 };

			var two01Label = new Label
			{
				Text = "A DoubleCollection with two or more values greater than 0.0 e.g. { 1.0, 1.0 }"
			};

			var two01Ellipse = new Ellipse
			{
				Fill = Brush.Red,
				Stroke = Brush.Black,
				StrokeThickness = 2,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 50,
				WidthRequest = 50
			};

			two01Ellipse.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };

			var two02Label = new Label
			{
				Text = "A DoubleCollection with two values, the first bring set to 1.0 and the second to 4.0"
			};

			var two02Ellipse = new Ellipse
			{
				Fill = Brush.Red,
				Stroke = Brush.Black,
				StrokeThickness = 2,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 50,
				WidthRequest = 50
			};

			two02Ellipse.StrokeDashArray = new DoubleCollection { 1.0, 4.0 };

			layout.Children.Add(instructions);

			layout.Children.Add(nullLabel);
			layout.Children.Add(nullEllipse);

			layout.Children.Add(emptyLabel);
			layout.Children.Add(emptyEllipse);

			layout.Children.Add(singleLabel);
			layout.Children.Add(singleEllipse);

			layout.Children.Add(two01Label);
			layout.Children.Add(two01Ellipse);

			layout.Children.Add(two02Label);
			layout.Children.Add(two02Ellipse);

			Content = layout;
		}
	}
}