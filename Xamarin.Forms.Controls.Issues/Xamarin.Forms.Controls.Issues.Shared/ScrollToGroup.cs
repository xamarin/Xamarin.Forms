using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	public class ScrollToGroup : TestNavigationPage
	{
		protected override void Init()
		{
#if APP
			FlagTestHelpers.SetCollectionViewTestFlag();
			PushAsync(new GalleryPages.CollectionViewGalleries.ScrollToGalleries.ScrollToGroup());
#endif
		}

#if UITEST && __IOS__ // Grouping for Android hasn't been merged yet
		[Test]
		public void ShouldBeAbleToScrollToGroupAndItemIndex()
		{
			RunningApp.WaitForElement("GroupIndexEntry");
			RunningApp.Tap("GroupIndexEntry");
			RunningApp.ClearText();
			RunningApp.EnterText("5");

			RunningApp.Tap("ItemIndexEntry");
			RunningApp.ClearText();
			RunningApp.EnterText("1");

			// Should scroll enough to display this item
			RunningApp.WaitForElement("Squirrel Girl");
		}

		[Test]
		public void InvalidScrollToIndexShouldNotCrash()
		{
			RunningApp.WaitForElement("GroupIndexEntry");
			RunningApp.Tap("GroupIndexEntry");
			RunningApp.ClearText();
			RunningApp.EnterText("55");

			RunningApp.Tap("ItemIndexEntry");
			RunningApp.ClearText();
			RunningApp.EnterText("1");

			// Should scroll enough to display this item
			RunningApp.WaitForElement("Avengers");
		}
#endif
	}
}
