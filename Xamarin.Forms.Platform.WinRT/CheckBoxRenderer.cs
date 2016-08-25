using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using WCheckBox = Windows.UI.Xaml.Controls.CheckBox;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
    public class CheckBoxRenderer : ViewRenderer<CheckBox, WCheckBox>
	{
		bool _fontApplied;

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

				UpdateContent();

				if (Element.BackgroundColor != Color.Default)
					UpdateBackground();

				if (Element.TextColor != Color.Default)
					UpdateTextColor();

				UpdateFont();
                UpdateIsChecked();
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
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackground();
			else if (e.PropertyName == CheckBox.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == CheckBox.FontProperty.PropertyName)
				UpdateFont();
            else if (e.PropertyName == CheckBox.IsCheckedProperty.PropertyName)
                UpdateIsChecked();
        }

        void UpdateIsChecked()
        {
            if (Control.IsChecked != Element.IsChecked)
                Control.IsChecked = Element.IsChecked;
        }

        void UpdateBackground()
		{
			Control.Background = Element.BackgroundColor != Color.Default ? Element.BackgroundColor.ToBrush() : (Brush)Windows.UI.Xaml.Application.Current.Resources["CheckBoxBackgroundThemeBrush"];
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

		void UpdateTextColor()
		{
			Control.Foreground = Element.TextColor != Color.Default ? Element.TextColor.ToBrush() : (Brush)Windows.UI.Xaml.Application.Current.Resources["CheckBoxContentForegroundThemeBrush"];
		}
	}
}