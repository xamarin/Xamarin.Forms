namespace Xamarin.Forms.Controls.GalleryPages.VisualStateManagerGalleries
{
	public class StateTriggerGallery : ContentPage
	{
		public StateTriggerGallery()
		{
			Title = "StateTrigger Gallery";

			Content = new StackLayout
			{
				Children =
				{
					GalleryBuilder.NavButton("MinWindowWidth AdaptiveTrigger Gallery", () => new MinWindowWidthAdaptiveTriggerGallery(), Navigation),
					GalleryBuilder.NavButton("MinWindowHeight AdaptiveTrigger Gallery", () => new MinWindowHeightAdaptiveTriggerGallery(), Navigation),
					GalleryBuilder.NavButton("CompareStateTrigger Gallery", () => new CompareStateTriggerGallery(), Navigation),
					GalleryBuilder.NavButton("DeviceStateTrigger Gallery", () => new DeviceStateTriggerGallery(), Navigation),
					GalleryBuilder.NavButton("OrientationStateTrigger Gallery", () => new OrientationStateTriggerGallery(), Navigation),
					GalleryBuilder.NavButton("StateTriggers directly on Elements", () => new StateTriggersDirectlyOnElements(), Navigation)
				}
			};
		}
	}
}