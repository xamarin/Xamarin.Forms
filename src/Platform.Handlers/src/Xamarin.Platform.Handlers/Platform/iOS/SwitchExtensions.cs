using UIKit;

namespace Xamarin.Platform
{
	public static class SwitchExtensions
	{
		public static void UpdateIsToggled(this UISwitch uiSwitch, ISwitch view)
		{
			uiSwitch.SetState(view.IsToggled, true);
		}

		public static void UpdateOnColor(this UISwitch uiSwitch, ISwitch view)
		{
			uiSwitch.UpdateOnColor(view, null);
		}

		public static void UpdateOnColor(this UISwitch uiSwitch, ISwitch view, UIColor? defaultOnColor)
		{
			if (view == null)
				return;

			if (defaultOnColor == null)
				defaultOnColor = UISwitch.Appearance.OnTintColor;

			if (view.TrackColor == Forms.Color.Default)
				uiSwitch.OnTintColor = defaultOnColor;
			else
				uiSwitch.OnTintColor = view.TrackColor.ToNative();
		}

		public static void UpdateThumbColor(this UISwitch uiSwitch, ISwitch view)
		{
			uiSwitch.UpdateThumbColor(view, null);
		}

		public static void UpdateThumbColor(this UISwitch uiSwitch, ISwitch view, UIColor? defaultThumbColor)
		{
			if (view == null)
				return;

			if (defaultThumbColor == null)
				defaultThumbColor = UISwitch.Appearance.ThumbTintColor;

			Forms.Color thumbColor = view.ThumbColor;
			uiSwitch.ThumbTintColor = thumbColor.IsDefault ? defaultThumbColor : thumbColor.ToNative();
		}
	}
}
