using System.Windows.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	internal static class Extensions
	{
		public static DeviceOrientation ToDeviceOrientation(this System.Windows.Window page)
		{
		    return page.Height> page.Width ? DeviceOrientation.Portrait : DeviceOrientation.Landscape;
		}
	}
}