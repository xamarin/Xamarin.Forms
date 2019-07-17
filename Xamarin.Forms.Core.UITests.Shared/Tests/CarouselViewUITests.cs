using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;

namespace Xamarin.Forms.Core.UITests
{
	[Category(UITestCategories.CarouselView)]
	internal class CarouselViewUITests : BaseTestFixture
	{
		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.CollectionViewGallery);

			App.WaitForElement("CarouselView Galleries");
			App.Tap("Enable CollectionView");
			App.Tap("CarouselView Galleries");
		}

		[Test]
		public void CarouselViewHorizontal()
		{
			App.Tap(c => c.Marked("CarouselView (Code, Horizontal)"));

			App.SwipeLeftToRight(c => c.Marked("TheCarouselView"));

			Assert.AreEqual(App.Query("CurrentPositionLabel").First().Text, "0", "Did not scroll to first position");

			App.SwipeRightToLeft(c => c.Marked("TheCarouselView"));

			Assert.AreEqual(App.Query("CurrentPositionLabel").First().Text, "1", "Did not scroll to second position");

			App.Tap("Item: 1");

#if __ANDROID__
			Assert.AreEqual(App.Query(c => c.Class("AlertDialogLayout")).Count(), 1, "Alert not shown");
#elif __iOS__
			App.Query(c => c.ClassFull("_UIAlertControllerView"));
#endif

#if __ANDROID__
			
			App.Tap(c => c.Marked("Ok"));
#elif __iOS__
			App.Tap(c => c.Marked("OK").Parent().ClassFull("_UIAlertControllerView"));
#endif

			App.Tap("SwipeSwitch");

			App.SwipeLeftToRight(c => c.Marked("TheCarouselView"));

			Assert.AreEqual(App.Query("CurrentPositionLabel").First().Text, "1", "Swiped while swipe is disabled");
		}
	}
}