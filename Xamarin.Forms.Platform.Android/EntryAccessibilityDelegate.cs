using Android.Views.Accessibility;

namespace Xamarin.Forms.Platform.Android
{
	class EntryAccessibilityDelegate : global::Android.Views.View.AccessibilityDelegate
	{
		public string ValueText { get; set; }

		public string ClassName { get; set; } = "android.widget.Button";

		public override void OnInitializeAccessibilityNodeInfo(global::Android.Views.View host, AccessibilityNodeInfo info)
		{
			base.OnInitializeAccessibilityNodeInfo(host, info);
			info.ClassName = ClassName;
			info.Text = ValueText;
		}
	}
}