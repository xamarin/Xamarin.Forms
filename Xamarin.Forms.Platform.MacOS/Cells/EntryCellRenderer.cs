using System;
using System.ComponentModel;
using AppKit;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	public class EntryCellRenderer : CellRenderer
	{
		static readonly Color DefaultTextColor = Color.Black;

		public override NSView GetCell(Cell item, NSView reusableView, NSTableView tv)
		{
			NSTextField nsEntry = null;
			var tvc = reusableView as CellTableViewCell;
			if (tvc == null)
				tvc = new CellTableViewCell(NSTableViewCellStyle.Value2, item.GetType().FullName);
			else
			{
				tvc.Cell.PropertyChanged -= OnCellPropertyChanged;

				nsEntry = tvc.AccessoryView.Subviews[0] as NSTextField;
				nsEntry.RemoveFromSuperview();
				nsEntry.Activated -= OnTextFieldTextChanged;
			}

			SetRealCell(item, tvc);

			if (nsEntry == null)
				tvc.AccessoryView.AddSubview(nsEntry = new NSTextField());

			var entryCell = (EntryCell)item;

			tvc.Cell = item;
			tvc.Cell.PropertyChanged += OnCellPropertyChanged;
			nsEntry.Changed += OnTextFieldTextChanged;

			WireUpForceUpdateSizeRequested(item, tvc, tv);

			UpdateBackground(tvc, entryCell);
			UpdateLabel(tvc, entryCell);
			UpdateText(tvc, entryCell);
			UpdatePlaceholder(tvc, entryCell);
			UpdateLabelColor(tvc, entryCell);
			UpdateHorizontalTextAlignment(tvc, entryCell);
			UpdateIsEnabled(tvc, entryCell);

			return tvc;
		}

		static void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var entryCell = (EntryCell)sender;
			var realCell = (CellTableViewCell)GetRealCell(entryCell);

			if (e.PropertyName == EntryCell.LabelProperty.PropertyName)
				UpdateLabel(realCell, entryCell);
			else if (e.PropertyName == EntryCell.TextProperty.PropertyName)
				UpdateText(realCell, entryCell);
			else if (e.PropertyName == EntryCell.PlaceholderProperty.PropertyName)
				UpdatePlaceholder(realCell, entryCell);
			else if (e.PropertyName == EntryCell.LabelColorProperty.PropertyName)
				UpdateLabelColor(realCell, entryCell);
			else if (e.PropertyName == EntryCell.HorizontalTextAlignmentProperty.PropertyName)
				UpdateHorizontalTextAlignment(realCell, entryCell);
			else if (e.PropertyName == Cell.IsEnabledProperty.PropertyName)
				UpdateIsEnabled(realCell, entryCell);
		}

		static void OnTextFieldTextChanged(object sender, EventArgs eventArgs)
		{
			var notification = (NSNotification)sender;
			var view = (NSView)notification.Object;
			var field = (NSTextField)view;

			CellTableViewCell realCell = null;
			while (view.Superview != null && realCell == null)
			{
				view = view.Superview;
				realCell = view as CellTableViewCell;
			}

			if (realCell != null)
				((EntryCell)realCell.Cell).Text = field.StringValue;
		}

		static void UpdateHorizontalTextAlignment(CellTableViewCell cell, EntryCell entryCell)
		{
			(cell.AccessoryView.Subviews[0] as NSTextField).Alignment = entryCell.HorizontalTextAlignment.ToNativeTextAlignment();
		}

		static void UpdateIsEnabled(CellTableViewCell cell, EntryCell entryCell)
		{
			cell.TextLabel.Enabled = entryCell.IsEnabled;
			(cell.AccessoryView.Subviews[0] as NSTextField).Enabled = entryCell.IsEnabled;
		}

		static void UpdateLabel(CellTableViewCell cell, EntryCell entryCell)
		{
			cell.TextLabel.StringValue = entryCell.Label;
		}

		static void UpdateLabelColor(CellTableViewCell cell, EntryCell entryCell)
		{
			cell.TextLabel.TextColor = entryCell.LabelColor.ToNSColor(DefaultTextColor);
		}

		static void UpdatePlaceholder(CellTableViewCell cell, EntryCell entryCell)
		{
			(cell.AccessoryView.Subviews[0] as NSTextField).PlaceholderString = entryCell.Placeholder ?? "";
		}

		static void UpdateText(CellTableViewCell cell, EntryCell entryCell)
		{
			if ((cell.AccessoryView.Subviews[0] as NSTextField).StringValue == entryCell.Text)
				return;

			(cell.AccessoryView.Subviews[0] as NSTextField).StringValue = entryCell.Text;
		}

		internal override void UpdateBackgroundChild(Cell cell, NSColor backgroundColor)
		{
			var realCell = (CellTableViewCell)GetRealCell(cell);

			(realCell.AccessoryView.Subviews[0] as NSTextField).BackgroundColor = backgroundColor;

			base.UpdateBackgroundChild(cell, backgroundColor);
		}
	}
}

