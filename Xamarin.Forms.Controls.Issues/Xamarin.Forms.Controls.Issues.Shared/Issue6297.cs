using System.Linq;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6297, "Frame size gets corrupted when ListView is scrolled (RecycleElement CachingStrategy)", PlatformAffected.Android)]
	public class Issue6297 : ContentPage
	{
		public Issue6297()
		{
			var grid = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto},
					new RowDefinition(),
				}
			};
			grid.AddChild(new Label
			{
				Text = "Scroll up and down several times and make sure Frame size is accurate.",
				VerticalTextAlignment = TextAlignment.Center
			}, 0, 0);
			grid.AddChild(new ListView(ListViewCachingStrategy.RecycleElement)
			{
				HasUnevenRows = true,
				ItemsSource = Enumerable.Range(0, 99).Select(i => i % 2 == 0 ? "small" : "big string > big frame"),
				ItemTemplate = new DataTemplate(() =>
				{
					var text = new Label
					{
						VerticalOptions = LayoutOptions.Fill,
						TextColor = Color.White,
					};

					text.SetBinding(Label.TextProperty, ".");
					var view = new Grid
					{
						HeightRequest = 200,
						Margin = new Thickness(0, 10, 0, 0),
						BackgroundColor = Color.FromHex("#F1F1F1")
					};
					view.AddChild(new Frame
					{
						Padding = new Thickness(5),
						Margin = new Thickness(0, 0, 10, 0),
						BorderColor = Color.Blue,
						BackgroundColor = Color.Gray,
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.End,
						CornerRadius = 3,
						Content = text
					}, 0, 0);
					return new ViewCell
					{
						View = view
					};
				})
			}, 0, 1);

			Content = grid;
		}
	}
}