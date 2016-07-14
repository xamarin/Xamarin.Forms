using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.None, 0, "Windows Image Error Handling")]
	public class WindowsImageErrorHandling : TestContentPage 
	{
		protected override void Init ()
		{
			var legitImage = new Button() {Text = "Load Image File" };
			var invalidImageFileName = new Button() {Text = "Load Invalid Image File Name" };
			var invalidImageFile = new Button() {Text = "Load Invalid Image File" };
			var fakeUri = new Button() {Text = "Load Image With Fake URI" };
			var crashImage = new Button() {Text = "Image Handler Which Throws Exception" };

			var uriInvalidImageData = new Button() {Text = "URI returning invalid image data" };

			var image = new Image();

			legitImage.Clicked += (sender, args) =>
			{
				image.Source = ImageSource.FromFile("coffee.png");
			};

			invalidImageFileName.Clicked += (sender, args) =>
			{
				image.Source = ImageSource.FromFile("fake.png");
			};

			invalidImageFile.Clicked += (sender, args) =>
			{
				image.Source = ImageSource.FromFile("invalidimage.jpg");
			};

			fakeUri.Clicked += (sender, args) =>
			{
				image.Source = ImageSource.FromUri(new Uri("http://not.real"));
			};

			crashImage.Clicked += (sender, args) =>
			{
				image.Source = new FailImageSource();
			};

			uriInvalidImageData.Clicked += (sender, args) =>
			{
				image.Source = ImageSource.FromUri(new Uri("http://192.168.1.10:8543/fail"));
			};

			Content = new StackLayout()
			{
				Children = { image, legitImage, invalidImageFile, invalidImageFileName, fakeUri, crashImage, uriInvalidImageData }
			};
		}
	}
}