using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xamarin.Forms.Platform.Android
{
	internal static class ElevationHelper
	{
		internal static void SetElevation<T>(global::Android.Views.View view, T element) where T : VisualElement
		{
			if (view == null || element == null || !Forms.IsLollipopOrNewer)
			{
				return;
			}

			var iec = element as IElementConfiguration<T>;

			var elevation = iec?.On<PlatformConfiguration.Android>().GetElevation();

			if (!elevation.HasValue)
			{
				return;
			}

			view.Elevation = elevation.Value;
		}
	}
}