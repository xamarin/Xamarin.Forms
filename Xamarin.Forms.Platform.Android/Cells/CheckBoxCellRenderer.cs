using System.ComponentModel;
using Android.Content;
using Android.Views;
using AView = Android.Views.View;
using AColor = Android.Graphics.Color;
using ACheckBox = Android.Widget.CheckBox;
using Android.Graphics;
using Android.Content.Res;
using Android.Support.V4.Widget;

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
			UpdateColors(_view, cell);
			UpdateFlowDirection();

			return _view;
		}

		protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == CheckBoxCell.TextProperty.PropertyName)
				UpdateText();
			else if (args.PropertyName == CheckBoxCell.IsCheckedProperty.PropertyName)
				UpdateChecked();
			else if (args.PropertyName == "RenderHeight")
				UpdateHeight();
			else if (args.PropertyName == Cell.IsEnabledProperty.PropertyName)
				UpdateIsEnabled(_view, (CheckBoxCell)sender);
			else if (args.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection();
			else if (args.PropertyName == CheckBox.CheckedColorProperty.PropertyName ||
				args.PropertyName == CheckBox.UncheckedColorProperty.PropertyName)
			{
				UpdateColors(_view, (CheckBoxCell)sender);
			}
		}

		void UpdateChecked()
		{
			((ACheckBox)_view.AccessoryView).Checked = ((CheckBoxCell)Cell).IsChecked;
		}

		void UpdateColors(CheckBoxCellView cell, CheckBoxCell checkBoxCell)
		{
			var aCheckBox = cell.AccessoryView as ACheckBox;
			if (aCheckBox == null)
				return;

			var mode = PorterDuff.Mode.SrcIn;

			var stateChecked = global::Android.Resource.Attribute.StateChecked;
			var stateEnabled = global::Android.Resource.Attribute.StateEnabled;

			//Need to find a way to get this color out of Android somehow.
			var uncheckedDefault = AColor.Gray;
			var disabledColor = AColor.LightGray;

			var list = new ColorStateList(
					new int[][]
					{
					new int[] { -stateEnabled, stateChecked },
					new int[] { stateEnabled, stateChecked },
					new int[] { stateEnabled, -stateChecked },
					new int[] { },
					},
					new int[]
					{
					disabledColor,
					checkBoxCell.CheckedColor == Color.Default ? Color.Accent.ToAndroid() : checkBoxCell.CheckedColor.ToAndroid(),
					checkBoxCell.UncheckedColor == Color.Default ? uncheckedDefault : checkBoxCell.UncheckedColor.ToAndroid(),
					disabledColor,
					});


			if (Forms.IsLollipopOrNewer)
			{
				aCheckBox.ButtonTintList = list;
				aCheckBox.ButtonTintMode = mode;
			}
			else
			{
				CompoundButtonCompat.SetButtonTintList(aCheckBox, list);
				CompoundButtonCompat.SetButtonTintMode(aCheckBox, mode);
			}
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