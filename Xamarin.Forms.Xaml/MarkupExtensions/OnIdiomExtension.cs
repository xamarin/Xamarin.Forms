using System;

namespace Xamarin.Forms.Xaml
{
	public class OnIdiomExtension : IMarkupExtension
	{
		// See Device.Idiom

		public object Default { get; set; }
		public object Unsupported { get; set; }
		public object Phone { get; set; }
		public object Tablet { get; set; } 
		public object Desktop { get; set; }
		public object TV { get; set; }
		public object Watch { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Default == null && Unsupported == null && Phone == null && 
				Tablet == null && Desktop == null && TV == null && Watch == null)
			{
				var lineInfo = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider)?.XmlLineInfo 
					?? new XmlLineInfo();
				throw new XamlParseException("OnIdiomExtension requires a value to be specified for at least one idiom or Default.", lineInfo);
			}

			switch (Device.Idiom)
			{
				case TargetIdiom.Unsupported:
					return Unsupported ?? Default;
				case TargetIdiom.Phone:
					return Phone ?? Default;
				case TargetIdiom.Tablet:
					return Tablet ?? Default;
				case TargetIdiom.Desktop:
					return Desktop ?? Default;
				case TargetIdiom.TV:
					return TV ?? Default;
				case TargetIdiom.Watch:
					return Watch ?? Default;
				default:
					return Default;
			}
		}
	}
}
