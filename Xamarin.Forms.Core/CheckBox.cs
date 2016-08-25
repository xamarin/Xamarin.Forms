using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
    [RenderWith(typeof(_CheckBoxRenderer))]
    public class CheckBox : View, IFontElement
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(CheckBox), null,
            propertyChanged: (bindable, oldVal, newVal) => ((CheckBox)bindable).InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged));

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(CheckBox), Color.Default);

        public static readonly BindableProperty FontProperty = BindableProperty.Create("Font", typeof(Font), typeof(CheckBox), default(Font), propertyChanged: FontStructPropertyChanged);

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create("FontFamily", typeof(string), typeof(CheckBox), default(string), propertyChanged: SpecificFontPropertyChanged);

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(CheckBox), -1.0, propertyChanged: SpecificFontPropertyChanged,
            defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, (CheckBox)bindable));

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create("FontAttributes", typeof(FontAttributes), typeof(CheckBox), FontAttributes.None,
            propertyChanged: SpecificFontPropertyChanged);

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create("IsChecked", typeof(bool), typeof(CheckBox), default(bool), propertyChanged: OnCheckedChanged, defaultBindingMode: BindingMode.TwoWay);

        bool _cancelEvents;

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

        bool IsEnabledCore
        {
            set { SetValueCore(IsEnabledProperty, value); }
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

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public event EventHandler<CheckedEventArgs> Checked;

        static void OnCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((CheckBox)bindable).Checked?.Invoke(bindable, new CheckedEventArgs((bool)newValue));
        }

        static void FontStructPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var checkBox = (CheckBox)bindable;

            if (checkBox._cancelEvents)
                return;

            checkBox.InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

            checkBox._cancelEvents = true;

            if (checkBox.Font == Font.Default)
            {
                checkBox.FontFamily = null;
                checkBox.FontSize = Device.GetNamedSize(NamedSize.Default, checkBox);
                checkBox.FontAttributes = FontAttributes.None;
            }
            else
            {
                checkBox.FontFamily = checkBox.Font.FontFamily;
                if (checkBox.Font.UseNamedSize)
                {
                    checkBox.FontSize = Device.GetNamedSize(checkBox.Font.NamedSize, checkBox.GetType(), true);
                }
                else
                {
                    checkBox.FontSize = checkBox.Font.FontSize;
                }
                checkBox.FontAttributes = checkBox.Font.FontAttributes;
            }

            checkBox._cancelEvents = false;
        }

        static void SpecificFontPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var checkBox = (CheckBox)bindable;

            if (checkBox._cancelEvents)
                return;

            checkBox.InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

            checkBox._cancelEvents = true;

            if (checkBox.FontFamily != null)
            {
                checkBox.Font = Font.OfSize(checkBox.FontFamily, checkBox.FontSize).WithAttributes(checkBox.FontAttributes);
            }
            else
            {
                checkBox.Font = Font.SystemFontOfSize(checkBox.FontSize, checkBox.FontAttributes);
            }

            checkBox._cancelEvents = false;
        }
    }
}