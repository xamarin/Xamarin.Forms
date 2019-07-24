using System;

namespace Xamarin.Forms.Controls.XamStore
{
	public static class Icons
	{
		public static string FontFamily
		{
			get
			{
				switch (Device.RuntimePlatform)
				{
					case Device.iOS:
						return "Ionicons";
					case Device.UWP:
						return "Assets/Fonts/ionicons.ttf#ionicons";
					case Device.Android:
					default:
						return "fonts/ionicons.ttf#";
				}
			}
		}
		
		public const string Card = "\uf119";
		public const string Add = "\uf2c7";
		public const string Heart = "\uf199";
		public const string Music = "\uf20c";
		public const string Octocat = "\uf233";
		public const string Game = "\uf13c";
	}
}