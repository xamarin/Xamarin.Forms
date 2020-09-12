using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7015, "Image placeholder", PlatformAffected.All)]
	public class Issue7015 : TestContentPage
	{
		Image image;
		CheckBox checkBox;
		ImageButton imageButton;

		string url = "https://cdn.wallpaperhub.app/cloudcache/0/1/7/7/1/4/0177141d23e1d77489fa02a20f66302fe01033fd.jpg";
		string wrongUrl = "https://cdn.wallpaperhub.app/cloudcache/0/1/7/7/1/4/0177141d23e1d77489fa02a20f66302fe010fd.jpg";

		string localImage = "bell.png";
		string errorLocalImage = "games.png";
		string loadingImage = "bank.png";

		protected override void Init()
		{
			Padding = new Thickness(10);
			image = new Image
			{
				Source = localImage,
				LoadingSource = loadingImage,
				ErrorSource = errorLocalImage
			};

			imageButton = new ImageButton
			{
				Source = localImage,
				LoadingSource = loadingImage,
				ErrorSource = errorLocalImage
			};

			checkBox = new CheckBox();

			var button = new Button
			{
				Text = "Change image",
				VerticalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = Color.Fuchsia
			};

			button.Clicked += async (_, __) =>
			{
				var s = image.Source.ToString();
				imageButton.Source = s.Contains(localImage)
				? GetURL()
				: localImage;

				await Task.Delay(50);

				image.Source = s.Contains(localImage)
				? GetURL()
				: localImage;
			};

			var layout = new StackLayout
			{
				HeightRequest = 300,
				WidthRequest = 300,
				BackgroundColor = Color.Azure
			};

			var description = new Label
			{
				Text = "When you press the button, the image source will change to an URL, if the checkbox is marked you will see the image otherwise you will see the joystick",
				TextColor = Color.Black
			};

			layout.Children.Add(image);
			layout.Children.Add(imageButton);
			layout.Children.Add(button);
			layout.Children.Add(description);
			layout.Children.Add(checkBox);

			Content = layout;


			string GetURL()
			{
				return checkBox.IsChecked ? url : wrongUrl;
			}
		}
	}
}
