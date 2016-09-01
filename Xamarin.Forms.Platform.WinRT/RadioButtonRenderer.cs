using System.Collections;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using WRadioButton = Windows.UI.Xaml.Controls.RadioButton;

#if WINDOWS_UWP
namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
    public class RadioButtonRenderer : ViewRenderer<RadioButton, WRadioButton>
    {
        bool _fontApplied;
        IElementController ElementController => Element as IElementController;

        protected override void OnElementChanged(ElementChangedEventArgs<RadioButton> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var radioButton = new WRadioButton();
                    radioButton.Checked += RadioButton_Checked;
                    radioButton.Unchecked += RadioButton_Unchecked;
                    SetNativeControl(radioButton);
                }

                UpdateAll();
            }
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ElementController.SetValueFromRenderer(RadioButton.IsCheckedProperty, Control.IsChecked);
            RadioButton radio = Element;
            ((ICheckedController)radio).SendUnchecked();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ElementController.SetValueFromRenderer(RadioButton.IsCheckedProperty, Control.IsChecked);
            RadioButton radio = Element;
            ((ICheckedController)radio).SendChecked();
            UpdateRadioButtonGroup();
        }

        void UpdateAll()
        {
            UpdateText();
            UpdateFont();
            UpdateTextColor();
            UpdateIsChecked();
            UpdateGroupName();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == RadioButton.TextProperty.PropertyName)
            {
                UpdateText();
            }
            else if (e.PropertyName == RadioButton.TextColorProperty.PropertyName)
            {
                UpdateTextColor();
            }
            else if (e.PropertyName == RadioButton.FontProperty.PropertyName)
            {
                UpdateFont();
            }
            else if (e.PropertyName == RadioButton.IsCheckedProperty.PropertyName)
            {
                UpdateIsChecked();
            }
            else if (e.PropertyName == RadioButton.GroupNameProperty.PropertyName)
            {
                UpdateGroupName();
            }
        }
        
        void UpdateText()
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
            Control.Foreground = Element.TextColor != Color.Default ? Element.TextColor.ToBrush() : (Brush)Windows.UI.Xaml.Application.Current.Resources["DefaultTextForegroundThemeBrush"];
        }

        private void UpdateRadioButtonGroup()
        {
            string groupName = Element.GroupName;
            if (string.IsNullOrEmpty(groupName))
            {
                DependencyObject parent = this.Parent;
                if (parent != null)
                {
                    // Traverse Parent children
                    LayoutRenderer layout = Parent as LayoutRenderer;
                    IEnumerable children = layout.Children;
                    IEnumerator itor = children.GetEnumerator();
                    while (itor.MoveNext())
                    {
                        WRadioButton rb = (itor.Current as RadioButtonRenderer)?.Control;
                        if (rb != null && rb != this.Control && string.IsNullOrEmpty(rb.GroupName) && (rb.IsChecked == true))
                            rb.SetValue(WRadioButton.IsCheckedProperty, false);
                    }
                }
            }
        }

        void UpdateGroupName()
        {
            Control.SetValue(WRadioButton.GroupNameProperty, Element.GroupName);
        }

        private void UpdateIsChecked()
        {
            Control.IsChecked = Element.IsChecked;
        }
    }
}