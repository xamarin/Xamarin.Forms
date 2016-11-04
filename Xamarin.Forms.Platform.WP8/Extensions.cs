using WPageOrientation = Microsoft.Phone.Controls.PageOrientation;

namespace Xamarin.Forms.Platform.WinPhone
{
	internal static class Extensions
	{
		public static ScreenOrientation ToScreenOrientation(this WPageOrientation pageOrientation)
		{
			switch (pageOrientation)
			{
				case WPageOrientation.Portrait:
				case WPageOrientation.PortraitUp:
				case WPageOrientation.PortraitDown:
					return ScreenOrientation.Portrait;
				case WPageOrientation.Landscape:
				case WPageOrientation.LandscapeRight:
				case WPageOrientation.LandscapeLeft:
					return ScreenOrientation.Landscape;
				case WPageOrientation.None:
					return ScreenOrientation.Unknown;
				default:
					return ScreenOrientation.Other;
			}
		}
	}
}