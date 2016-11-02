using Android.Views;
using AListView = Android.Widget.ListView;

namespace Xamarin.Forms.Platform.Android
{
	public class TableViewRenderer : ViewRenderer<TableView, AListView>
	{
		public Color? SectionHeaderDividerBackgroundColor { get; set; }
		public Color? SectionDividerBackgroundColor { get; set; }

		public TableViewRenderer()
		{
			AutoPackage = false;
		}

		protected virtual TableViewModelRenderer GetModelRenderer(AListView listView, TableView view)
		{
			return new TableViewModelRenderer(Context, listView, view)
			{
				SectionHeaderDividerBackgroundColor = SectionHeaderDividerBackgroundColor,
				SectionDividerBackgroundColor = SectionDividerBackgroundColor,
				Divider = Control.Divider,
				DividerHeight = Control.DividerHeight
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