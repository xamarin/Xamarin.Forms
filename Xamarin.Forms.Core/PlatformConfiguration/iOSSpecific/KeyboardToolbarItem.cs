using System;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	public class KeyboardToolbarItem
	{
		public UIBarButtonSystemItem UIBarButtonSystemItem { get; set; }
		public Action Action { get; set; }
		public Color TintColor { get; set; }

		public KeyboardToolbarItem(UIBarButtonSystemItem uiBarButtonSystemItem) : this(uiBarButtonSystemItem, null)
		{
		}

		public KeyboardToolbarItem(UIBarButtonSystemItem uiBarButtonSystemItem, Action action) : this(uiBarButtonSystemItem, action, Color.Accent)
		{
		}

		public KeyboardToolbarItem(UIBarButtonSystemItem uiBarButtonSystemItem, Action action, Color tintColor)
		{
			UIBarButtonSystemItem = uiBarButtonSystemItem;
			Action = action;
			TintColor = tintColor;
		}
	}
}