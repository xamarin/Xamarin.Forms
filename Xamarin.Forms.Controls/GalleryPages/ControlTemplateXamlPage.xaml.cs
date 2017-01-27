namespace Xamarin.Forms.Controls.GalleryPages
{
	public partial class ControlTemplateXamlPage : ContentPage
	{
		public static readonly BindableProperty AboveTextProperty =
			BindableProperty.Create(nameof(AboveText), typeof(string), typeof(ControlTemplateXamlPage), null);

		public ControlTemplateXamlPage()
		{
			BindingContext = new
			{
				Text = "Testing 123"
			};
			this.SetBinding(AboveTextProperty, "Text");
			InitializeComponent();
		}

		public string AboveText
		{
			get { return (string)GetValue(AboveTextProperty); }
			set { SetValue(AboveTextProperty, value); }
		}
	}
}