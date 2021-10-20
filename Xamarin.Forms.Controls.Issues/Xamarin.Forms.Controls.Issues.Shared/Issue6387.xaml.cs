using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ToolbarItem)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6387,
		"[Bug] [iOS] Crash When the First Toolbar Item Has an Icon and a Second Item Gets Added",
		PlatformAffected.iOS)]
	public partial class Issue6387 : TestContentPage
	{
#if APP
		readonly ToolbarItem _item0;
		readonly ToolbarItem _item1;
#endif
		public Issue6387()
		{
#if APP
			InitializeComponent();

			_item0 = new ToolbarItem("Item 0", null, ClearAndAddToolbarItems)
			{
				IconImageSource = ImageSource.FromResource("Xamarin.Forms.Controls.GalleryPages.crimson.jpg", typeof(Issue6387).GetTypeInfo().Assembly)
			};
			_item1 = new ToolbarItem("Item 1", null, ClearAndAddToolbarItems)
			{
				// It doesn't matter if this item has an image or not.
			};

			ClearAndAddToolbarItems();
#endif
		}

		protected override void Init()
		{

		}
#if APP
		void ClearAndAddToolbarItems()
		{
			ToolbarItems.Clear();

			ToolbarItems.Add(_item0);
			ToolbarItems.Add(_item1);
		}
#endif
	}
}