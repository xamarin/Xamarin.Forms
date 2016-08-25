using System;
using System.ComponentModel;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;
using static System.String;
using ACheckBox = Android.Widget.CheckBox;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android
{
    public class CheckBoxRenderer : ViewRenderer<CheckBox, ACheckBox>, CompoundButton.IOnCheckedChangeListener
    {
        CheckBoxDrawable _backgroundDrawable;
        TextColorSwitcher _textColorSwitcher;
        Drawable _defaultDrawable;
        float _defaultFontSize;
        Typeface _defaultTypeface;
        bool _drawableEnabled;
        bool _isDisposed;

        public CheckBoxRenderer()
        {
            AutoPackage = false;
        }

        ACheckBox NativeCheckBox
        {
            get { return Control; }
        }

        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            UpdateText();
            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (disposing)
            {
                if (_backgroundDrawable != null)
                {
                    _backgroundDrawable.Dispose();
                    _backgroundDrawable = null;
                }
                
            }
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                ACheckBox checkBox = Control;
                if (checkBox == null)
                {
                    checkBox = new ACheckBox(Context);
                    checkBox.Tag = this;
                    SetNativeControl(checkBox);
                    _textColorSwitcher = new TextColorSwitcher(checkBox.TextColors);
                    checkBox.SetOnCheckedChangeListener(this);
                }
            }
            else
            {
                if (_drawableEnabled)
                {
                    _drawableEnabled = false;
                    _backgroundDrawable.Reset();
                    _backgroundDrawable = null;
                }
            }
            UpdateAll();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CheckBox.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == CheckBox.TextColorProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == CheckBox.FontProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateEnabled();
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateDrawable();
            else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
                UpdateText();

            if (_drawableEnabled && (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName))
            {
               _backgroundDrawable.Reset();
                Control.Invalidate();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void UpdateBackgroundColor()
        {
            // Do nothing, the drawable handles this now
        }

        void UpdateAll()
        {
            UpdateFont();
            UpdateText();
            UpdateTextColor();
            UpdateEnabled();
            UpdateDrawable();
        }

        void UpdateDrawable()
        {
            if (Element.BackgroundColor == Color.Default)
            {
                if (!_drawableEnabled)
                    return;

                if (_defaultDrawable != null)
                    Control.SetBackground(_defaultDrawable);

                _drawableEnabled = false;
            }
            else
            {
                if (_backgroundDrawable == null)
                    _backgroundDrawable = new CheckBoxDrawable();

                _backgroundDrawable.CheckBox = Element;

                if (_drawableEnabled)
                    return;

                if (_defaultDrawable == null)
                    _defaultDrawable = Control.Background;

                Control.SetBackground(_backgroundDrawable);
                _drawableEnabled = true;
            }
            Control.Invalidate();
        }

        void UpdateEnabled()
        {
            Control.Enabled = Element.IsEnabled;
        }

        void UpdateFont()
        {
            CheckBox checkbox = Element;
            if (checkbox.Font == Font.Default && _defaultFontSize == 0f)
                return;

            if (_defaultFontSize == 0f)
            {
                _defaultTypeface = NativeCheckBox.Typeface;
                _defaultFontSize = NativeCheckBox.TextSize;
            }

            if (checkbox.Font == Font.Default)
            {
                NativeCheckBox.Typeface = _defaultTypeface;
                NativeCheckBox.SetTextSize(ComplexUnitType.Px, _defaultFontSize);
            }
            else
            {
                NativeCheckBox.Typeface = checkbox.Font.ToTypeface();
                NativeCheckBox.SetTextSize(ComplexUnitType.Sp, checkbox.Font.ToScaledPixel());
            }
        }

        void UpdateText()
        {
            var oldText = NativeCheckBox.Text;
            NativeCheckBox.Text = Element.Text;
        }

        void UpdateTextColor()
        {
            _textColorSwitcher?.UpdateTextColor(Control, Element.TextColor);
        }

        void CompoundButton.IOnCheckedChangeListener.OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            ((IViewController)Element).SetValueFromRenderer(CheckBox.IsCheckedProperty, isChecked);
        }
    }
}