﻿namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class PropagationGallery : ContentPage
	{
		public PropagationGallery()
		{
			var descriptionLabel =
				new Label { Text = "Property Propagation Galleries", Margin = new Thickness(2, 2, 2, 2) };

			Title = "Property Propagation Galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryBuilder.NavButton("Propagate FlowDirection", () =>
							new PropagateCodeGallery(ListItemsLayout.VerticalList), Navigation),
					}
				}
			};
		}
	}
}