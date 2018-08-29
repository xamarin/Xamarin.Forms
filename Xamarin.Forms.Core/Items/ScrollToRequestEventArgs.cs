using System;

namespace Xamarin.Forms
{
	public class ScrollToRequestEventArgs : EventArgs
	{
		public ScrollToMode Mode { get; }

		public ScrollToPosition ScrollToPosition { get; }
		public bool Animate { get; }

		public int Index { get; }
		public int GroupIndex { get; }

		public object Item { get; }
		public object Group { get; }

		public ScrollToRequestEventArgs(int index, int groupIndex, 
			ScrollToPosition scrollToPosition, bool animate)
		{
			Mode = ScrollToMode.Position;

			Index = index;
			GroupIndex = groupIndex;
			ScrollToPosition = scrollToPosition;
			Animate = animate;
		}

		public ScrollToRequestEventArgs(object item, object group, 
			ScrollToPosition scrollToPosition, bool animate)
		{
			Mode = ScrollToMode.Element;

			Item = item;
			Group = group;
			ScrollToPosition = scrollToPosition;
			Animate = animate;
		}
	}
}