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
					GalleryBuilder.NavButton("AdaptiveTrigger Gallery", () => new AdaptiveTriggerGallery(), Navigation),
					GalleryBuilder.NavButton("DeviceStateTrigger Gallery", () => new DeviceStateTriggerGallery(), Navigation),
					GalleryBuilder.NavButton("OrientationStateTrigger Gallery", () => new OrientationStateTriggerGallery(), Navigation)
				}
			};
		}
	}
}