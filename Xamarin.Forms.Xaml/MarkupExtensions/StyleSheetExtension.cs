using System;
using System.Xml;
using Xamarin.Forms.StyleSheets;
using System.Reflection;
using System.IO;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(Style))]
	[ProvideCompiled("Xamarin.Forms.Core.XamlC.StyleSheetProvider")]
	public sealed class StyleSheetExtension : IValueProvider
	{
		public string Style { get; set; }

		[TypeConverter(typeof(UriTypeConverter))]
		public Uri Source { get; set; }

		object IValueProvider.ProvideValue(IServiceProvider serviceProvider)
		{
			if (!string.IsNullOrEmpty(Style) && Source != null)
				throw new XFException(XFException.Ecode.StyleSheet, serviceProvider.GetLineInfo());

			if (Source != null) {
				if (Source.IsAbsoluteUri)
					throw new XFException(XFException.Ecode.RelativeUriOnly, serviceProvider.GetLineInfo(), nameof(Source));

				var rootObjectType = (serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider)?.RootObject.GetType();
				if (rootObjectType == null)
					return null;
				var rootTargetPath = XamlResourceIdAttribute.GetPathForType(rootObjectType);
				var resourcePath = ResourceDictionary.RDSourceTypeConverter.GetResourcePath(Source, rootTargetPath);
				var assembly = rootObjectType.GetTypeInfo().Assembly;

				return StyleSheet.FromResource(resourcePath, assembly, serviceProvider.GetLineInfo());
			}

			if (!string.IsNullOrEmpty(Style)) {
				using (var reader = new StringReader(Style))
					return StyleSheet.FromReader(reader);
			}

			throw new XFException(XFException.Ecode.StyleSheet, serviceProvider.GetLineInfo());
		}
	}
}