using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Controls.GalleryPages.VisualStateManagerGalleries
{
	public class VisualStateManagerGallery : ContentPage
	{
		public VisualStateManagerGallery()
		{
			var entryDisabledStatesButton = new Button { Text = "Entry Disabled States" };
			
			entryDisabledStatesButton.Clicked += (sender, args) => { Navigation.PushAsync (new EntryDisabledStatesGallery()); };
			
			Content = new StackLayout
			{
				Children = { entryDisabledStatesButton }
			};
		}
	}
}
