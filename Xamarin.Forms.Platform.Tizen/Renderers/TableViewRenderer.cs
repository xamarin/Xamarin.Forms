using System;
using ElmSharp;

namespace Xamarin.Forms.Platform.Tizen
{
	public class TableViewRenderer : ViewRenderer<TableView, Native.TableView>, IVisualElementRenderer
	{
		internal static BindableProperty PresentationProperty = BindableProperty.Create("Presentation", typeof(View), typeof(TableSectionBase), null, BindingMode.OneWay, null, null, null, null, null as BindableProperty.CreateDefaultValueDelegate);

		public TableViewRenderer()
		{
			RegisterPropertyHandler(TableView.HasUnevenRowsProperty, UpdateHasUnevenRows);
			RegisterPropertyHandler(TableView.RowHeightProperty, UpdateRowHeight);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
		{
			if (Control == null)
			{
				var _tableView = new Native.TableView(Forms.Context.MainWindow);
				SetNativeControl(_tableView);
			}

			if (e.OldElement != null)
			{
				Control.ItemSelected -= ListViewItemSelectedHandler;
				e.OldElement.ModelChanged -= OnRootPropertyChanged;
			}

			if (e.NewElement != null)
			{
				e.NewElement.ModelChanged += OnRootPropertyChanged;
				Control.ItemSelected += ListViewItemSelectedHandler;
				Control.ApplyTableRoot(e.NewElement.Root);
			}

			base.OnElementChanged(e);
		}

		protected override void Dispose(bool disposing)
		{
			Element.ModelChanged -= OnRootPropertyChanged;
			base.Dispose(disposing);
		}

		void ListViewItemSelectedHandler(object sender, GenListItemEventArgs e)
		{
			var item = e.Item as GenListItem;

			if (item != null)
			{
				var clickedCell = item.Data as Native.ListView.ItemContext;
				if (null != clickedCell)
				{
					Element.Model.RowSelected(clickedCell.Cell);
				}
			}
		}

		void OnRootPropertyChanged(object sender, EventArgs e)
		{
			if (Element != null)
			{
				Control.ApplyTableRoot(Element.Root);
			}
		}

		void UpdateHasUnevenRows()
		{
			Control.SetHasUnevenRows(Element.HasUnevenRows);
		}

		void UpdateRowHeight()
		{
			Control.UpdateRealizedItems();
		}

	}
}
