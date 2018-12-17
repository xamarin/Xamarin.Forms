using System;
using System.ComponentModel;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class CheckBoxCellRenderer : CellRenderer
	{
		public override NSView GetCell(Cell item, NSView reusableView, NSTableView tv)
		{
			var tvc = reusableView as CellNSView;
			NSButton nsCheckBox = null;
			if (tvc == null)
				tvc = new CellNSView(NSTableViewCellStyle.Value1);
			else
			{
				nsCheckBox = tvc.AccessoryView.Subviews[0] as NSButton;
				if (nsCheckBox != null)
				{
					nsCheckBox.RemoveFromSuperview();
					nsCheckBox.Activated -= OnCheckBoxValueChanged;
				}
				tvc.Cell.PropertyChanged -= OnCellPropertyChanged;
			}

			SetRealCell(item, tvc);

			if (nsCheckBox == null)
			{
				nsCheckBox = new NSButton { AllowsMixedState = false, Title = string.Empty };
				nsCheckBox.SetButtonType(NSButtonType.Switch);
			}

			var boolCell = (CheckBoxCell)item;

			tvc.Cell = item;
			tvc.Cell.PropertyChanged += OnCellPropertyChanged;
			tvc.AccessoryView.AddSubview(nsCheckBox);
			tvc.TextLabel.StringValue = boolCell.Text ?? "";

			nsCheckBox.State = boolCell.IsChecked ? NSCellStateValue.On : NSCellStateValue.Off;
			nsCheckBox.Activated += OnCheckBoxValueChanged;
			WireUpForceUpdateSizeRequested(item, tvc, tv);

			UpdateBackground(tvc, item);
			UpdateIsEnabled(tvc, boolCell);

			return tvc;
		}

		static void UpdateIsEnabled(CellNSView cell, CheckBoxCell checkBoxCell)
		{
			cell.TextLabel.Enabled = checkBoxCell.IsEnabled;
			var uiCheckBox = cell.AccessoryView.Subviews[0] as NSButton;
			if (uiCheckBox != null)
				uiCheckBox.Enabled = checkBoxCell.IsEnabled;
		}

		void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var boolCell = (CheckBoxCell)sender;
			var realCell = (CellNSView)GetRealCell(boolCell);

			if (e.PropertyName == CheckBoxCell.IsCheckedProperty.PropertyName)
				((NSButton)realCell.AccessoryView.Subviews[0]).State = boolCell.IsChecked ? NSCellStateValue.On : NSCellStateValue.Off;
			else if (e.PropertyName == CheckBoxCell.TextProperty.PropertyName)
				realCell.TextLabel.StringValue = boolCell.Text ?? "";
			else if (e.PropertyName == Cell.IsEnabledProperty.PropertyName)
				UpdateIsEnabled(realCell, boolCell);
		}

		void OnCheckBoxValueChanged(object sender, EventArgs eventArgs)
		{
			var view = (NSView)sender;
			var sw = (NSButton)view;

			CellNSView realCell = null;
			while (view.Superview != null && realCell == null)
			{
				view = view.Superview;
				realCell = view as CellNSView;
			}

			if (realCell != null)
				((CheckBoxCell)realCell.Cell).IsChecked = sw.State == NSCellStateValue.On;
		}
	}
}