namespace Xamarin.Forms
{
	public static class PageExtensions
	{
		public static bool IsLandscape(this Page page)
		{
			return page.Width > 0 && page.Height > 0 && page.Width > page.Height;
		}

		public static bool IsPortrait(this Page page)
		{
			return page.Width > 0 && page.Height > 0 && page.Width < page.Height;
		}
	}
}