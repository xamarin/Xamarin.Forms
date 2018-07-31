namespace Xamarin.Forms.Platform.UWP
{
	internal class ItemTemplatePair
	{
		public ItemTemplatePair(DataTemplate formsDataTemplate, object item)
		{
			FormsDataTemplate = formsDataTemplate;
			Item = item;
		}

		public DataTemplate FormsDataTemplate { get; set; }
		public object Item { get; set; }
	}
}