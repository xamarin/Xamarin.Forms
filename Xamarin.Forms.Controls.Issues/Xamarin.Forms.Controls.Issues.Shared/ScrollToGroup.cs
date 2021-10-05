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

#if __WINDOWS__
[NUnit.Framework.Category(UITestCategories.UwpIgnore)] // Can't accurately check this test 
#endif
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 11111111, "CollectionView Scroll To Grouped Item",
		PlatformAffected.All)]
	public class ScrollToGroup : TestNavigationPage
	{
		protected override void Init()
		{
#if APP
			PushAsync(new GalleryPages.CollectionViewGalleries.ScrollToGalleries.ScrollToGroup());
#endif
		}

#if UITEST
		[Test]
		public void CanScrollToGroupAndItemIndex()
		{
			RunningApp.WaitForElement("GroupIndexEntry");
			RunningApp.Tap("GroupIndexEntry");
			RunningApp.ClearText();
			RunningApp.EnterText("5");

			RunningApp.Tap("ItemIndexEntry");
			RunningApp.ClearText();
			RunningApp.EnterText("1");

			RunningApp.Tap("GoButton");

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

			RunningApp.Tap("GoButton");

			// Should scroll enough to display this item
			RunningApp.WaitForElement("Avengers");
		}

		[Test]
		public void CanScrollToGroupAndItem()
		{
			RunningApp.WaitForElement("GroupNameEntry");
			RunningApp.Tap("GroupNameEntry");
			RunningApp.ClearText();
			RunningApp.EnterText("Heroes for Hire");

			RunningApp.Tap("ItemNameEntry");
			RunningApp.ClearText();
			RunningApp.EnterText("Misty Knight");

			RunningApp.Tap("GoItemButton");

			// Should scroll enough to display this item
			RunningApp.WaitForElement("Luke Cage");
		}
#endif
	}
}
