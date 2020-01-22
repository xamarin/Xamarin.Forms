namespace Xamarin.Forms.Markup
{
	public static class PlatformSpecificsExtensions
	{
		public static T iOSSetDefaultBackgroundColor<T>(this T cell, Color color) where T : Cell
		{
			PlatformConfiguration.iOSSpecific.Cell.SetDefaultBackgroundColor(cell, color);
			return cell;
		}

		public static T iOSSetGroupHeaderStyleGrouped<T>(this T listView) where T : ListView
		{
			PlatformConfiguration.iOSSpecific.ListView.SetGroupHeaderStyle(
				listView,
				PlatformConfiguration.iOSSpecific.GroupHeaderStyle.Grouped
			);
			return listView;
		}
	}
}
