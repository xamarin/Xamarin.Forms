using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7015, "Test the placeholder implementation for Image control ")]
	public class Issue7015 : TestContentPage
	{
		const string batata = "batata.png";
		const string bank = "bank.png";
		const string bell = "bell.png";
		const string fromSource = "ImageSource";
		const string fromPlaceholder = "Placeholder";

		protected override void Init()
		{
			var label = new Label
			{
				Text = "Press the image button and change the Source, from Image above. See the label at the end of the page to see if it's the PlaceholderImage or the ImageSource",
				VerticalOptions = LayoutOptions.Start
			};
			var image = new Image
			{
				Placeholder = bank,
				Source = batata
			};

			var sourceIs = new Label
			{
				Text = fromPlaceholder,
				VerticalOptions = LayoutOptions.EndAndExpand,
				HorizontalTextAlignment = TextAlignment.Center
			};

			var imageB = new ImageButton
			{
				Source = bank,
				Command = new Command(() =>
				{
					var source = image.Source.ToString();
					image.Source = (source.Contains(batata)) ? bell : batata;
					sourceIs.Text = (source.Contains(batata)) ? fromPlaceholder : fromSource;
				})
			};

			var stack = new StackLayout
			{
				Padding = new Thickness(15)
			};

			stack.Children.Add(label);
			stack.Children.Add(image);
			stack.Children.Add(imageB);
			stack.Children.Add(sourceIs);

			Content = stack;
		}
	}
}
