using System;
#if __ANDROID_29__
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.BottomSheet;
#else
using Android.Support.Design.Widget;
using Android.Support.Design.BottomNavigation;
#endif
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xamarin.Forms.Platform.Android
{
	public static class TabExtensions
	{
		public static int ToAndroidLabelVisibilityMode(this BottomToolBarLabelVisibilityMode label)
		{
			switch (label)
			{
				case BottomToolBarLabelVisibilityMode.Default:
				case BottomToolBarLabelVisibilityMode.Auto:
					return LabelVisibilityMode.LabelVisibilityAuto;
				case BottomToolBarLabelVisibilityMode.Labeled:
					return LabelVisibilityMode.LabelVisibilityLabeled;
				case BottomToolBarLabelVisibilityMode.Selected:
					return LabelVisibilityMode.LabelVisibilitySelected;
				case BottomToolBarLabelVisibilityMode.Unlabeled:
					return LabelVisibilityMode.LabelVisibilityUnlabeled;
				default:
					throw new ArgumentException($"Please make sure that {nameof(label)} is a {nameof(BottomToolBarLabelVisibilityMode)}");
			}
		}
	}
}
