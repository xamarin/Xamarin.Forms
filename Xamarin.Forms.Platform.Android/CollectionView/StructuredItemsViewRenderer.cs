using Android.Content;

namespace Xamarin.Forms.Platform.Android
{
	public class StructuredItemsViewRenderer<TItemsView, TAdapter, TItemsViewSource> : ItemsViewRenderer<TItemsView, TAdapter, TItemsViewSource>
		where TItemsView : StructuredItemsView
		where TAdapter : StructuredItemsViewAdapter<TItemsView, TItemsViewSource>
		where TItemsViewSource : IItemsViewSource
	{
		public StructuredItemsViewRenderer(Context context) : base(context)
		{
		}

		protected override TAdapter CreateAdapter()
		{
			return (TAdapter)new StructuredItemsViewAdapter<TItemsView, TItemsViewSource>(ItemsView);
		}
	}
}