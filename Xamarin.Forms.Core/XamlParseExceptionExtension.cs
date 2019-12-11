using System;
using System.Xml;

namespace Xamarin.Forms.Xaml
{
	internal static class XamlParseExceptionExtension
	{
		internal static IXmlLineInfo GetLineInfo(this IServiceProvider serviceProvider)
			=> (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider) ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
	}
}
