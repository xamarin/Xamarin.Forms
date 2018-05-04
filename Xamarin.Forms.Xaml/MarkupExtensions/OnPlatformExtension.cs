using System;
using System.Xml;

namespace Xamarin.Forms.Xaml
{
	public class OnPlatformExtension : IMarkupExtension
	{
		public object iOS { get; set; }
		public object Android { get; set; } 
		public object UWP { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (iOS == null && Android == null && UWP == null)
			{
				IXmlLineInfoProvider lineInfoProvider = serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider;
				IXmlLineInfo lineInfo = (lineInfoProvider != null) ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
				throw new XamlParseException("OnPlatformExtension requires Value property to be set", lineInfo);
			}

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					return iOS;
				case Device.Android:
					return Android;
				case Device.UWP:
					return UWP;
			}

			return null;
		}
	}
}
