using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(Default))]
	public class OnAppThemeExtension : IMarkupExtension, INotifyPropertyChanged, IDisposable
	{
		public OnAppThemeExtension()
		{
			Application.Current.RequestedThemeChanged += RequestedThemeChanged;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

		public object Default { get; set; }
		public object Light { get; set; }
		public object Dark { get; set; }

		private object _actualValue;
		public object ActualValue
		{
			get => _actualValue;
			private set
			{
				_actualValue = value;
				OnPropertyChanged();
			}
		}

		public IValueConverter Converter { get; set; }

		public object ConverterParameter { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Default == null
				&& Light == null
				&& Dark == null)
				throw new XamlParseException("OnAppThemeExtension requires a non-null value to be specified for at least one theme or Default.", serviceProvider);

			var valueProvider = serviceProvider?.GetService<IProvideValueTarget>() ?? throw new ArgumentException();

			BindableProperty bp;
			PropertyInfo pi = null;
			Type propertyType = null;

			if (valueProvider.TargetObject is Setter setter)
			{
				bp = setter.Property;
			}
			else
			{
				bp = valueProvider.TargetProperty as BindableProperty;
				pi = valueProvider.TargetProperty as PropertyInfo;
			}
			propertyType = bp?.ReturnType
							  ?? pi?.PropertyType
							  ?? throw new InvalidOperationException("Cannot determine property to provide the value for.");

			var value = GetValue();
			var info = propertyType.GetTypeInfo();
			if (value == null && info.IsValueType)
				ActualValue = Activator.CreateInstance(propertyType);

			if (Converter != null)
				ActualValue = Converter.Convert(value, propertyType, ConverterParameter, CultureInfo.CurrentUICulture);

			var converterProvider = serviceProvider?.GetService<IValueConverterProvider>();
			if (converterProvider != null)
			{
				MemberInfo minforetriever()
				{
					if (pi != null)
						return pi;

					MemberInfo minfo = null;
					try
					{
						minfo = bp.DeclaringType.GetRuntimeProperty(bp.PropertyName);
					}
					catch (AmbiguousMatchException e)
					{
						throw new XamlParseException($"Multiple properties with name '{bp.DeclaringType}.{bp.PropertyName}' found.", serviceProvider, innerException: e);
					}
					if (minfo != null)
						return minfo;
					try
					{
						return bp.DeclaringType.GetRuntimeMethod("Get" + bp.PropertyName, new[] { typeof(BindableObject) });
					}
					catch (AmbiguousMatchException e)
					{
						throw new XamlParseException($"Multiple methods with name '{bp.DeclaringType}.Get{bp.PropertyName}' found.", serviceProvider, innerException: e);
					}
				}

				ActualValue = converterProvider.Convert(value, propertyType, minforetriever, serviceProvider);
			}
			if (converterProvider != null)
				ActualValue = converterProvider.Convert(value, propertyType, () => pi, serviceProvider);

			var ret = value.ConvertTo(propertyType, () => pi, serviceProvider, out Exception exception);
			if (exception != null)
				throw exception;
			ActualValue = ret;

			if (!(value is Binding))
				return new Binding(nameof(ActualValue), source: this);
			else
				return ret;
		}

		public void Dispose()
		{
			Application.Current.RequestedThemeChanged -= RequestedThemeChanged;
		}

		object GetValue()
		{
			switch (Application.Current?.RequestedTheme)
			{
				default:
				case AppTheme.Light:
					return Light ?? Default;
				case AppTheme.Dark:
					return Dark ?? Default;
			}
		}

		void RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
		{
			ActualValue = GetValue();
		}
	}
}