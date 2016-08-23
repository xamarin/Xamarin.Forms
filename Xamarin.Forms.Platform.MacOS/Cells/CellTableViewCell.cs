using System;
using System.ComponentModel;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class CellTableViewCell : FormsNSView, INativeElementView
	{

		Cell _cell;
		NSTableViewCellStyle _style;

		public Action<object, PropertyChangedEventArgs> PropertyChanged;

		public CellTableViewCell(NSTableViewCellStyle style, string key)
		{
			_style = style;
			CreateUI();
		}

		public override void Layout()
		{
			var availableHeight = Frame.Height;
			var availableWidth = Frame.Width;
			nfloat imageWidth = 0;
			nfloat imageHeight = 0;

			if (ImageView != null)
			{
				imageHeight = imageWidth = availableHeight;
				ImageView.Frame = new CoreGraphics.CGRect(0, 0, imageWidth, imageHeight);
			}

			if (DetailTextLabel != null)
			{
				var labelHeights = availableHeight / 2;
				var labelWidth = availableWidth - imageWidth;
				DetailTextLabel.Frame = new CoreGraphics.CGRect(imageWidth, 0, labelWidth, labelHeights);
				TextLabel.Frame = new CoreGraphics.CGRect(imageWidth, labelHeights, labelWidth, labelHeights);
			}

			base.Layout();
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

		public NSTextField TextLabel { get; private set; }

		public NSTextField DetailTextLabel { get; private set; }

		public NSImageView ImageView { get; private set; }

		internal static NSView GetNativeCell(NSTableView tableView, Cell cell, bool recycleCells = false, string templateId = "")
		{
			var id = cell.GetType().FullName;

			var renderer = (CellRenderer)Registrar.Registered.GetHandler(cell.GetType());

			//ContextActionsCell contextCell = null;
			//UITableViewCell reusableCell = null;
			//if (cell.HasContextActions || recycleCells)
			//{
			//	contextCell = (ContextActionsCell)tableView.DequeueReusableCell(ContextActionsCell.Key + templateId);
			//	if (contextCell == null)
			//	{
			//		contextCell = new ContextActionsCell(templateId);
			//		reusableCell = tableView.DequeueReusableCell(id);
			//	}
			//	else
			//	{
			//		contextCell.Close();
			//		reusableCell = contextCell.ContentCell;

			//		if (reusableCell.ReuseIdentifier.ToString() != id)
			//			reusableCell = null;
			//	}
			//}
			//else
			//	reusableCell = tableView.DequeueReusableCell(id);

			var nativeCell = renderer.GetCell(cell, null, tableView);

			//var cellWithContent = nativeCell;

			//// Sometimes iOS for returns a dequeued cell whose Layer is hidden. 
			//// This prevents it from showing up, so lets turn it back on!
			//if (cellWithContent.Layer.Hidden)
			//	cellWithContent.Layer.Hidden = false;

			//if (contextCell != null)
			//{
			//	contextCell.Update(tableView, cell, nativeCell);
			//	var viewTableCell = contextCell.ContentCell as ViewCellRenderer.ViewTableCell;
			//	if (viewTableCell != null)
			//		viewTableCell.SupressSeparator = true;
			//	nativeCell = contextCell;
			//}

			//// Because the layer was hidden we need to layout the cell by hand
			//if (cellWithContent != null)
			//	cellWithContent.LayoutSubviews();

			return nativeCell;
		}

		void CreateUI()
		{
			var style = _style;

			AddSubview(TextLabel = new NSTextField
			{
				Bordered = false,
				Selectable = false,
				Editable = false
			});

			TextLabel.Cell.BackgroundColor = Color.Transparent.ToNSColor();

			if (style == NSTableViewCellStyle.Image || style == NSTableViewCellStyle.Subtitle)
			{
				AddSubview(DetailTextLabel = new NSTextField
				{
					Bordered = false,
					Selectable = false,
					Editable = false
				});
				DetailTextLabel.Cell.BackgroundColor = Color.Transparent.ToNSColor();
			}

			if (style == NSTableViewCellStyle.Image)
				AddSubview(ImageView = new NSImageView());

		}

		public virtual void SetBackground()
		{

		}
	}
}