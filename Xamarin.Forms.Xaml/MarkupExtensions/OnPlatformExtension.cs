using System;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty("Default")]
	public class OnPlatformExtension : IMarkupExtension
	{
		public object Default { get; set; }
		public object Android { get; set; }
		public object GTK { get; set; }
		public object iOS { get; set; }
		public object macOS { get; set; }
		public object Tizen { get; set; }
		public object UWP { get; set; }
		public object WPF { get; set; }
		public string Other { get; set; }


		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Android == null && GTK == null && iOS == null && 
				macOS == null && Tizen == null && UWP == null && 
				WPF == null && Default == null && string.IsNullOrEmpty(Other))
			{
				var lineInfo = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider)?.XmlLineInfo 
					?? new XmlLineInfo();
				throw new XamlParseException("OnPlatformExtension requires a value to be specified for at least one platform or Default.", lineInfo);
			}

			switch (Device.RuntimePlatform)
			{
				case Device.Android:
					return Android ?? Default;
				case Device.GTK:
					return GTK ?? Default;
				case Device.iOS:
					return iOS ?? Default;
				case Device.macOS:
					return macOS ?? Default;
				case Device.Tizen:
					return Tizen ?? Default;
				case Device.UWP:
					return UWP ?? Default;
				case Device.WPF:
					return WPF ?? Default;
				default:
					if (string.IsNullOrEmpty(Other))
						return Default;

					var others = Other.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var other in others)
					{
						var pair = other.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
						if (pair.Length != 2)
							continue;

						if (Device.RuntimePlatform == pair[0])
							return pair[1];
					}

					return Default;
			}
		}
	}
}
