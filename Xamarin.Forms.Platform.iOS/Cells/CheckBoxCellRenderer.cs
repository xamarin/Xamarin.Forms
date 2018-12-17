using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CheckBoxCellRenderer : CellRenderer
	{
		const string CellName = "Xamarin.CheckBoxCell";

		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var tvc = reusableCell as CellTableViewCell;
			XFCheckBox checkBox = null;
			if (tvc == null)
				tvc = new CellTableViewCell(UITableViewCellStyle.Value1, CellName);
			else
			{
				checkBox = tvc.AccessoryView as XFCheckBox;
				tvc.PropertyChanged -= HandlePropertyChanged;
			}

			SetRealCell(item, tvc);

			if (checkBox == null)
			{
				checkBox = new XFCheckBox();
				checkBox.ValueChanged += OnCheckedValueChanged;
				tvc.AccessoryView = checkBox;
			}

			var boolCell = (CheckBoxCell)item;

			tvc.Cell = item;
			tvc.PropertyChanged += HandlePropertyChanged;
			tvc.AccessoryView = checkBox;
			tvc.TextLabel.Text = boolCell.Text;

			checkBox.IsChecked = boolCell.IsChecked;

			WireUpForceUpdateSizeRequested(item, tvc, tv);

			UpdateBackground(tvc, item);
			UpdateIsEnabled(tvc, boolCell);
			UpdateFlowDirection(tvc, boolCell);

			return tvc;
		}

		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var boolCell = (CheckBoxCell)sender;
			var realCell = (CellTableViewCell)GetRealCell(boolCell);

			if (e.PropertyName == CheckBoxCell.IsCheckedProperty.PropertyName)
				((XFCheckBox)realCell.AccessoryView).IsChecked = boolCell.IsChecked;
			else if (e.PropertyName == CheckBoxCell.TextProperty.PropertyName)
				realCell.TextLabel.Text = boolCell.Text;
			else if (e.PropertyName == Cell.IsEnabledProperty.PropertyName)
				UpdateIsEnabled(realCell, boolCell);
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection(realCell, boolCell);
		}

		void OnCheckedValueChanged(object sender, EventArgs eventArgs)
		{
			var view = (UIView)sender;
			var sw = (XFCheckBox)view;

			CellTableViewCell realCell = null;
			while (view.Superview != null && realCell == null)
			{
				view = view.Superview;
				realCell = view as CellTableViewCell;
			}

			if (realCell != null)
				((CheckBoxCell)realCell.Cell).IsChecked = sw.IsChecked;
		}

		void UpdateFlowDirection(CellTableViewCell cell, CheckBoxCell checkBoxCell)
		{
			IVisualElementController controller = checkBoxCell.Parent as View;

			var checkBox = cell.AccessoryView as XFCheckBox;

			checkBox.UpdateFlowDirection(controller);
		}

		void UpdateIsEnabled(CellTableViewCell cell, CheckBoxCell checkBoxCell)
		{
			cell.UserInteractionEnabled = checkBoxCell.IsEnabled;
			cell.TextLabel.Enabled = checkBoxCell.IsEnabled;
			cell.DetailTextLabel.Enabled = checkBoxCell.IsEnabled;
			var checkBox = cell.AccessoryView as XFCheckBox;
			if (checkBox != null)
				checkBox.Enabled = checkBoxCell.IsEnabled;
		}
	}
}