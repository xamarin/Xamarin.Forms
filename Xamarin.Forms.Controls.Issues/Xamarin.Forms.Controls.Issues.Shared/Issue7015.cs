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
		const string correctUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn%3AANd9GcTZDQ1nLIpshq9ubfuv20tS28rc3i-rxyJMod0A_V-_5caaB34N";
		const string wrongUrl = "http://avatars.githubusercontent.co/u/20712372?s=400&u=ecb5fe0584cba02ab4c7e159768e9366a95e3&v=4";

		protected override void Init()
		{
			var label = new Label
			{
				Text = "Press the image button and change the Source, from Image above. See the label at the end of the page to see if it's the PlaceholderImage or the ImageSource",
				VerticalOptions = LayoutOptions.Start
			};
			var image = new Image
			{
				ErrorPlaceholder = bank,
				Source = batata
			};

			var urlImage = new Image
			{
				ErrorPlaceholder = bank,
				Source = wrongUrl,
				LoadingPlaceholder = bell
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
					image.Source = (source.Contains(wrongUrl)) ? correctUrl : wrongUrl;
					sourceIs.Text = (source.Contains(wrongUrl)) ? fromPlaceholder : fromSource;

					source = image.Source.ToString();
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
