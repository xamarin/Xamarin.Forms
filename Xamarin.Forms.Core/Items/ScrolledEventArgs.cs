using System;

namespace Xamarin.Forms.Core.Items
{
	public class ScrolledEventArgs : EventArgs
	{
		public double HorizontalDelta { get; private set; }

		public double VerticalDelta { get; private set; }

		public double HorizontalOffset { get; private set; }

		public double VerticalOffset { get; private set; }

		public int FirstVisibleItemIndex { get; private set; }

		public int LastVisibleItemIndex { get; private set; }

		public ScrolledEventArgs(double horizontalDelta, double verticalDelta, double horizontalOffset, double verticalOffset, int firstVisibleItemIndex, int lastVisibleItemIndex)
		{
			HorizontalDelta = horizontalDelta;
			VerticalDelta = verticalDelta;
			HorizontalOffset = horizontalOffset;
			VerticalOffset = verticalOffset;
			FirstVisibleItemIndex = firstVisibleItemIndex;
			LastVisibleItemIndex = lastVisibleItemIndex;
		}
	}
}