using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	public partial class ExampleTemplateCarousel : Grid
	{
		public ExampleTemplateCarousel()
		{
			InitializeComponent();
			this.PropertyChanged += Handle_PropertyChanged;
		}

		void Handle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//var vsmGroups = VisualStateManager.GetVisualStateGroups(this);
			//if (vsmGroups.Count > 0)
			//{
				//var carouselVSM = vsmGroups[0];
				//if (carouselVSM.CurrentState?.Name == "DefaultItem")
				//{
				//	this.ScaleTo(0.2);
				//}
				//if (carouselVSM.CurrentState?.Name == "CurrentItem")
				//{
				//	this.ScaleTo(1);
				//}
				//if (carouselVSM.CurrentState?.Name == "PreviousItem" ||
				//	carouselVSM.CurrentState?.Name == "NextItem")
				//{
				//	this.ScaleTo(0.5);
				//}
			//}
		}
	}
}
