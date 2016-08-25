using System.ComponentModel;
using System.Windows;
using WCheckBox = System.Windows.Controls.CheckBox;
using System.Windows.Media;

namespace Xamarin.Forms.Platform.WinPhone
{
    public class CheckBoxRenderer : ViewRenderer<CheckBox, WCheckBox>
	{
        private bool _fontApplied;

		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var check = new WCheckBox();
                    check.Checked += OnCheckedChanged;
                    check.Unchecked += OnCheckedChanged;
                    SetNativeControl(check);
                }
            }
		}

        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            ((IElementController)Element).SetValueFromRenderer(CheckBox.IsCheckedProperty, Control.IsChecked);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CheckBox.TextProperty.PropertyName)
                UpdateContent();
            else if (e.PropertyName == CheckBox.TextColorProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == CheckBox.FontProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackground();
            else if (e.PropertyName == CheckBox.IsCheckedProperty.PropertyName)
                UpdateIsChecked();
        }

        void UpdateIsChecked()
        {
            if (Control.IsChecked != Element.IsChecked)
                Control.IsChecked = Element.IsChecked;
        }

        void UpdateContent()
        {
            Control.Content = Element.Text;
        }

        void UpdateFont()
        {
            if (Control == null || Element == null)
                return;

            if (Element.Font == Font.Default && !_fontApplied)
                return;

            Font fontToApply = Element.Font == Font.Default ? Font.SystemFontOfSize(NamedSize.Medium) : Element.Font;

            Control.ApplyFont(fontToApply);
            _fontApplied = true;
        }
        void UpdateBackground()
        {
            Control.Background = Element.BackgroundColor != Color.Default ? Element.BackgroundColor.ToBrush() : (Brush)System.Windows.Application.Current.Resources["PhoneForegroundBrush"];
        }

        void UpdateTextColor()
        {
            Control.Foreground = Element.TextColor != Color.Default ? Element.TextColor.ToBrush() : (Brush)System.Windows.Application.Current.Resources["PhoneForegroundBrush"];
        }
    }
}