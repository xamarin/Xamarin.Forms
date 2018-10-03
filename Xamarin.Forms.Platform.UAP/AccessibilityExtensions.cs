using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using NativeAutomationProperties = Windows.UI.Xaml.Automation.AutomationProperties;

namespace Xamarin.Forms.Platform.UWP
{
	public static class AccessibilityExtensions
	{
		public static void SetAutomationPropertiesAutomationId(this FrameworkElement Control, string id)
		{
			Control.SetValue(NativeAutomationProperties.AutomationIdProperty, id);
		}

		public static string SetAutomationPropertiesName(this FrameworkElement Control, Element Element, string _defaultAutomationPropertiesName = null)
		{
			if (Element == null)
				return _defaultAutomationPropertiesName;

			if (_defaultAutomationPropertiesName == null)
				_defaultAutomationPropertiesName = (string)Control.GetValue(NativeAutomationProperties.NameProperty);

			var elemValue = (string)Element.GetValue(AutomationProperties.NameProperty);

			if (!string.IsNullOrWhiteSpace(elemValue))
				Control.SetValue(NativeAutomationProperties.NameProperty, elemValue);
			else
				Control.SetValue(NativeAutomationProperties.NameProperty, _defaultAutomationPropertiesName);

			return _defaultAutomationPropertiesName;
		}

		public static AccessibilityView? SetAutomationPropertiesAccessibilityView(this FrameworkElement Control, Element Element, AccessibilityView? _defaultAutomationPropertiesAccessibilityView = null)
		{
			if (Element == null)
				return _defaultAutomationPropertiesAccessibilityView;

			if (!_defaultAutomationPropertiesAccessibilityView.HasValue)
				_defaultAutomationPropertiesAccessibilityView = (AccessibilityView)Control.GetValue(NativeAutomationProperties.AccessibilityViewProperty);

			var newValue = _defaultAutomationPropertiesAccessibilityView;

			var elemValue = (bool?)Element.GetValue(AutomationProperties.IsInAccessibleTreeProperty);

			if (elemValue == true)
				newValue = AccessibilityView.Content;
			else if (elemValue == false)
				newValue = AccessibilityView.Raw;

			Control.SetValue(NativeAutomationProperties.AccessibilityViewProperty, newValue);

			return _defaultAutomationPropertiesAccessibilityView;

		}
		public static string SetAutomationPropertiesHelpText(this FrameworkElement Control, Element Element, string _defaultAutomationPropertiesHelpText = null)
		{
			if (Element == null)
				return _defaultAutomationPropertiesHelpText;

			if (_defaultAutomationPropertiesHelpText == null)
				_defaultAutomationPropertiesHelpText = (string)Control.GetValue(NativeAutomationProperties.HelpTextProperty);

			var elemValue = (string)Element.GetValue(AutomationProperties.HelpTextProperty);

			if (!string.IsNullOrWhiteSpace(elemValue))
				Control.SetValue(NativeAutomationProperties.HelpTextProperty, elemValue);
			else
				Control.SetValue(NativeAutomationProperties.HelpTextProperty, _defaultAutomationPropertiesHelpText);

			return _defaultAutomationPropertiesHelpText;
		}

		public static UIElement SetAutomationPropertiesLabeledBy(this FrameworkElement Control, Element Element, UIElement _defaultAutomationPropertiesLabeledBy = null)
		{
			if (Element == null)
				return _defaultAutomationPropertiesLabeledBy;

			if (_defaultAutomationPropertiesLabeledBy == null)
				_defaultAutomationPropertiesLabeledBy = (UIElement)Control.GetValue(NativeAutomationProperties.LabeledByProperty);

			var elemValue = (VisualElement)Element.GetValue(AutomationProperties.LabeledByProperty);

			var renderer = elemValue?.GetOrCreateRenderer();

			var nativeElement = renderer?.GetNativeElement();

			if (nativeElement != null)
				Control.SetValue(AutomationProperties.LabeledByProperty, nativeElement);
			else
				Control.SetValue(NativeAutomationProperties.LabeledByProperty, _defaultAutomationPropertiesLabeledBy);

			return _defaultAutomationPropertiesLabeledBy;
		}

	}
}
