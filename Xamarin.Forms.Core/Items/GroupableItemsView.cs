namespace Xamarin.Forms
{
	public class GroupableItemsView : SelectableItemsView
	{
		public static readonly BindableProperty IsGroupingEnabledProperty =
			BindableProperty.Create(nameof(IsGroupingEnabled), typeof(bool), typeof(GroupableItemsView), false);

		public bool IsGroupingEnabled
		{
			get => (bool)GetValue(IsGroupingEnabledProperty);
			set => SetValue(IsGroupingEnabledProperty, value);
		}

		public static readonly BindableProperty GroupHeaderTemplateProperty =
			BindableProperty.Create(nameof(GroupHeaderTemplate), typeof(DataTemplate), typeof(GroupableItemsView));

		public DataTemplate GroupHeaderTemplate
		{
			get => (DataTemplate)GetValue(GroupHeaderTemplateProperty);
			set => SetValue(GroupHeaderTemplateProperty, value);
		}

		public static readonly BindableProperty GroupFooterTemplateProperty =
			BindableProperty.Create(nameof(GroupFooterTemplate), typeof(DataTemplate), typeof(GroupableItemsView));

		public DataTemplate GroupFooterTemplate
		{
			get => (DataTemplate)GetValue(GroupFooterTemplateProperty);
			set => SetValue(GroupFooterTemplateProperty, value);
		}
	}
}
