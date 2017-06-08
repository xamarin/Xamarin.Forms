using System;
using System.ComponentModel;
using System.Collections.Generic;
using ElmSharp;
using EColor = ElmSharp.Color;

namespace Xamarin.Forms.Platform.Tizen
{
	public class PickerRenderer : ViewRenderer<Picker, Native.Button>
	{
		static readonly EColor s_defaultTextColor = EColor.White;
		internal List _list;
		internal Native.Dialog _dialog;
		Dictionary<ListItem, int> _itemToItemNumber = new Dictionary<ListItem, int>();

		public PickerRenderer()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			if (Control == null)
			{
				var button = new Native.Button(Forms.Context.MainWindow);
				SetNativeControl (button);
			}

			if (e.OldElement != null)
			{
				Control.Clicked -= OnClick;
			}

			if (e.NewElement != null)
			{
				UpdateSelectedIndex();
				UpdateTextColor();
				Control.Clicked += OnClick;
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
			{
				UpdateSelectedIndex();
			}
			else if (e.PropertyName == Picker.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
		}

		void UpdateSelectedIndex()
		{
			Control.Text = (Element.SelectedIndex == -1 || Element.Items == null ?
				"" : Element.Items[Element.SelectedIndex]);
		}

		void UpdateTextColor()
		{
			Control.TextColor = Element.TextColor.IsDefault ? s_defaultTextColor : Element.TextColor.ToNative();
		}

		void OnClick(object sender, EventArgs e)
		{
			int i = 0;
			_dialog = new Native.Dialog(Forms.Context.MainWindow);
			_list = new List(_dialog);
			_dialog.AlignmentX = -1;
			_dialog.AlignmentY = -1;

			_dialog.Title = Element.Title;
			_dialog.Dismissed += DialogDismissed;
			_dialog.BackButtonPressed += (object senders, EventArgs es) =>
			{
				_dialog.Dismiss();
			};

			foreach (var s in Element.Items)
			{
				ListItem item = _list.Append(s);
				_itemToItemNumber[item] = i;
				i++;
			}
			_list.ItemSelected += ItemSelected;
			_dialog.Content = _list;

			_dialog.Show();
			_list.Show();
		}

		void ItemSelected(object senderObject, EventArgs ev)
		{
			Element.SelectedIndex = _itemToItemNumber[(senderObject as List).SelectedItem];
			_dialog.Dismiss();
		}

		void DialogDismissed(object sender, EventArgs e)
		{
			CleanView();
		}

		void CleanView()
		{
			if (null != _list)
			{
				_list.Unrealize();
				_itemToItemNumber.Clear();
				_list = null;
			}
			if (null != _dialog)
			{
				_dialog.Unrealize();
				_dialog = null;
			}
		}
	}
}
