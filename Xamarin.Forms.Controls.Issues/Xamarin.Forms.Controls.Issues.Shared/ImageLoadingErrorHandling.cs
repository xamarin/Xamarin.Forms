using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.None, 0, "Image Loading Error Handling")]
	public class ImageLoadingErrorHandling : TestContentPage 
	{
		static Grid CreateTest(Action imageLoadAction, string title, string instructions, Color? backgroundColor = null)
		{
			var button = new Button { Text = "Test" };

			button.Clicked += (sender, args) => imageLoadAction();

			var titleLabel = new Label
			{
				Text = title,
				FontAttributes = FontAttributes.Bold
			};

			var label = new Label
			{
				Text = instructions
			};

			var grid = new Grid
			{
				ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(), new ColumnDefinition(), new ColumnDefinition() },
				RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = 80 } }
			};

			if (backgroundColor.HasValue)
			{
				grid.BackgroundColor = backgroundColor.Value;
			}

			grid.AddChild(titleLabel, 0, 0);
			grid.AddChild(label, 1, 0);
			grid.AddChild(button, 2, 0);

			return grid;
		}

		protected override void Init ()
		{
			Log.Listeners.Add(new DelegateLogListener((c, m) => Device.BeginInvokeOnMainThread(() => DisplayAlert (c, m, "Cool, Thanks"))));

			var image = new Image();

			var legit = CreateTest(() => image.Source = ImageSource.FromFile("coffee.png"),
				"Valid Image",
				"Clicking this button should load an image at the top of the page.",
				Color.Silver);

			var invalidImageFileName = CreateTest(() => image.Source = ImageSource.FromFile("fake.png"),
				"Non-existent Image File",
				"Clicking this button should display an alert dialog with an error that the image could not be loaded.");
			
			var invalidImageFile = CreateTest(() => image.Source = ImageSource.FromFile("invalidimage.jpg"),
				"Invalid Image File (bad data)",
				"Clicking this button should display an alert dialog with an error that the image could not be loaded.",
				Color.Silver);

			var fakeUri = CreateTest(() => image.Source = ImageSource.FromUri(new Uri("http://not.real")),
				"Non-existent URI",
				"Clicking this button should display an alert dialog. The error message should include the text 'the server name or address could not be resolved'.");
			
			var crashImage  = CreateTest(() => image.Source = new FailImageSource(),
				"Source Throws Exception",
				"Clicking this button should display an alert dialog with an error that the image could not be loaded.",
				Color.Silver);

			var uriInvalidImageData  = CreateTest(() => image.Source = ImageSource.FromUri(new Uri("http://192.168.1.10:8543/fail")),
				"Valid URI with invalid image file",
				"Clicking this button should display an alert dialog with an error that the image could not be loaded.");

			Content = new StackLayout
			{
				Children = { image,
					legit,
					invalidImageFileName,
					invalidImageFile,
					fakeUri,
					crashImage,
					uriInvalidImageData }
			};
		}
	}
}