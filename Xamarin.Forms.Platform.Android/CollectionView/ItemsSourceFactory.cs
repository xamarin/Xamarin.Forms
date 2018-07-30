using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Android.Support.V7.Widget;

namespace Xamarin.Forms.Platform.Android
{
	internal static class ItemsSourceFactory
	{
		public static ICollectionViewSource Create(IEnumerable itemSource, RecyclerView.Adapter adapter)
		{
			switch (itemSource)
			{
				case IList _ when itemSource is INotifyCollectionChanged:
					return new ObservableCollectionSource(itemSource, adapter);
				case IEnumerable<object> generic:
					return new ListSource(generic);
			}

			return new ListSource(itemSource);
		}
	}
}