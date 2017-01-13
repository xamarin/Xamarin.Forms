#if __MOBILE__
using UIKit;
using Xamarin.Forms.Maps.iOS;
#else
using Xamarin.Forms.Maps.MacOS;
#endif

namespace Xamarin
{
	public static class FormsMaps
	{
		static bool s_isInitialized;
#if __MOBILE__
		static bool? s_isiOs10OrNewer;

		internal static bool IsiOs10OrNewer
		{
			get
			{
				if (!s_isiOs10OrNewer.HasValue)
					s_isiOs10OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(10, 0);
				return s_isiOs10OrNewer.Value;
			}
		}
#endif
		public static void Init()
		{
			if (s_isInitialized)
				return;
			GeocoderBackend.Register();
			s_isInitialized = true;
		}
	}
}