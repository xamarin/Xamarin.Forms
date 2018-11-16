using System;

namespace Xamarin.Forms
{
	public class SelectedItemChangedEventArgs : EventArgs
	{
		[Obsolete("Please use the constructor that reports the items index")]
		public SelectedItemChangedEventArgs(object selectedItem)
			: this(selectedItem, -1)
		{

		}

		public SelectedItemChangedEventArgs(object selectedItem, int selectedItemIndex)
		{
			SelectedItem = selectedItem;
			SelectedItemIndex = selectedItemIndex;
		}

		public object SelectedItem { get; private set; }

		public int SelectedItemIndex { get; private set; }

	}
}