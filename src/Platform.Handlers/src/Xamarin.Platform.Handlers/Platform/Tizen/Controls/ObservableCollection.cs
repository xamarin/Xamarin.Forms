using System.Collections.Specialized;
using System.Linq;

namespace Xamarin.Platform.Tizen
{
	public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
	{
		protected override void ClearItems()
		{
			var oldItems = Items.ToList();
			Items.Clear();
			using (BlockReentrancy())
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
			}
			base.ClearItems();
		}
	}
}
