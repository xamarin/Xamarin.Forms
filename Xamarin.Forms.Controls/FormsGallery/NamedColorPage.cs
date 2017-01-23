using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsGallery
{
	// Used in:
	//      MasterDetailPageDemoPage (as a page)
	//      TabbedPageDemoPage (as a page template)
	//      CarouselPageDemoPage (as a page template)
	//
	//  Expects BindingContext to be of type NamedColor!
	class NamedColorPage : ContentPage
	{
		public NamedColorPage (bool includeBigLabel)
		{
			// This binding is necessary to label the tabs in 
			//      the TabbedPage.
			this.SetBinding (ContentPage.TitleProperty, "Name");

			// BoxView to show the color.
			BoxView boxView = new BoxView {
				WidthRequest = 100,
				HeightRequest = 100,
				HorizontalOptions = LayoutOptions.Center
			};
			boxView.SetBinding (BoxView.ColorProperty, "Color");

			// Function to create six Labels.
			Func<string, string, Label> CreateLabel = (string source, string fmt) => {
				Label label = new Label {
					FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
					HorizontalTextAlignment = TextAlignment.End
				};
				label.SetBinding (Label.TextProperty,
					new Binding (source, BindingMode.OneWay, null, null, fmt));

				return label;
			};

			// Build the page
			this.Content = new StackLayout {
				Children = {
					new StackLayout {   
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.CenterAndExpand,
						Children = {
							CreateLabel ("Color.R", "R = {0:F2}"),
							CreateLabel ("Color.G", "G = {0:F2}"),
							CreateLabel ("Color.B", "B = {0:F2}"),
						}
					},
					boxView,
					new StackLayout {   
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.CenterAndExpand,
						Children = {
							CreateLabel ("Color.Hue", "Hue = {0:F2}"),
							CreateLabel ("Color.Saturation", "Saturation = {0:F2}"),
							CreateLabel ("Color.Luminosity", "Luminosity = {0:F2}")
						}
					}
				}
			};

			// Add in the big Label at top for CarouselPage.
			if (includeBigLabel) {
				Label bigLabel = new Label {
					FontSize = 50,
					HorizontalOptions = LayoutOptions.Center
				};
				bigLabel.SetBinding (Label.TextProperty, "Name");

				(this.Content as StackLayout).Children.Insert (0, bigLabel);
			}
		}
	}
}
