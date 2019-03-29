using System;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.View.Accessibility;
using Android.Views;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class NameAndHelpTextAccessibilityDelegate : AccessibilityDelegateCompat
	{

		public string AccessibilityText { get; set; }

		public NameAndHelpTextAccessibilityDelegate()
		{
		}

		protected NameAndHelpTextAccessibilityDelegate(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public override void OnInitializeAccessibilityNodeInfo(global::Android.Views.View host, AccessibilityNodeInfoCompat info)
		{
			base.OnInitializeAccessibilityNodeInfo(host, info);
			info.ContentDescription = AccessibilityText;
		}
	}
}
