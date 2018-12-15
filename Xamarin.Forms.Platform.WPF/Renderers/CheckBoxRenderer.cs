using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFCheckBox = System.Windows.Controls.CheckBox;

namespace Xamarin.Forms.Platform.WPF
{
	public class CheckBoxRenderer : ViewRenderer<CheckBox, WPFCheckBox>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null) // construct and SetNativeControl and suscribe control event
				{
					SetNativeControl(new WPFCheckBox());
					Control.Checked += OnNativeChecked;
					Control.Unchecked += OnNativeChecked;

					
				}

				// Update control property 
				UpdateIsChecked();
			}

			base.OnElementChanged(e);
		}
		
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == CheckBox.IsCheckedProperty.PropertyName)
			{
				UpdateIsChecked();
			}
		}

		void UpdateIsChecked()
		{
			Control.IsChecked = Element.IsChecked;
		}

		void OnNativeChecked(object sender, System.Windows.RoutedEventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(CheckBox.IsCheckedProperty, Control.IsChecked);
		}

		bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				if (Control != null)
				{
					Control.Checked -= OnNativeChecked;
					Control.Unchecked -= OnNativeChecked;
				}
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}
	}
}