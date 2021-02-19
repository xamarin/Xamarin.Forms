using UIKit;

namespace Xamarin.Platform
{
	public static class SwitchExtensions
	{
		public static void UpdateIsToggled(this UISwitch uiSwitch, ISwitch view)
		{
			uiSwitch.SetState(view.IsToggled, true);
		}

		public static void UpdateTrackColor(this UISwitch uiSwitch, ISwitch view) =>
			uiSwitch.UpdateTrackColor(view, UISwitch.Appearance.OnTintColor);

		public static void UpdateTrackColor(this UISwitch uiSwitch, ISwitch view, UIColor? defaultOnColor)
		{
			if (view == null)
				return;

			if (view.TrackColor == Forms.Color.Default)
				uiSwitch.OnTintColor = defaultOnColor;
			else
				uiSwitch.OnTintColor = view.TrackColor.ToNative();
		}

		public static void UpdateThumbColor(this UISwitch uiSwitch, ISwitch view) =>
			uiSwitch.UpdateThumbColor(view, UISwitch.Appearance.ThumbTintColor);

		public static void UpdateThumbColor(this UISwitch uiSwitch, ISwitch view, UIColor? defaultThumbColor)
		{
			if (view == null)
				return;

			Forms.Color thumbColor = view.ThumbColor;
			uiSwitch.ThumbTintColor = thumbColor.IsDefault ? defaultThumbColor : thumbColor.ToNative();
		}
	}
}
