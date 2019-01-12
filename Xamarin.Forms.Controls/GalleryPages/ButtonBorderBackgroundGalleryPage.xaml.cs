using System;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ButtonBorderBackgroundGalleryPage : ContentPage
	{
		public ButtonBorderBackgroundGalleryPage()
			: this(VisualMarker.MatchParent)
		{
		}

		public ButtonBorderBackgroundGalleryPage(IVisual visual)
		{
			InitializeComponent();
			Visual = visual;

			// buttons are transparent on default iOS, so we have to give them something
			if (Device.RuntimePlatform == Device.iOS)
			{
				if (Visual != VisualMarker.Material)
				{
					SetBackground(Content);

					void SetBackground(View view)
					{
						if (view is Button button && !button.IsSet(Button.BackgroundColorProperty))
							view.BackgroundColor = Color.LightGray;

						if (view is Layout layout)
						{
							foreach (var child in layout.Children)
							{
								if (child is View childView)
									SetBackground(childView);
							}
						}
					}
				}
			}
		}

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			(sender as Button).BorderWidth = 15;
			(sender as Button).BorderColor = Color.Red;
			(sender as Button).On<Android>().SetBorderAdjustsPadding(true);
		}

		void HandleChecks_Clicked(object sender, System.EventArgs e)
		{
			var thisButton = sender as Button;
			var layout = thisButton.Parent as Layout;
			foreach (var child in layout.Children)
			{
				var button = child as Button;

				Console.WriteLine($"{button.Text} => {button.Bounds}");
			}
		}
	}
}
