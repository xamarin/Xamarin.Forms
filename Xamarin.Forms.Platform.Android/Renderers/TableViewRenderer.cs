using Android.Views;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using AListView = Android.Widget.ListView;

namespace Xamarin.Forms.Platform.Android
{
	public class TableViewRenderer : ViewRenderer<TableView, AListView>
	{
		public TableViewRenderer()
		{
			AutoPackage = false;
		}

		protected virtual TableViewModelRenderer GetModelRenderer(AListView listView, TableView view)
		{
			return new TableViewModelRenderer(Context, listView, view)
			{
				SectionHeaderDividerBackgroundColor = Element.On<PlatformConfiguration.Android>().SectionHeaderDividerBackgroundColor(),
				SectionDividerBackgroundColor = Element.On<PlatformConfiguration.Android>().SectionDividerBackgroundColor(),
				DividerBackgroundColor = Element.On<PlatformConfiguration.Android>().DividerBackgroundColor(),
				DividerHeight = Element.On<PlatformConfiguration.Android>().DividerHeight()
			};
		}

		protected override Size MinimumSize()
		{
			return new Size(40, 40);
		}

		protected override AListView CreateNativeControl()
		{
			return new AListView(Context);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
		{
			base.OnElementChanged(e);

			AListView listView = Control;
			if (listView == null)
			{
				listView = CreateNativeControl();
				SetNativeControl(listView);
			}

			listView.Focusable = false;
			listView.DescendantFocusability = DescendantFocusability.AfterDescendants;

			TableView view = e.NewElement;

			TableViewModelRenderer source = GetModelRenderer(listView, view);
			listView.Adapter = source;
		}
	}
}