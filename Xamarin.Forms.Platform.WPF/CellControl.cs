using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xamarin.Forms.Internals;
using WSize = System.Windows.Size;

namespace Xamarin.Forms.Platform.WPF
{
	public class CellControl : ContentControl
	{
		public static readonly DependencyProperty CellProperty = DependencyProperty.Register("Cell", typeof(object), typeof(CellControl),
			new PropertyMetadata((o, e) => ((CellControl)o).SetSource(e.OldValue, e.NewValue)));

		public static readonly DependencyProperty ShowContextActionsProperty = DependencyProperty.Register("ShowContextActions", typeof(bool), typeof(CellControl), new PropertyMetadata(true));

		readonly PropertyChangedEventHandler _propertyChangedHandler;

		public CellControl()
		{
			Unloaded += (sender, args) =>
			{
				ICellController cell = DataContext as ICellController;
				if (cell != null)
					cell.SendDisappearing();
			};

			_propertyChangedHandler = OnCellPropertyChanged;
		}

		public Cell Cell
		{
			get { return (Cell)GetValue(CellProperty); }
			set { SetValue(CellProperty, value); }
		}

		public bool ShowContextActions
		{
			get { return (bool)GetValue(ShowContextActionsProperty); }
			set { SetValue(ShowContextActionsProperty, value); }
		}

		System.Windows.DataTemplate GetTemplate(Cell cell)
		{
			var renderer = Registrar.Registered.GetHandlerForObject<ICellRenderer>(cell);
			return renderer.GetTemplate(cell);
		}

		void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "HasContextActions")
				SetupContextMenu();
		}

		void SetSource(object oldCellObj, object newCellObj)
		{

			var oldCell = oldCellObj as Cell;
			var newCell = newCellObj as Cell;

			if (oldCell != null)
			{
				oldCell.PropertyChanged -= _propertyChangedHandler;
				((ICellController)oldCell).SendDisappearing();
				if (oldCell.Parent is ListView listView)
				{
					listView.ItemAppearing -= CellControl_ItemAppearing;
				}
			}

			if (newCell != null)
			{
				((ICellController)newCell).SendAppearing();

				if (oldCell == null || oldCell.GetType() != newCell.GetType())
					ContentTemplate = GetTemplate(newCell);

				Content = newCell;

				SetupContextMenu();

				newCell.PropertyChanged += _propertyChangedHandler;
				if (newCell.Parent is ListView listView)
				{
					listView.ItemAppearing += CellControl_ItemAppearing;
				}
			}
			else
				Content = null;
		}

		void CellControl_ItemAppearing(object sender, ItemVisibilityEventArgs e)
		{
			var width = (Cell.Parent as ListView)?.Width ?? 0;
			var height = Cell.RenderHeight;
			if (width > 0 && height > 0)
			{
				CellLayoutContent(new WSize(width, height));
			}
		}

		protected override WSize ArrangeOverride(WSize arrangeBounds)
		{
			CellLayoutContent(arrangeBounds);
			return base.ArrangeOverride(arrangeBounds);
		}

		void CellLayoutContent(WSize size)
		{
			if (double.IsInfinity(size.Width) || double.IsInfinity(size.Height) || size.Width <= 0 || size.Height <= 0)
				return;

			if (Content is ViewCell vc)
			{
				if (vc.LogicalChildren != null && vc.LogicalChildren.Any())
				{
					foreach (var child in vc.LogicalChildren)
					{
						if (child is Layout layout)
						{
							layout.Layout(new Rectangle(layout.X, layout.Y, size.Width, size.Height));
						}
					}
				}
			}
		}

		void SetupContextMenu()
		{
			if (Content == null || !ShowContextActions)
				return;

			if (!Cell.HasContextActions)
			{
				ContextMenuService.SetContextMenu(this, null);
				return;
			}

			ApplyTemplate();

			ContextMenu menu = new CustomContextMenu();
			menu.SetBinding(ItemsControl.ItemsSourceProperty, new System.Windows.Data.Binding("ContextActions"));

			ContextMenuService.SetContextMenu(this, menu);
		}
	}
}