using System;
using System.ComponentModel;
using System.Linq;
using Android.App;
using Android.Content.Res;
using Android.Text;
using Android.Widget;
using Object = Java.Lang.Object;
using System.Collections.Specialized;

namespace Xamarin.Forms.Platform.Android.AppCompat
{
	public class PickerRenderer : ViewRenderer<Picker, EditText>
	{
		AlertDialog _dialog;
		bool _disposed;
		TextColorSwitcher _textColorSwitcher;

		public PickerRenderer()
		{
			AutoPackage = false;
		}

		protected override EditText CreateNativeControl()
		{
			return new EditText(Context);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;

				((INotifyCollectionChanged)Element.Items).CollectionChanged -= RowsCollectionChanged;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			if (e.OldElement != null)
				((INotifyCollectionChanged)e.OldElement.Items).CollectionChanged -= RowsCollectionChanged;

			if (e.NewElement != null)
			{
				((INotifyCollectionChanged)e.NewElement.Items).CollectionChanged += RowsCollectionChanged;
				if (Control == null)
				{
					EditText textField = CreateNativeControl();
					textField.Focusable = false;
					textField.Clickable = true;
					textField.Tag = this;
					textField.InputType = InputTypes.Null;
					textField.SetOnClickListener(PickerListener.Instance);
					_textColorSwitcher = new TextColorSwitcher(textField.TextColors);
					SetNativeControl(textField);
				}
				UpdatePicker();
				UpdateTextColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Picker.TitleProperty.PropertyName)
				UpdatePicker();
			if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
				UpdatePicker();
			if (e.PropertyName == Picker.TextColorProperty.PropertyName)
				UpdateTextColor();
		}

		internal override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			base.OnFocusChangeRequested(sender, e);

			if (e.Focus)
				OnClick();
			else if (_dialog != null)
			{
				_dialog.Hide();
				((IElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
				Control.ClearFocus();
				_dialog = null;
			}
		}

		void OnClick()
		{
			Picker model = Element;
			if (_dialog == null)
			{
				using (var builder = new AlertDialog.Builder(Context))
				{
					builder.SetTitle(model.Title ?? "");
					string[] items = model.Items.ToArray();
					builder.SetItems(items, (s, e) => ((IElementController)model).SetValueFromRenderer(Picker.SelectedIndexProperty, e.Which));

					builder.SetNegativeButton(global::Android.Resource.String.Cancel, (o, args) => { });
					
					((IElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);

					_dialog = builder.Create();
				}
				_dialog.SetCanceledOnTouchOutside(true);
				_dialog.DismissEvent += (sender, args) =>
				{
					(Element as IElementController)?.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
					_dialog.Dispose();
					_dialog = null;
				};

				_dialog.Show();
			}
		}

		void RowsCollectionChanged(object sender, EventArgs e)
		{
			UpdatePicker();
		}

		void UpdatePicker()
		{
			Control.Hint = Element.Title;

			if (Element.SelectedIndex == -1 || Element.Items == null)
				Control.Text = null;
			else
				Control.Text = Element.Items[Element.SelectedIndex];
		}

		void UpdateTextColor()
		{
			_textColorSwitcher?.UpdateTextColor(Control, Element.TextColor);
		}

		class PickerListener : Object, IOnClickListener
		{
			#region Statics

			public static readonly PickerListener Instance = new PickerListener();

			#endregion

			public void OnClick(global::Android.Views.View v)
			{
				var renderer = v.Tag as PickerRenderer;
				renderer?.OnClick();
			}
		}
	}
}