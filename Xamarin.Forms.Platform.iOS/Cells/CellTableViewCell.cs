using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
#if __UNIFIED__
using UIKit;

#else
using MonoTouch.UIKit;
#endif

namespace Xamarin.Forms.Platform.iOS
{
	public class CellTableViewCell : UITableViewCell, INativeElementView
	{
		Cell _cell;

		public Action<object, PropertyChangedEventArgs> PropertyChanged;

		public CellTableViewCell(UITableViewCellStyle style, string key) : base(style, key)
		{
		}

		public Cell Cell
		{
			get { return _cell; }
			set
			{
				if (_cell == value)
					return;

				ICellController cellController = _cell;

				if (cellController != null)
					Device.BeginInvokeOnMainThread(cellController.SendDisappearing);
				
				_cell = value;
				cellController = value;

				if (cellController != null)
					Device.BeginInvokeOnMainThread(cellController.SendAppearing);
			}
		}

		public Element Element => Cell;

		public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		internal static UITableViewCell GetNativeCell(UITableView tableView, Cell cell, bool recycleCells = false, string templateId = "")
		{
			var id = cell.GetType().FullName;

			var renderer = (CellRenderer)Registrar.Registered.GetHandler(cell.GetType());

			ContextActionsCell contextCell = null;
			UITableViewCell reusableCell = null;
			if (cell.HasContextActions || recycleCells)
			{
				contextCell = (ContextActionsCell)tableView.DequeueReusableCell(ContextActionsCell.Key + templateId);
				if (contextCell == null)
				{
					contextCell = new ContextActionsCell(templateId);
					reusableCell = tableView.DequeueReusableCell(id);
				}
				else
				{
					contextCell.Close();
					reusableCell = contextCell.ContentCell;

					if (reusableCell.ReuseIdentifier.ToString() != id)
						reusableCell = null;
				}
			}
			else
				reusableCell = tableView.DequeueReusableCell(id);

			var nativeCell = renderer.GetCell(cell, reusableCell, tableView);

			var cellWithContent = nativeCell;

			// Sometimes iOS for returns a dequeued cell whose Layer is hidden. 
			// This prevents it from showing up, so lets turn it back on!
			if (cellWithContent.Layer.Hidden)
				cellWithContent.Layer.Hidden = false;

			if (contextCell != null)
			{
				contextCell.Update(tableView, cell, nativeCell);
				nativeCell = contextCell;
			}

			// Because the layer was hidden we need to layout the cell by hand
			if (cellWithContent != null)
				cellWithContent.LayoutSubviews();

			return nativeCell;
		}
	}
}