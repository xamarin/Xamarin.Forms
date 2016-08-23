using System;
using System.ComponentModel;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class SwitchCellRenderer : CellRenderer
	{
		const string CellName = "Xamarin.SwitchCell";

		public override NSView GetCell(Cell item, NSView reusableCell, NSTableView tv)
		{
			var tvc = reusableCell as CellTableViewCell;
			NSButton uiSwitch = null;
			if (tvc == null)
				tvc = new CellTableViewCell(NSTableViewCellStyle.Value1, CellName);
			else
			{
				uiSwitch = tvc.AccessoryView.Subviews[0] as NSButton;
				uiSwitch.RemoveFromSuperview();
				uiSwitch.Activated -= OnSwitchValueChanged;
				tvc.Cell.PropertyChanged -= OnCellPropertyChanged;
			}

			SetRealCell(item, tvc);

			if (uiSwitch == null)
			{
				uiSwitch = new NSButton { AllowsMixedState = false, Title = string.Empty };
				uiSwitch.SetButtonType(NSButtonType.Switch);
			}

			var boolCell = (SwitchCell)item;

			tvc.Cell = item;
			tvc.Cell.PropertyChanged += OnCellPropertyChanged;
			tvc.AccessoryView.AddSubview(uiSwitch);
			tvc.TextLabel.StringValue = boolCell.Text;

			uiSwitch.State = boolCell.On ? NSCellStateValue.On : NSCellStateValue.Off;
			uiSwitch.Activated += OnSwitchValueChanged;
			WireUpForceUpdateSizeRequested(item, tvc, tv);

			UpdateBackground(tvc, item);
			UpdateIsEnabled(tvc, boolCell);

			return tvc;
		}

		void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var boolCell = (SwitchCell)sender;
			var realCell = (CellTableViewCell)GetRealCell(boolCell);

			if (e.PropertyName == SwitchCell.OnProperty.PropertyName)
				((NSButton)realCell.AccessoryView.Subviews[0]).State = boolCell.On ? NSCellStateValue.On : NSCellStateValue.Off;
			else if (e.PropertyName == SwitchCell.TextProperty.PropertyName)
				realCell.TextLabel.StringValue = boolCell.Text;
			else if (e.PropertyName == Cell.IsEnabledProperty.PropertyName)
				UpdateIsEnabled(realCell, boolCell);
		}

		void OnSwitchValueChanged(object sender, EventArgs eventArgs)
		{
			var view = (NSView)sender;
			var sw = (NSButton)view;

			CellTableViewCell realCell = null;
			while (view.Superview != null && realCell == null)
			{
				view = view.Superview;
				realCell = view as CellTableViewCell;
			}

			if (realCell != null)
				((SwitchCell)realCell.Cell).On = sw.State == NSCellStateValue.On;
		}

		void UpdateIsEnabled(CellTableViewCell cell, SwitchCell switchCell)
		{
			cell.TextLabel.Enabled = switchCell.IsEnabled;
			var uiSwitch = cell.AccessoryView.Subviews[0] as NSButton;
			if (uiSwitch != null)
				uiSwitch.Enabled = switchCell.IsEnabled;
		}
	}
}

