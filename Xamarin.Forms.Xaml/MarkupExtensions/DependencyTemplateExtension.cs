using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Xamarin.Forms.Xaml.MarkupExtensions
{
	[ContentProperty(nameof(TypeName))]
	public class DependencyTemplateExtension : IMarkupExtension<DataTemplate>
	{
		public string TypeName { get; set; }

		public DependencyFetchTarget FallbackTarget { get; set; }

		public DataTemplate ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException(nameof(serviceProvider));
			if (!(serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver))
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");
			if (string.IsNullOrEmpty(TypeName))
			{
				var li = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lip) ? lip.XmlLineInfo : new XmlLineInfo();
				throw new XamlParseException("TypeName isn't set.", li);
			}

			object TemplateFactory(Type templateType)
			{
				try
				{
					var templateInstance = DependencyService.Resolve(templateType, FallbackTarget);
					if (templateInstance == null)
					{
						throw new InvalidOperationException($"None template was found for type {templateType} in the DependencyService");
					}
					return templateInstance;
				}
				catch (Exception ex)
				{
					var lineInfo = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider) ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();

					throw new XamlParseException($"DependencyTemplateExtension: Could not load template {templateType} due to an exception, see the inner exception for detail", lineInfo, ex);
				}
			}

			if (typeResolver.TryResolve(TypeName, out var type))
				return new DataTemplate(() => TemplateFactory(type));

			throw new XamlParseException($"DependencyTemplateExtension: Could not locate type for {TypeName}.", GetLineInfo(serviceProvider));
		}

		private static IXmlLineInfo GetLineInfo(IServiceProvider serviceProvider)
		{
			return (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider) ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<DataTemplate>).ProvideValue(serviceProvider);
		}
	}
}
