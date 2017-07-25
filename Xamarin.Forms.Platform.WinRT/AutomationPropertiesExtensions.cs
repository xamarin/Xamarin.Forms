using Windows.UI.Xaml;
using WAutomationProperties = Windows.UI.Xaml.Automation.AutomationProperties;
using Windows.UI.Xaml.Automation.Peers;

#if WINDOWS_UWP
namespace Xamarin.Forms.Platform.UWP
#else
namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public static class AutomationPropertiesExtensions
	{
		public static string SetAutomationPropertiesName(this FrameworkElement Control, Element Element, string _defaultAutomationPropertiesName = null)
		{
			if (Element == null)
				return _defaultAutomationPropertiesName;

			if (_defaultAutomationPropertiesName == null)
				_defaultAutomationPropertiesName = (string)Control.GetValue(WAutomationProperties.NameProperty);

			var elemValue = (string)Element.GetValue(AutomationProperties.NameProperty);

			if (!string.IsNullOrWhiteSpace(elemValue))
				Control.SetValue(WAutomationProperties.NameProperty, elemValue);
			else
				Control.SetValue(WAutomationProperties.NameProperty, _defaultAutomationPropertiesName);

			return _defaultAutomationPropertiesName;
		}

		public static AccessibilityView? SetAutomationPropertiesAccessibilityView(this FrameworkElement Control, Element Element, AccessibilityView? _defaultAutomationPropertiesAccessibilityView = null)
		{
			if (Element == null)
				return _defaultAutomationPropertiesAccessibilityView;

			if (!_defaultAutomationPropertiesAccessibilityView.HasValue)
				_defaultAutomationPropertiesAccessibilityView = (AccessibilityView)Control.GetValue(WAutomationProperties.AccessibilityViewProperty);

			var newValue = _defaultAutomationPropertiesAccessibilityView;

			var elemValue = (bool?)Element.GetValue(AutomationProperties.IsInAccessibleTreeProperty);

			if (elemValue == true)
				newValue = AccessibilityView.Content;

			else if (elemValue == false)
				newValue = AccessibilityView.Raw;

			Control.SetValue(WAutomationProperties.AccessibilityViewProperty, newValue);

			return _defaultAutomationPropertiesAccessibilityView;
		}

		public static string SetAutomationPropertiesHelpText(this FrameworkElement Control, Element Element, string _defaultAutomationPropertiesHelpText = null)
		{
			if (Element == null)
				return _defaultAutomationPropertiesHelpText;

			if (_defaultAutomationPropertiesHelpText == null)
				_defaultAutomationPropertiesHelpText = (string)Control.GetValue(WAutomationProperties.HelpTextProperty);

			var elemValue = (string)Element.GetValue(AutomationProperties.HelpTextProperty);

			if (!string.IsNullOrWhiteSpace(elemValue))
				Control.SetValue(WAutomationProperties.HelpTextProperty, elemValue);
			else
				Control.SetValue(WAutomationProperties.HelpTextProperty, _defaultAutomationPropertiesHelpText);

			return _defaultAutomationPropertiesHelpText;
		}

		public static UIElement SetAutomationPropertiesLabeledBy(this FrameworkElement Control, Element Element, UIElement _defaultAutomationPropertiesLabeledBy = null)
		{
			if (Element == null)
				return _defaultAutomationPropertiesLabeledBy;

			if (_defaultAutomationPropertiesLabeledBy == null)
				_defaultAutomationPropertiesLabeledBy = (UIElement)Control.GetValue(WAutomationProperties.LabeledByProperty);

			var elemValue = (VisualElement)Element.GetValue(AutomationProperties.LabeledByProperty);

			var renderer = elemValue?.GetOrCreateRenderer();

			var nativeElement = renderer?.GetNativeElement();

			if (nativeElement != null)
				Control.SetValue(WAutomationProperties.LabeledByProperty, nativeElement);
			else
				Control.SetValue(WAutomationProperties.LabeledByProperty, _defaultAutomationPropertiesLabeledBy);

			return _defaultAutomationPropertiesLabeledBy;
		}
	}
}
