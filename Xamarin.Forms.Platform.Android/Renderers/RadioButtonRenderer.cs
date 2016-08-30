using System;
using System.ComponentModel;
using System.Collections.Generic;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;
using ARadioButton = Android.Widget.RadioButton;
using AView = Android.Views.View;
using System.Linq;

namespace Xamarin.Forms.Platform.Android
{
    internal class RadioGroupManager
    {
        Dictionary<string, List<ARadioButton>> _groupMap = new Dictionary<string, List<ARadioButton>>();

        public void JoinGroup(string group, ARadioButton button)
        {
            if (string.IsNullOrEmpty(group))
            {
                group = string.Empty;
            }

            if (!_groupMap.ContainsKey(group))
            {
                _groupMap.Add(group, new List<ARadioButton>());
            }
            _groupMap[group].Add(button);
        }

        public void PartGroup(string group, ARadioButton button)
        {
            if (string.IsNullOrEmpty(group))
            {
                group = string.Empty;
            }

            if (_groupMap.ContainsKey(group))
            {
                _groupMap[group].Remove(button);
            }
        }

        public void UpdateChecked(string group, ARadioButton button)
        {
            if (button.Checked == false)
                return;

            IEnumerable<ARadioButton> dic = null;

            if (string.IsNullOrEmpty(group))
            {
                group = string.Empty;
                dic = _groupMap[group].Where(b => b.Checked && b != button && b.Parent.Parent == button.Parent.Parent);
            }
            else
            {
                dic = _groupMap[group].Where(b => b.Checked && b != button);
            }

            foreach (var btn in dic)
            {
                btn.Checked = false;
            }
        }

        bool ischecker(ARadioButton arg)
        {
            return arg.Checked;
        }

        public void PartGroup(ARadioButton button)
        {
            string realGroup = null;
            foreach (var list in _groupMap)
            {
                if (list.Value.Contains(button))
                {
                    realGroup = list.Key;
                }
            }
            PartGroup(realGroup, button);
        }
    }

    public class RadioButtonRenderer : ViewRenderer<RadioButton, ARadioButton>, CompoundButton.IOnCheckedChangeListener
    {
        TextColorSwitcher _textColorSwitcher;
        float _defaultFontSize;
        Typeface _defaultTypeface;
        bool _isDisposed;
        static Lazy<RadioGroupManager> s_GroupManager = new Lazy<RadioGroupManager>();

        public RadioButtonRenderer()
        {
        }

        void CompoundButton.IOnCheckedChangeListener.OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            ((IViewController)Element).SetValueFromRenderer(RadioButton.IsCheckedProperty, isChecked);

            if (isChecked)
                ((ICheckedController)Element).SendChecked();
            else
                ((ICheckedController)Element).SendUnchecked();
        }

        ARadioButton NativeRadioButton
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
                s_GroupManager.Value.PartGroup(Element.GroupName, Control);
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<RadioButton> e)
        {
            base.OnElementChanged(e);

            ARadioButton radioButton = Control;
            if (e.OldElement == null)
            {
                if (radioButton == null)
                {
                    radioButton = new ARadioButton(Context);
                    SetNativeControl(radioButton);
                    _textColorSwitcher = new TextColorSwitcher(radioButton.TextColors);
                    radioButton.SetOnCheckedChangeListener(this);
                    s_GroupManager.Value.JoinGroup(Element.GroupName, radioButton);
                }
            }

            UpdateAll();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RadioButton.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == RadioButton.TextColorProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateEnabled();
            else if (e.PropertyName == RadioButton.FontProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == RadioButton.IsCheckedProperty.PropertyName)
                UpdateIsChecked();
            else if (e.PropertyName == RadioButton.GroupNameProperty.PropertyName)
                UpdateGroupName();

            base.OnElementPropertyChanged(sender, e);
        }

        void UpdateAll()
        {
            UpdateFont();
            UpdateText();
            UpdateTextColor();
            UpdateEnabled();
            UpdateGroupName();
        }

        void UpdateEnabled()
        {
            Control.Enabled = Element.IsEnabled;
        }

        void UpdateFont()
        {
            RadioButton radioButton = Element;
            if (radioButton.Font == Font.Default && _defaultFontSize == 0f)
                return;

            if (_defaultFontSize == 0f)
            {
                _defaultTypeface = NativeRadioButton.Typeface;
                _defaultFontSize = NativeRadioButton.TextSize;
            }

            if (radioButton.Font == Font.Default)
            {
                NativeRadioButton.Typeface = _defaultTypeface;
                NativeRadioButton.SetTextSize(ComplexUnitType.Px, _defaultFontSize);
            }
            else
            {
                NativeRadioButton.Typeface = radioButton.Font.ToTypeface();
                NativeRadioButton.SetTextSize(ComplexUnitType.Sp, radioButton.Font.ToScaledPixel());
            }
        }

        void UpdateText()
        {
            NativeRadioButton.Text = Element.Text;
        }

        void UpdateTextColor()
        {
            _textColorSwitcher?.UpdateTextColor(Control, Element.TextColor);
        }

        void UpdateIsChecked()
        {
            s_GroupManager.Value.UpdateChecked(Element.GroupName, Control);
        }

        void UpdateGroupName()
        {
            s_GroupManager.Value.PartGroup(Control);
            s_GroupManager.Value.JoinGroup(Element.GroupName, Control);
        }
    }
}
