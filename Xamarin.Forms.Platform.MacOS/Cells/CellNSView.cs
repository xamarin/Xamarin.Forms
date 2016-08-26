using System;
using System.ComponentModel;
using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	public class CellNSView : FormsNSView, INativeElementView
	{
		static readonly NSColor _defaultChildViewsBackground = NSColor.Clear;
		Cell _cell;
		NSTableViewCellStyle _style;

		public Action<object, PropertyChangedEventArgs> PropertyChanged;

		public CellNSView(NSTableViewCellStyle style)
		{
			_style = style;
			CreateUI();
		}

		public NSTextField TextLabel { get; private set; }

		public NSTextField DetailTextLabel { get; private set; }

		public NSImageView ImageView { get; private set; }

		public NSView AccessoryView { get; private set; }

		public Element Element => Cell;

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

		public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		public override void Layout()
		{
			var padding = 10;
			var availableHeight = Frame.Height;
			var availableWidth = Frame.Width - padding * 2;
			nfloat imageWidth = 0;
			nfloat imageHeight = 0;
			nfloat accessoryViewWidth = 0;

			var style = _style;

			if (ImageView != null)
			{
				imageHeight = imageWidth = availableHeight;
				ImageView.Frame = new CGRect(padding, 0, imageWidth, imageHeight);
			}

			if (AccessoryView != null)
			{
				accessoryViewWidth = _style == NSTableViewCellStyle.Value1 ? 50 : availableWidth - 100;
				AccessoryView.Frame = new CGRect(availableWidth - accessoryViewWidth + padding, 0, accessoryViewWidth, availableHeight);
				foreach (var subView in AccessoryView.Subviews)
				{
					//try to find the size the control wants, if no width use default width
					var size = subView.FittingSize;
					if (size.Width == 0)
						size.Width = accessoryViewWidth;

					var x = AccessoryView.Bounds.Width - size.Width;
					var y = (AccessoryView.Bounds.Height - size.Height) / 2;
					subView.Frame = new CGRect(new CGPoint(x, y), size);
				}
			}

			var labelHeights = availableHeight;
			var labelWidth = availableWidth - imageWidth - accessoryViewWidth;

			if (DetailTextLabel != null && !string.IsNullOrEmpty(DetailTextLabel.StringValue))
			{
				labelHeights = availableHeight / 2;
				DetailTextLabel.CenterTextVertically(new CGRect(imageWidth + padding, 0, labelWidth, labelHeights));
			}

			TextLabel.CenterTextVertically(new CGRect(imageWidth + padding, availableHeight - labelHeights, labelWidth, labelHeights));
			base.Layout();
		}


		internal static NSView GetNativeCell(NSTableView tableView, Cell cell, string templateId = "")
		{

			var reusable = tableView.MakeView(templateId, tableView);
			var renderer = (CellRenderer)Registrar.Registered.GetHandler(cell.GetType());
			var nativeCell = renderer.GetCell(cell, reusable, tableView);
			if (string.IsNullOrEmpty(nativeCell.Identifier))
				nativeCell.Identifier = templateId;
			nativeCell.LayoutSubtreeIfNeeded();
			return nativeCell;
		}

		void CreateUI()
		{
			var style = _style;

			AddSubview(TextLabel = new NSTextField
			{
				Bordered = false,
				Selectable = false,
				Editable = false,
				Font = NSFont.LabelFontOfSize(NSFont.SystemFontSize)
			});

			TextLabel.Cell.BackgroundColor = _defaultChildViewsBackground;

			if (style == NSTableViewCellStyle.Image || style == NSTableViewCellStyle.Subtitle || style == NSTableViewCellStyle.ImageSubtitle)
			{
				AddSubview(DetailTextLabel = new NSTextField
				{
					Bordered = false,
					Selectable = false,
					Editable = false,
					Font = NSFont.LabelFontOfSize(NSFont.SmallSystemFontSize)
				});
				DetailTextLabel.Cell.BackgroundColor = _defaultChildViewsBackground;
			}

			if (style == NSTableViewCellStyle.Image || style == NSTableViewCellStyle.ImageSubtitle)
				AddSubview(ImageView = new NSImageView());

			if (style == NSTableViewCellStyle.Value1 || style == NSTableViewCellStyle.Value2)
				AddSubview(AccessoryView = new FormsNSView { BackgroundColor = _defaultChildViewsBackground });
		}
	}
}