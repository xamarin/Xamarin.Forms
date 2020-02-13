using System.Linq;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UITests
{
	[Category(UITestCategories.CarouselView)]
	internal class CarouselViewUITests : BaseTestFixture
	{
		string _carouselViewGalleries = "CarouselView Galleries";

		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.CollectionViewGallery);

			App.WaitForElement(_carouselViewGalleries);
			App.Tap(_carouselViewGalleries);
		}

		[TestCase("CarouselView (XAML, Horizontal)")]
		public void CarouselViewRemoveAndUpdateCurrentItem(string subgallery)
		{
			VisitSubGallery(subgallery);
			var position = App.WaitForElement(x => x.Marked("lblPosition")).First().Text;
			Assert.IsTrue(position == "0");
			var currentItem = App.WaitForElement(x => x.Marked("lblCurrentItem")).First().Text;
			Assert.IsTrue(currentItem == "0");
			var selected = App.WaitForElement(x => x.Marked("lblSelected")).First().Text;
			Assert.IsTrue(selected == "0");
			var rect = App.Query(c => c.Marked("TheCarouselView")).First().Rect;
			var centerX = rect.CenterX;
			var rightX = rect.X - 5;
			App.DragCoordinates(centerX + 40, rect.CenterY, rightX, rect.CenterY);
			var positionAfter = App.WaitForElement(x => x.Marked("lblPosition")).First().Text;
			Assert.IsTrue(positionAfter == "1");
			var currentItemAfter = App.WaitForElement(x => x.Marked("lblCurrentItem")).First().Text;
			Assert.IsTrue(currentItemAfter == "1");
			var selectedAfter = App.WaitForElement(x => x.Marked("lblSelected")).First().Text;
			Assert.IsTrue(selectedAfter == "1");
			App.Tap(x => x.Marked("btnRemove"));
			var positionAfterRemove = App.WaitForElement(x => x.Marked("lblPosition")).First().Text;
			Assert.IsTrue(positionAfterRemove == "1");
			var currentItemAfterRemove = App.WaitForElement(x => x.Marked("lblCurrentItem")).First().Text;
			Assert.IsTrue(currentItemAfterRemove == "2");
			var selectedAfterRemove = App.WaitForElement(x => x.Marked("lblSelected")).First().Text;
			Assert.IsTrue(selectedAfterRemove == "2");

			App.Back();
		}

		[TestCase("CarouselView (Code, Horizontal)")]
		//[TestCase("CarouselView (XAML, Horizontal)")]
		public void CarouselViewHorizontal(string subgallery)
		{
			VisitSubGallery(subgallery);


			App.WaitForElement("pos:1", "Did start on the correct position");
			var rect = App.Query(c => c.Marked("TheCarouselView")).First().Rect;
			var centerX = rect.CenterX;
			var rightX = rect.X + rect.Width - 1;
			App.DragCoordinates(centerX, rect.CenterY, rightX, rect.CenterY);
			App.WaitForElement("pos:0", "Did not scroll to first position");
			App.DragCoordinates(centerX, rect.CenterY, rect.X + 5, rect.CenterY);
			App.WaitForElement("pos:1", "Did not scroll to second position");

			App.Tap("Item: 1");

			App.WaitForElement("Button works");

			App.Tap(c => c.Marked("Ok"));

			App.Tap("SwipeSwitch");

			// iOS will show the Master page when we try drag
#if __ANDROID__
			App.DragCoordinates(centerX, rect.CenterY, rightX, rect.CenterY);

			App.WaitForNoElement("pos:0", "Swiped while swipe is disabled");
#endif
			App.Back();
		}

#if __IOS__
		[TestCase("CarouselView (Code, Vertical)")]
#endif
		public void CarouselViewVertical(string subgallery)
		{
			VisitSubGallery(subgallery);
			var rect = App.Query(c => c.Marked("TheCarouselView")).First().Rect;

			var centerX = rect.CenterX;
			var centerY = rect.CenterY;
			var bottomY = rect.Y + rect.Height - 1;

			App.DragCoordinates(rect.CenterX, rect.CenterY, rect.CenterX, bottomY);

			App.WaitForElement("pos:0", "Did not scroll to first position");

			App.DragCoordinates(rect.CenterX, rect.CenterY, rect.CenterX, rect.Y - 1);

			App.WaitForElement("pos:1", "Did not scroll to second position");

			App.Tap("Item: 1");

			App.WaitForElement("Button works");

			App.Tap(c => c.Marked("Ok"));

			App.Tap("SwipeSwitch");

#if __ANDROID__
			App.DragCoordinates(rect.CenterX, rect.CenterY, rect.CenterY, rect.Y + rect.Height - 1);

			App.WaitForNoElement("pos:0", "Swiped while swipe is disabled");
#endif
			App.Back();
		}

		void VisitSubGallery(string galleryName)
		{
			App.WaitForElement(t => t.Marked(galleryName));
			App.Tap(t => t.Marked(galleryName));
		}
	}
}