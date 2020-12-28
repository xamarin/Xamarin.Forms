using System;
using Android.Runtime;

using AndroidX.Core.View;
using AndroidX.Core.View.Accessibiity;

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
