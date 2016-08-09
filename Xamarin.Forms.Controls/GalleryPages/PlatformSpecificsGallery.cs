using Xamarin.Forms.Controls.GalleryPages.PlatformSpecificsGalleries;

namespace Xamarin.Forms.Controls
{
	public class PlatformSpecificsGallery : ContentPage
	{
		Page _originalRoot;

		public PlatformSpecificsGallery()
		{
			var mdpWindowsButton = new Button { Text = "Master Detail Page (Windows)" };
			var npWindowsButton = new Button { Text = "Navigation Page (Windows)" };
			var navpageiOSButton = new Button() { Text = "Navigation Page (iOS)"};

			mdpWindowsButton.Clicked += (sender, args) => { SetRoot(new MasterDetailPageWindows(new Command(RestoreOriginal))); };
			npWindowsButton.Clicked += (sender, args) => { SetRoot(new NavigationPageWindows(new Command(RestoreOriginal))); };
			navpageiOSButton.Clicked += (sender, args) => { SetRoot(NavigationPageiOS.Create(new Command(RestoreOriginal))); };

			Content = new StackLayout
			{
				Children = { mdpWindowsButton, npWindowsButton, navpageiOSButton }
			};
		}

		void SetRoot(Page page)
		{
			var app = Application.Current as App;
			if (app == null)
			{
				return;
			}

			_originalRoot = app.MainPage;
			app.SetMainPage (page);
		}

		void RestoreOriginal()
		{
			if (_originalRoot == null)
			{
				return;
			}

			var app = Application.Current as App;
			app?.SetMainPage (_originalRoot);
		}
	}
}