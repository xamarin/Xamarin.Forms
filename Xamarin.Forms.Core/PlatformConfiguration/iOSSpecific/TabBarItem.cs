namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	public class TabBarItem
	{
		public FileImageSource SelectedImage { get; set; }

		public bool ShouldRemoveImageTint { get; set; }
		public bool ShouldRemoveSelectedImageTint { get; set; }

		public Color TitleTextColor { get; set; }
		public Color SelectedTitleTextColor { get; set; }

		public string BadgeValue { get; set; }
		public Color BadgeColor { get; set; } = Color.Red;

		public Color BadgeTextColor { get; set; } = Color.White;
		public Color SelectedBadgeTextColor { get; set; } = Color.White;
	}
}