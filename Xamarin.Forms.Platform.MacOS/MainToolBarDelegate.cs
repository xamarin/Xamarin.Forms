using System;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class MainToolBarDelegate : NSToolbarDelegate
	{

		Func<string> _getPageTitle;
		Func<string> _getPreviousPageTitle;
		Action _goBackAction;

		public MainToolBarDelegate(Func<string> getPageTitle, Func<string> getPreviousPageTitle, Action goBackAction)
		{
			_getPageTitle = getPageTitle;
			_getPreviousPageTitle = getPreviousPageTitle;
			_goBackAction = goBackAction;
		}
		public static string TitleIdentifier = "Title";
		public static string BackButtonIdentifier = "BackButton";

		public override string[] SelectableItemIdentifiers(NSToolbar toolbar)
		{
			return new string[] { };
		}
		public override string[] AllowedItemIdentifiers(NSToolbar toolbar)
		{
			return new string[] { NSToolbar.NSToolbarCustomizeToolbarItemIdentifier, NSToolbar.NSToolbarSpaceItemIdentifier };
		}
		public override string[] DefaultItemIdentifiers(NSToolbar toolbar)
		{
			return new string[] { };
		}
		public override NSToolbarItem WillInsertItem(NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
		{
			var tb = new NSToolbarItem(itemIdentifier);
			if (itemIdentifier == TitleIdentifier)
			{
				tb.Label = _getPageTitle();
			}
			if (itemIdentifier == BackButtonIdentifier)
			{
				var title = _getPreviousPageTitle();
				if (!string.IsNullOrEmpty(title))
				{
					tb.Image = NSImage.ImageNamed(NSImageName.GoLeftTemplate);
					tb.Label = title;
					tb.Activated += (sender, e) => _goBackAction();
				}
			}

			return tb;
		}
	}
}
