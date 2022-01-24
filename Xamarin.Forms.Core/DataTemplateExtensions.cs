using System;
using System.ComponentModel;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class DataTemplateExtensions
	{
		public static DataTemplate SelectDataTemplate(this DataTemplate self, object item, BindableObject container)
		{
			var selector = self as DataTemplateSelector;
			if (selector == null)
				return self;

			return selector.SelectTemplate(item, container);
		}

		public static object CreateContent(this DataTemplate self, object item, BindableObject container)
		{
			return self.SelectDataTemplate(item, container)?.CreateContent();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public  static class DataTemplateHelpers
	{
		public readonly static DataTemplate DefaultContentTemplate =
			new Lazy<DataTemplate>(
				() => new DataTemplate(CreateCoreContent)).Value;

		static View CreateCoreContent()
		{
			var label = new Label();
			label.SetBinding(Label.TextProperty, ".");

			return new StackLayout()
			       {
				       HorizontalOptions = LayoutOptions.Center,
				       VerticalOptions = LayoutOptions.Center,
				       Children = { label }
			       };
		}
	}
}