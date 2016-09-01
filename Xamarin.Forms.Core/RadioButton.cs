using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
    [RenderWith(typeof(_RadioButtonRenderer))]
    public class RadioButton : View, IFontElement, ICheckedController
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(RadioButton), null,
          propertyChanged: (bindable, oldVal, newVal) => ((RadioButton)bindable).InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged));

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(RadioButton), Color.Default);

        public static readonly BindableProperty FontProperty = BindableProperty.Create("Font", typeof(Font), typeof(RadioButton), default(Font), propertyChanged: FontStructPropertyChanged);

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create("FontFamily", typeof(string), typeof(RadioButton), default(string), propertyChanged: SpecificFontPropertyChanged);

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(RadioButton), -1.0, propertyChanged: SpecificFontPropertyChanged,
          defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, (RadioButton)bindable));

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create("FontAttributes", typeof(FontAttributes), typeof(RadioButton), FontAttributes.None,
          propertyChanged: SpecificFontPropertyChanged);

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create("IsChecked", typeof(bool), typeof(RadioButton), false);

        public static readonly BindableProperty GroupNameProperty = BindableProperty.Create("GroupName", typeof(string), typeof(RadioButton), null);

        public Font Font
        {
            get { return (Font)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public FontAttributes FontAttributes
        {
            get { return (FontAttributes)GetValue(FontAttributesProperty); }
            set { SetValue(FontAttributesProperty, value); }
        }

        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value ?? string.Empty); }
        }

        public event EventHandler Checked;

        public event EventHandler Unchecked;

        static void FontStructPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var radioButton = (RadioButton)bindable;

            radioButton.InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

            if (radioButton.Font == Font.Default)
            {
                radioButton.FontFamily = null;
                radioButton.FontSize = Device.GetNamedSize(NamedSize.Default, radioButton);
                radioButton.FontAttributes = FontAttributes.None;
            }
            else
            {
                radioButton.FontFamily = radioButton.Font.FontFamily;
                if (radioButton.Font.UseNamedSize)
                {
                    radioButton.FontSize = Device.GetNamedSize(radioButton.Font.NamedSize, radioButton.GetType(), true);
                }
                else
                {
                    radioButton.FontSize = radioButton.Font.FontSize;
                }
                radioButton.FontAttributes = radioButton.Font.FontAttributes;
            }
        }

        static void SpecificFontPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var radioButton = (RadioButton)bindable;

            radioButton.InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

            if (radioButton.FontFamily != null)
            {
                radioButton.Font = Font.OfSize(radioButton.FontFamily, radioButton.FontSize).WithAttributes(radioButton.FontAttributes);
            }
            else
            {
                radioButton.Font = Font.SystemFontOfSize(radioButton.FontSize, radioButton.FontAttributes);
            }
        }

        void ICheckedController.SendChecked()
        {
            Checked?.Invoke(this, EventArgs.Empty);
        }

        void ICheckedController.SendUnchecked()
        {
            Unchecked?.Invoke(this, EventArgs.Empty);
        }
    }
}
