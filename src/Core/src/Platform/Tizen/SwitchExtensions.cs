using ElmSharp;
using Tizen.UIExtensions.ElmSharp;

namespace Microsoft.Maui
{
	public static class SwitchExtensions
	{
		public static void UpdateIsToggled(this Check nativeCheck, ISwitch view)
		{
			nativeCheck.IsChecked = view.IsToggled;
		}

		public static void UpdateTrackColor(this Check nativeCheck, ISwitch view)
		{
			//TODO: need to consider default color
			nativeCheck.Color = view.TrackColor.ToNativeEFL();
		}

		public static void UpdateThumbColor(this Check nativeCheck, ISwitch view)
		{
			if (view.ThumbColor.IsDefault)
			{
				nativeCheck.DeleteOnColors();
			}
			else
			{
				nativeCheck.SetOnColors(view.ThumbColor.ToNativeEFL());
			}
		}
	}
}