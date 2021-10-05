using System;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	internal class AutomationPropertiesProvider : IDisposable
	{
		static readonly string s_defaultDrawerId = "drawer";
		static readonly string s_defaultDrawerIdOpenSuffix = "_open";
		static readonly string s_defaultDrawerIdCloseSuffix = "_close";

		internal static void GetDrawerAccessibilityResources(global::Android.Content.Context context, FlyoutPage page, out int resourceIdOpen, out int resourceIdClose)
		{
			resourceIdOpen = 0;
			resourceIdClose = 0;
			if (page == null)
				return;

			var automationIdParent = s_defaultDrawerId;
			var icon = page.Flyout?.IconImageSource;
			if (icon != null && !icon.IsEmpty)
				automationIdParent = page.Flyout.IconImageSource.AutomationId;
			else if (!string.IsNullOrEmpty(page.AutomationId))
				automationIdParent = page.AutomationId;

			resourceIdOpen = context.Resources.GetIdentifier($"{automationIdParent}{s_defaultDrawerIdOpenSuffix}", "string", context.ApplicationInfo.PackageName);
			resourceIdClose = context.Resources.GetIdentifier($"{automationIdParent}{s_defaultDrawerIdCloseSuffix}", "string", context.ApplicationInfo.PackageName);
		}

		static bool ShoudISetImportantForAccessibilityToNoIfAutomationIdIsSet(AView control, Element element)
		{
			if (!Flags.IsAccessibilityExperimentalSet())
				return false;

			if (element == null)
				return false;

			if (String.IsNullOrWhiteSpace(element.AutomationId))
				return false;

			// User has specifically said what they want
			if (AutomationProperties.GetIsInAccessibleTree(element) == true)
				return false;

			// User has explicitly set name and help text so we honor that
			if (!String.IsNullOrWhiteSpace(ConcatenateNameAndHelpText(element)))
				return false;

			if (element is Layout ||
				element is ItemsView ||
				element is BoxView ||
				element is ScrollView ||
				element is TableView ||
				element is WebView ||
				element is Page ||
				element is Shapes.Path ||
				element is Frame ||
				element is ListView ||
				element is Image)
			{
				return true;
			}

			return false;
		}

		internal static void SetAutomationId(AView control, Element element, string value = null)
		{
			if (!control.IsAlive())
			{
				return;
			}

			string automationId = value ?? element?.AutomationId;
			if (!string.IsNullOrEmpty(automationId))
			{
				control.ContentDescription = automationId;

				if (ShoudISetImportantForAccessibilityToNoIfAutomationIdIsSet(control, element))
				{
					control.ImportantForAccessibility = ImportantForAccessibility.No;
				}
				else if (Flags.IsAccessibilityExperimentalSet() && control.GetAccessibilityDelegate() == null)
					ViewCompat.SetAccessibilityDelegate(control, new AccessibilityDelegateAutomationId(element));

			}
		}

		internal static void SetBasicContentDescription(
			AView control,
			Element element,
			string defaultContentDescription)
		{
			if (element == null || control == null)
				return;

			string value = ConcatenateNameAndHelpText(element);

			var contentDescription = !string.IsNullOrWhiteSpace(value) ? value : defaultContentDescription;

			if (String.IsNullOrWhiteSpace(contentDescription))
			{
				if (Flags.IsAccessibilityExperimentalSet())
				{
					SetAutomationId(control, element, element.AutomationId);
					return;
				}

				contentDescription = element.AutomationId;
			}

			if (Flags.IsAccessibilityExperimentalSet())
			{
				if (!AutomationProperties.GetIsInAccessibleTree(element).HasValue)
				{
					control.ImportantForAccessibility = ImportantForAccessibility.Auto;
				}
			}

			control.ContentDescription = contentDescription;
		}

		internal static void SetContentDescription(
			AView control,
			Element element,
			string defaultContentDescription,
			string defaultHint)
		{
			if (element == null || control == null || SetHint(control, element, defaultHint))
				return;

			SetBasicContentDescription(control, element, defaultContentDescription);
		}

		internal static void SetFocusable(AView control, Element element, ref bool? defaultFocusable, ref ImportantForAccessibility? defaultImportantForAccessibility)
		{
			if (element == null || control == null)
			{
				return;
			}

			if (!defaultFocusable.HasValue)
			{
				defaultFocusable = control.Focusable;
			}

			if (!defaultImportantForAccessibility.HasValue)
			{
				// Auto is the default just use that
				if (Flags.IsAccessibilityExperimentalSet())
					defaultImportantForAccessibility = ImportantForAccessibility.Auto;
				else
					defaultImportantForAccessibility = control.ImportantForAccessibility;
			}

			bool? isInAccessibleTree = (bool?)element.GetValue(AutomationProperties.IsInAccessibleTreeProperty);

			if (!isInAccessibleTree.HasValue && Flags.IsAccessibilityExperimentalSet())
				return;

			control.Focusable = (bool)(isInAccessibleTree ?? defaultFocusable);
			control.ImportantForAccessibility = !isInAccessibleTree.HasValue ? (ImportantForAccessibility)defaultImportantForAccessibility : (bool)isInAccessibleTree ? ImportantForAccessibility.Yes : ImportantForAccessibility.No;
		}

		internal static void SetLabeledBy(AView control, Element element)
		{
			if (element == null || control == null)
				return;

			var elemValue = (VisualElement)element.GetValue(AutomationProperties.LabeledByProperty);

			if (elemValue != null)
			{
				var id = control.Id;
				if (id == AView.NoId)
					id = control.Id = Platform.GenerateViewId();

				var renderer = elemValue?.GetRenderer();
				renderer?.SetLabelFor(id);
			}
		}

		static bool SetHint(AView Control, BindableObject Element, string defaultHint)
		{
			if (Element == null || Control == null)
			{
				return false;
			}

			if (Element is Picker || Element is Button)
			{
				return false;
			}

			var textView = Control as TextView;
			if (textView == null)
			{
				return false;
			}

			// TODO: add EntryAccessibilityDelegate to Entry
			// Let the specified Placeholder take precedence, but don't set the ContentDescription (won't work anyway)
			if ((Element as Entry)?.Placeholder != null)
			{
				return true;
			}

			string value = ConcatenateNameAndHelpText(Element);

			textView.Hint = !string.IsNullOrWhiteSpace(value) ? value : defaultHint;

			return true;
		}

		string _defaultContentDescription;
		bool? _defaultFocusable;
		ImportantForAccessibility? _defaultImportantForAccessibility;
		string _defaultHint;
		bool _disposed;

		IVisualElementRenderer _renderer;

		public AutomationPropertiesProvider(IVisualElementRenderer renderer)
		{
			_renderer = renderer;
			_renderer.ElementPropertyChanged += OnElementPropertyChanged;
			_renderer.ElementChanged += OnElementChanged;
		}

		AView Control => _renderer?.View;

		VisualElement Element => _renderer?.Element;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (Element != null)
			{
				Element.PropertyChanged -= OnElementPropertyChanged;
			}

			if (_renderer != null)
			{
				_renderer.ElementChanged -= OnElementChanged;
				_renderer.ElementPropertyChanged -= OnElementPropertyChanged;

				_renderer = null;
			}
		}

		void SetContentDescription()
			=> SetContentDescription(Control, Element, _defaultContentDescription, _defaultHint);

		void SetFocusable()
			=> SetFocusable(Control, Element, ref _defaultFocusable, ref _defaultImportantForAccessibility);

		void SetLabeledBy()
			=> SetLabeledBy(Control, Element);

		internal static void AccessibilitySettingsChanged(AView control, Element element, string _defaultHint, string _defaultContentDescription, ref bool? _defaultFocusable, ref ImportantForAccessibility? _defaultImportantForAccessibility)
		{
			SetHint(control, element, _defaultHint);
			SetAutomationId(control, element);
			SetContentDescription(control, element, _defaultContentDescription, _defaultHint);
			SetFocusable(control, element, ref _defaultFocusable, ref _defaultImportantForAccessibility);
			SetLabeledBy(control, element);
		}

		internal static void AccessibilitySettingsChanged(AView control, Element element)
		{
			string _defaultHint = String.Empty;
			string _defaultContentDescription = String.Empty;
			bool? _defaultFocusable = null;
			ImportantForAccessibility? _defaultImportantForAccessibility = null;
			AccessibilitySettingsChanged(control, element, _defaultHint, _defaultContentDescription, ref _defaultFocusable, ref _defaultImportantForAccessibility);
		}


		internal static string ConcatenateNameAndHelpText(BindableObject Element)
		{
			var name = (string)Element.GetValue(AutomationProperties.NameProperty);
			var helpText = (string)Element.GetValue(AutomationProperties.HelpTextProperty);

			if (string.IsNullOrWhiteSpace(name))
				return helpText;
			if (string.IsNullOrWhiteSpace(helpText))
				return name;

			return $"{name}. {helpText}";
		}

		internal static void SetupDefaults(AView control, ref string defaultContentDescription)
		{
			string hint = null;
			SetupDefaults(control, ref defaultContentDescription, ref hint);
		}

		internal static void SetupDefaults(AView control, ref string defaultContentDescription, ref string defaultHint)
		{
			// Setting defaults for these values makes no sense
			if (!Flags.IsAccessibilityExperimentalSet())
			{
				if (defaultContentDescription == null)
					defaultContentDescription = control.ContentDescription;

				if (control is TextView textView && defaultHint == null)
				{
					defaultHint = textView.Hint;
				}
			}
		}

		bool _defaultsSet;
		void SetupDefaults()
		{
			if (_defaultsSet || Control == null)
				return;

			_defaultsSet = true;
			SetupDefaults(Control, ref _defaultHint, ref _defaultContentDescription);
		}

		void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.NewElement != null)
			{
				e.NewElement.PropertyChanged += OnElementPropertyChanged;
			}

			SetupDefaults();
			AccessibilitySettingsChanged(Control, Element, _defaultHint, _defaultContentDescription, ref _defaultFocusable, ref _defaultImportantForAccessibility);
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == AutomationProperties.HelpTextProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == AutomationProperties.NameProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == AutomationProperties.IsInAccessibleTreeProperty.PropertyName)
			{
				SetFocusable();
			}
			else if (e.PropertyName == AutomationProperties.LabeledByProperty.PropertyName)
			{
				SetLabeledBy();
			}
		}
	}
}