using Android.Content;

namespace Xamarin.Forms.Platform.Android
{
	public class CollectionViewRenderer : GroupableItemsViewRenderer<GroupableItemsView, GroupableItemsViewAdapter<GroupableItemsView, IGroupedItemsViewSource>, IGroupedItemsViewSource>
	{
		public CollectionViewRenderer(Context context) : base(context)
		{
		}
	}
}