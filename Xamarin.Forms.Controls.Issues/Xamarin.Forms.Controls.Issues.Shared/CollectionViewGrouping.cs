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

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 4539134, "CollectionView: Grouping", PlatformAffected.All)]
	public class CollectionViewGrouping : TestNavigationPage
	{
		protected override void Init()
		{
#if APP
			Device.SetFlags(new List<string>(Device.Flags ?? new List<string>()) { "CollectionView_Experimental" });

			PushAsync(new GalleryPages.CollectionViewGalleries.GroupingGalleries.ObservableGrouping());
#endif
		}

#if UITEST && __IOS__ // Grouping is not implemented on Android yet
		[Test]
		public void RemoveSelectedItem()
		{
			RunningApp.WaitForElement("Hawkeye");
			RunningApp.Tap("Hawkeye");	

			RunningApp.Tap("Remove Selected");
			
			RunningApp.WaitForNoElement("Hawkeye");
		}
#endif
	}
}