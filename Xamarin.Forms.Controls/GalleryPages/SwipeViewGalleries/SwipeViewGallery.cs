﻿using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.SwipeViewGalleries
{
	[Preserve(AllMembers = true)]
	public class SwipeViewGallery : ContentPage
	{
		public SwipeViewGallery()
		{
			var button = new Button
			{
				Text = "Enable SwipeView",
				AutomationId = "EnableSwipeView"
			};
			button.Clicked += ButtonClicked;

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
	 					button,
						GalleryBuilder.NavButton("Basic SwipeView Gallery", () => new BasicSwipeGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeView Events Gallery", () => new SwipeViewEventsGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeItems from Resource Gallery", () => new ResourceSwipeItemsGallery(), Navigation),
						GalleryBuilder.NavButton("BindableLayout Gallery", () => new SwipeBindableLayoutGallery(), Navigation),
						GalleryBuilder.NavButton("ListView (RecycleElement) Gallery", () => new SwipeListViewGallery(), Navigation),
						GalleryBuilder.NavButton("CollectionView Galleries", () => new SwipeCollectionViewGallery(), Navigation),
						GalleryBuilder.NavButton("CollectionView using VisualStates Gallery", () => new SwipeViewVisualStatesCollectionGallery(), Navigation),
						GalleryBuilder.NavButton("CarouselView Gallery", () => new SwipeCarouselViewGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeView GestureRecognizer Gallery", () => new SwipeViewGestureRecognizerGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeBehaviorOnInvoked Gallery", () => new SwipeBehaviorOnInvokedGallery(), Navigation),
						GalleryBuilder.NavButton("Custom SwipeItem Galleries", () => new CustomSwipeItemGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeView BindingContext Gallery", () => new SwipeViewBindingContextGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeItem Icon Gallery", () => new SwipeItemIconGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeItem Size Gallery", () => new SwipeItemSizeGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeItem IsEnabled Gallery", () => new SwipeItemIsEnabledGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeItemView IsEnabled Gallery", () => new SwipeItemViewIsEnabledGallery(), Navigation),
 						GalleryBuilder.NavButton("SwipeItem IsVisible Gallery", () => new SwipeItemIsVisibleGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeTransitionMode Gallery", () => new SwipeTransitionModeGallery(), Navigation),
						GalleryBuilder.NavButton("Add/Remove SwipeItems Gallery", () => new AddRemoveSwipeItemsGallery(), Navigation),
						GalleryBuilder.NavButton("Open/Close SwipeView Gallery", () => new CloseSwipeGallery(), Navigation),
						GalleryBuilder.NavButton("SwipeItems Dispose Gallery", () => new SwipeItemsDisposeGallery(), Navigation)
					}
				}
			};
		}

		void ButtonClicked(object sender, System.EventArgs e)
		{
			var button = sender as Button;

			button.Text = "SwipeView Enabled!";
			button.TextColor = Color.Black;
			button.IsEnabled = false;

			Device.SetFlags(new[] { ExperimentalFlags.SwipeViewExperimental, ExperimentalFlags.CarouselViewExperimental });
		}
	}
}