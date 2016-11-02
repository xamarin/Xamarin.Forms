namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.TableView;

	public static class TableView
	{
		public static readonly BindableProperty SectionHeaderDividerBackgroundColorProperty = BindableProperty.Create(nameof(SectionHeaderDividerBackgroundColor), typeof(Color?), typeof(TableView));

		public static Color? GetSectionHeaderDividerBackgroundColor(BindableObject element)
		{
			return (Color?)element.GetValue(SectionHeaderDividerBackgroundColorProperty);
		}

		public static void SetSectionHeaderDividerBackgroundColor(BindableObject element, Color? value)
		{
			element.SetValue(SectionHeaderDividerBackgroundColorProperty, value);
		}

		public static Color? SectionHeaderDividerBackgroundColor(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetSectionHeaderDividerBackgroundColor(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetSectionHeaderDividerBackgroundColor(this IPlatformElementConfiguration<Android, FormsElement> config, Color? value)
		{
			SetSectionHeaderDividerBackgroundColor(config.Element, value);
			return config;
		}

		public static readonly BindableProperty SectionDividerBackgroundColorProperty = BindableProperty.Create(nameof(SectionDividerBackgroundColor), typeof(Color?), typeof(TableView));

		public static Color? GetSectionDividerBackgroundColor(BindableObject element)
		{
			return (Color?)element.GetValue(SectionDividerBackgroundColorProperty);
		}

		public static void SetSectionDividerBackgroundColor(BindableObject element, Color? value)
		{
			element.SetValue(SectionDividerBackgroundColorProperty, value);
		}

		public static Color? SectionDividerBackgroundColor(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetSectionDividerBackgroundColor(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetSectionDividerBackgroundColor(this IPlatformElementConfiguration<Android, FormsElement> config, Color? value)
		{
			SetSectionDividerBackgroundColor(config.Element, value);
			return config;
		}

		public static readonly BindableProperty DividerBackgroundColorProperty = BindableProperty.Create(nameof(DividerBackgroundColor), typeof(Color?), typeof(TableView));

		public static Color? GetDividerBackgroundColor(BindableObject element)
		{
			return (Color?)element.GetValue(DividerBackgroundColorProperty);
		}

		public static void SetDividerBackgroundColor(BindableObject element, Color? value)
		{
			element.SetValue(DividerBackgroundColorProperty, value);
		}

		public static Color? DividerBackgroundColor(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetDividerBackgroundColor(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetDividerBackgroundColor(this IPlatformElementConfiguration<Android, FormsElement> config, Color? value)
		{
			SetDividerBackgroundColor(config.Element, value);
			return config;
		}

		public static readonly BindableProperty DividerHeightProperty = BindableProperty.Create(nameof(DividerHeight), typeof(int?), typeof(TableView));

		public static int? GetDividerHeight(BindableObject element)
		{
			return (int?)element.GetValue(DividerHeightProperty);
		}

		public static void SetDividerHeight(BindableObject element, int? value)
		{
			element.SetValue(DividerHeightProperty, value);
		}

		public static int? DividerHeight(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetDividerHeight(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetDividerHeight(this IPlatformElementConfiguration<Android, FormsElement> config, int? value)
		{
			SetDividerHeight(config.Element, value);
			return config;
		}
	}
}