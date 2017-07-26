using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UITests
{
#if __ANDROID__ || __WINDOWS__
	[Ignore("Test only meant for Mac and iOS, besides needing API keys for Android and Windows")]
#endif
	[TestFixture]
	[Category(UITestCategories.Maps)]
    internal class MapUITests : BaseTestFixture
    {
	    protected override void NavigateToGallery()
	    {
		    App.NavigateToGallery(GalleryQueries.MapGalleryLegacy);
	    }

		[Test]
		public void MapGalleryPinClicked()
		{
			App.Tap(PlatformQueries.Pin);
			App.Tap(c => c.Text("Colosseum"));
			App.Screenshot("Alert displayed as result of PinClicked event");
		}
    }
}
