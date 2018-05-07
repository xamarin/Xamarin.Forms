using System;
using System.Globalization;
using System.Reflection;

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

		public IValueConverter Converter { get; set; }

		public object ConverterParameter { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			var lineInfo = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider)?.XmlLineInfo;

			if (Android == null && GTK == null && iOS == null && 
				macOS == null && Tizen == null && UWP == null && 
				WPF == null && Default == null && string.IsNullOrEmpty(Other))
			{
				throw new XamlParseException("OnPlatformExtension requires a value to be specified for at least one platform or Default.", lineInfo ?? new XmlLineInfo());
			}

			var valueProvider = serviceProvider.GetService<IProvideValueTarget>() ?? throw new ArgumentException();

			var bp = valueProvider.TargetProperty as BindableProperty;
			var pi = valueProvider.TargetProperty as PropertyInfo;
			var propertyType = bp?.ReturnType 
				?? pi?.PropertyType 
				?? throw new InvalidOperationException("Cannot determine property to provide the value for.");

			var value = GetValue();
			var info = propertyType.GetTypeInfo();
			if (value == null && info.IsValueType)
				return Activator.CreateInstance(propertyType);

			if (Converter != null)
				return Converter.Convert(value, propertyType, ConverterParameter, CultureInfo.CurrentUICulture);

			var converterProvider = serviceProvider.GetService<IValueConverterProvider>();

			if (converterProvider != null)
				return converterProvider.Convert(value, propertyType, () => pi, serviceProvider);
			else
				return value.ConvertTo(propertyType, () => pi, serviceProvider);
		}

		object GetValue()
		{
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
