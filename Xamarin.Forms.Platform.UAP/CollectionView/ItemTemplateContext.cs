namespace Xamarin.Forms.Platform.UWP
{
	internal class ItemTemplateContext
	{
		public ItemTemplateContext(DataTemplate formsDataTemplate, object item, BindableObject container, double? height = null, double? width = null, Thickness? itemSpacing = null)
		{
			s_count += 1;

			Report(100);

			FormsDataTemplate = formsDataTemplate;
			Item = item;
			Container = container;

			if (height.HasValue)
				ItemHeight = height.Value;

			if (width.HasValue)
				ItemWidth = width.Value;

			if (itemSpacing.HasValue)
				ItemSpacing = itemSpacing.Value;
		}

		public DataTemplate FormsDataTemplate { get; }
		public object Item { get; }
		public BindableObject Container { get; }
		public double ItemHeight { get; }
		public double ItemWidth { get; }
		public Thickness ItemSpacing { get; }

		static int s_count;

		~ItemTemplateContext()
		{
			s_count -= 1;
			Report(100);
		}

		public static void Report()
		{
			System.Diagnostics.Debug.WriteLine($">>>>>> ItemTemplateContext allocated = {s_count}");
		}

		public static void Report(int interval)
		{
			if (s_count % interval == 0)
			{
				System.Diagnostics.Debug.WriteLine($">>>>>> ItemTemplateContext allocated = {s_count}");
			}
		}
	}
}