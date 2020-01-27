using System;
using System.Linq;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Controls.GalleryPages
{
	class ShowModalWithTransparentBkgndGalleryPage : ContentPage
	{
		readonly Picker _modalPresentationStylesPicker;
		readonly PageWithTransparentBkgnd _pageWithTransparentBkgnd;

		public ShowModalWithTransparentBkgndGalleryPage()
		{
			BackgroundColor = Color.LightPink;

			var layout = new StackLayout();

			_modalPresentationStylesPicker = new Picker();

			var modalPresentationStyles = Enum.GetNames(typeof(UIModalPresentationStyle)).Select(m => m).ToList();

			_modalPresentationStylesPicker.Title = "Select ModalPresentation Style";
			_modalPresentationStylesPicker.ItemsSource = modalPresentationStyles;
			_modalPresentationStylesPicker.SelectedIndex = 2;

			_modalPresentationStylesPicker.SelectedIndexChanged += (sender, args) =>
			{
				var selected = _modalPresentationStylesPicker.SelectedItem;

				switch (selected)
				{
					case "Automatic":
						_pageWithTransparentBkgnd.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.Automatic);
						break;
					case "FormSheet":
						_pageWithTransparentBkgnd.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
						break;
					case "FullScreen":
						_pageWithTransparentBkgnd.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FullScreen);
						break;
					case "OverFullScreen":
						_pageWithTransparentBkgnd.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);
						break;
					case "PageSheet":
						_pageWithTransparentBkgnd.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.PageSheet);
						break;
				}
			};

			if (Device.RuntimePlatform == Device.iOS)
			{
				layout.Children.Add(_modalPresentationStylesPicker);
			}

			var showTransparentModalPageButton = new Button()
			{
				Text = "Show Transparent Modal Page",
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Center
			};

			showTransparentModalPageButton.Clicked += ShowModalBtnClicked;

			layout.Children.Add(showTransparentModalPageButton);

			Content = layout;

			_pageWithTransparentBkgnd = new PageWithTransparentBkgnd();

			_pageWithTransparentBkgnd.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Console.WriteLine("OnAppearing");
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Console.WriteLine("OnDisappearing");
		}

		void ShowModalBtnClicked(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(_pageWithTransparentBkgnd);
		}
	}
}