using System.ComponentModel;
using Android.Content;
using Android.Views;
using AView = Android.Views.View;
using ACheckBox = Android.Widget.CheckBox;

namespace Xamarin.Forms.Platform.Android
{
	public class CheckBoxCellRenderer : CellRenderer
	{
		CheckBoxCellView _view;

		protected override AView GetCellCore(Cell item, AView convertView, ViewGroup parent, Context context)
		{
			var cell = (CheckBoxCell)Cell;

			if ((_view = convertView as CheckBoxCellView) == null)
				_view = new CheckBoxCellView(context, item);

			_view.Cell = cell;

			UpdateText();
			UpdateChecked();
			UpdateHeight();
			UpdateIsEnabled(_view, cell);
			UpdateFlowDirection();

			return _view;
		}

		protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == CheckBoxCell.TextProperty.PropertyName)
				UpdateText();
			else if (args.PropertyName == CheckBoxCell.OnProperty.PropertyName)
				UpdateChecked();
			else if (args.PropertyName == "RenderHeight")
				UpdateHeight();
			else if (args.PropertyName == Cell.IsEnabledProperty.PropertyName)
				UpdateIsEnabled(_view, (CheckBoxCell)sender);
			else if (args.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection();
		}

		void UpdateChecked()
		{
			((ACheckBox)_view.AccessoryView).Checked = ((CheckBoxCell)Cell).On;
		}

		void UpdateIsEnabled(CheckBoxCellView cell, CheckBoxCell checkBoxCell)
		{
			cell.Enabled = checkBoxCell.IsEnabled;
			var aCheckBox = cell.AccessoryView as ACheckBox;
			if (aCheckBox != null)
				aCheckBox.Enabled = checkBoxCell.IsEnabled;
		}

		void UpdateFlowDirection()
		{
			_view.UpdateFlowDirection(ParentView);
		}

		void UpdateHeight()
		{
			_view.SetRenderHeight(Cell.RenderHeight);
		}

		void UpdateText()
		{
			_view.MainText = ((CheckBoxCell)Cell).Text;
		}
	}
}