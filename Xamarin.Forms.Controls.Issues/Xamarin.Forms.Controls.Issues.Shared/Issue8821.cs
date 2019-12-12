using System;
using System.IO;
using System.Net;
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
	[Category(UITestCategories.Image)]
#endif
    [Preserve(AllMembers = true)]
    [Issue(IssueTracker.Github, 8821, "Animations of downloaded gifs are not playing on Android", PlatformAffected.Android)]
    public class Issue8821 : TestContentPage
    {
        private WebClient _webClientBlobDownload = null;
        private readonly Button _downloadButton;
        private readonly Image _image;

        public Issue8821()
        {
			var instructions = new Label
			{
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Press the DownloadFile button and then the Animate button. Verify that the gif is downloaded and animate without problems."
            };

			_downloadButton = new Button { Text = "DownloadFile" };
            _downloadButton.Clicked += DownloadFile;
			var animateButton = new Button { Text = "Animate" };
            animateButton.Clicked += Animate;
            _image = new Image { Source = string.Empty };

            Content = new StackLayout
            {
                Padding = new Thickness(20, 35, 20, 20),
                Children =
                {
                    instructions,
                    _downloadButton,
                    _image,
                    animateButton
                }
            };
        }

        public string SecondImageSource { get; set; }

        protected override void Init()
        {
            Title = "Issue 8821";
        }

        void Animate(object sender, EventArgs e)
        {
            _image.IsAnimationPlaying = true;
        }

        void DownloadFile(object sender, EventArgs e)
        {
            if (_webClientBlobDownload == null)
            {
                _webClientBlobDownload = new WebClient();
                _webClientBlobDownload.DownloadDataCompleted += new DownloadDataCompletedEventHandler(OnImageDownloadDataCompleted);
            }

            string nextURL = "https://upload.wikimedia.org/wikipedia/commons/c/c0/An_example_animation_made_with_Pivot.gif";

            _webClientBlobDownload.DownloadDataAsync(new Uri(nextURL), SecondImageSource);
        }

        void OnImageDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled == true)
                    return;

                if (e.Error != null)
                    return;
                
                byte[] bytes = new byte[e.Result.Length]; 
                bytes = e.Result;
                SecondImageSource = Path.Combine(GetCurrentImagePath(), "Dmg.gif");
                File.WriteAllBytes(SecondImageSource, bytes);
            }
            catch 
            {
                return;
            }

            _image.Source = SecondImageSource;
            OnPropertyChanged(nameof(SecondImageSource));
        }

        string GetCurrentImagePath()
        {
			var imagePath = "imagePath";
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/" + imagePath;

            if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return path;
        }
	}
}