using Android.Views.Accessibility;
using AndroidX.Core.View.Accessibility;
using Xamarin.Forms.Platform.Android.FastRenderers;

namespace Xamarin.Forms.Platform.Android
{
	class EntryAccessibilityDelegate : AccessibilityDelegateAutomationId
	{
		BindableObject _element;

		public EntryAccessibilityDelegate(BindableObject element) : base(element)
		{
			_element = element;
		}

		protected override void Dispose(bool disposing)
		{
			_element = null;
			base.Dispose(disposing);
		}

		public string ValueText { get; set; }

		public string ClassName { get; set; } = "android.widget.Button";

		public override void OnInitializeAccessibilityNodeInfo(global::Android.Views.View host, AccessibilityNodeInfoCompat info)
		{
			base.OnInitializeAccessibilityNodeInfo(host, info);
			info.ClassName = ClassName;
			if (_element != null)
			{
				var value = string.IsNullOrWhiteSpace(ValueText) ? string.Empty : $"{ValueText}. ";
				host.ContentDescription = $"{value}{AutomationPropertiesProvider.ConcatenateNameAndHelpText(_element)}";
			}
		}
	}
}