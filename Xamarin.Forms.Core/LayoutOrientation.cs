namespace Xamarin.Forms
{
	public enum LayoutOrientation
	{
		Unknown,
		Square,
		Portrait,
		Landscape
	}

	public class PageOrientation
	{
		public Page Page { get; set; }

		public LayoutOrientation LayoutOrientation { get; set; }

		public PageOrientation()
		{
		}

		public PageOrientation(Page page, LayoutOrientation layoutOrientation)
		{
			Page = page;
			LayoutOrientation = layoutOrientation;
		}
	}
}