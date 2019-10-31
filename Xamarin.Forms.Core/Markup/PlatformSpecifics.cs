namespace Xamarin.Forms.Markup
{
	public static class PlatformSpecificsExtensions
	{
		public static T iOSSetDefaultBackgroundColor<T>(this T cell, Color color) where T : Cell
		{
			Xamarin.Forms.PlatformConfiguration.iOSSpecific.Cell.SetDefaultBackgroundColor(cell, color);
			return cell;
		}

		public static T iOSSetGroupHeaderStyleGrouped<T>(this T listView) where T : ListView
		{
			Xamarin.Forms.PlatformConfiguration.iOSSpecific.ListView.SetGroupHeaderStyle(
				listView,
				Xamarin.Forms.PlatformConfiguration.iOSSpecific.GroupHeaderStyle.Grouped
			);
			return listView;
		}
	}
}
