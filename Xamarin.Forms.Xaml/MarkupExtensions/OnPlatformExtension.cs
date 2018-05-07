using System;

namespace Xamarin.Forms.Xaml
{
	public class OnPlatformExtension : IMarkupExtension
	{
		public object Default { get; set; }
		public object iOS { get; set; }
		public object Android { get; set; } 
		public object UWP { get; set; }
		public string Other { get; set; }


		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (iOS == null && Android == null && UWP == null && Default == null && string.IsNullOrEmpty(Other))
			{
				var lineInfo = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider)?.XmlLineInfo 
					?? new XmlLineInfo();
				throw new XamlParseException("OnPlatformExtension requires a value to be specified for at least one platform or Default.", lineInfo);
			}

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					return iOS;
				case Device.Android:
					return Android;
				case Device.UWP:
					return UWP;
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
