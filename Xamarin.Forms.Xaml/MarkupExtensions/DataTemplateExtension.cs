using System;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty("TypeName")]
	[ProvideCompiled("Xamarin.Forms.Build.Tasks.DataTemplateExtension")]
	public sealed class DataTemplateExtension : IMarkupExtension<DataTemplate>
	{
		public string TypeName { get; set; }

		public DataTemplate ProvideValue(IServiceProvider serviceProvider)
		{
			if (string.IsNullOrEmpty(TypeName))
				throw new InvalidOperationException("TypeName isn't set.");
			if (serviceProvider == null)
				throw new ArgumentNullException(nameof(serviceProvider));
			if (!(serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver))
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");

			if (typeResolver.TryResolve(TypeName, out var type))
				return new DataTemplate(type);

			var lineInfo = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider) ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
			throw new XamlParseException($"DataTemplateExtension: Could not locate type for {TypeName}.", lineInfo);
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<DataTemplate>).ProvideValue(serviceProvider);
		}
	}
}