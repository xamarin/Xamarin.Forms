
namespace Xamarin.Forms.PlatformConfiguration.WindowsSpecific
{
	using FormsElement = Forms.MasterDetailPage;

	public static class MasterDetailPage
	{
		public static readonly BindableProperty CollapseStyleProperty =
			BindableProperty.CreateAttached("CollapseStyle", typeof(CollapseStyle),
			typeof(MasterDetailPage), CollapseStyle.None);

		public static CollapseStyle GetCollapseStyle(BindableObject element)
		{
			return (CollapseStyle)element.GetValue(CollapseStyleProperty);
		}

		public static void SetCollapseStyle(BindableObject element, CollapseStyle value)
		{
			element.SetValue(CollapseStyleProperty, value);
		}

		public static CollapseStyle GetCollapseStyle(this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			return GetCollapseStyle(config.Element);
		}

		public static IPlatformElementConfiguration<Windows, FormsElement> UseCollapseStyle(this IPlatformElementConfiguration<Windows, FormsElement> config, CollapseStyle value)
		{
			SetCollapseStyle(config.Element, value);
			return config;

		}

		public static IPlatformElementConfiguration<Windows, FormsElement> UsePartialCollapse(
			this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			SetCollapseStyle(config.Element, CollapseStyle.Partial);
			return config;
		}
	}
}
