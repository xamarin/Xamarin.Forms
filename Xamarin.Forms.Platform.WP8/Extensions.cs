using Microsoft.Phone.Controls;

namespace Xamarin.Forms.Platform.WinPhone
{
	internal static class Extensions
	{
		public static ScreenOrientation ToScreenOrientation(this PageOrientation pageOrientation)
		{
			switch (pageOrientation)
			{
				case PageOrientation.Portrait:
				case PageOrientation.PortraitUp:
				case PageOrientation.PortraitDown:
					return ScreenOrientation.Portrait;
				case PageOrientation.Landscape:
				case PageOrientation.LandscapeRight:
				case PageOrientation.LandscapeLeft:
					return ScreenOrientation.Landscape;
				case PageOrientation.None:
					return ScreenOrientation.Unknown;
				default:
					return ScreenOrientation.Other;
			}
		}
	}
}