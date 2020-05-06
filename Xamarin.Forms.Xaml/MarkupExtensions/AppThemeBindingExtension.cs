using System;
using System.Globalization;
using System.Reflection;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(Default))]
	public class AppThemeBindingExtension : IMarkupExtension<BindingBase>
	{
		public AppThemeBindingExtension()
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(AppThemeBindingExtension), ExperimentalFlags.AppThemeExperimental, nameof(AppThemeBindingExtension));
		}

		public object Default { get; set; }
		public object Light { get; set; }
		public object Dark { get; set; }
		public object Value	{ get; private set;	}

		public object ProvideValue(IServiceProvider serviceProvider) => (this as IMarkupExtension<BindingBase>).ProvideValue(serviceProvider);

		BindingBase IMarkupExtension<BindingBase>.ProvideValue(IServiceProvider serviceProvider)
		{
			if (   Default == null
				&& Light == null
				&& Dark == null)
				throw new XamlParseException("AppThemeBindingExtension requires a non-null value to be specified for at least one theme or Default.", serviceProvider);

			var valueProvider = serviceProvider?.GetService<IProvideValueTarget>() ?? throw new ArgumentException();

			BindableProperty bp;
			PropertyInfo pi = null;
			Type propertyType = null;

			if (valueProvider.TargetObject is Setter setter)
				bp = setter.Property;
			else
			{
				bp = valueProvider.TargetProperty as BindableProperty;
				pi = valueProvider.TargetProperty as PropertyInfo;
			}
			propertyType = bp?.ReturnType
							  ?? pi?.PropertyType
							  ?? throw new InvalidOperationException("Cannot determine property to provide the value for.");

			var converterProvider = serviceProvider?.GetService<IValueConverterProvider>();

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

			if (converterProvider != null)
				return new OnAppTheme<object> {
					Light = converterProvider.Convert(Light, propertyType, minforetriever, serviceProvider),
					Dark = converterProvider.Convert(Dark, propertyType, minforetriever, serviceProvider),
					Default = converterProvider.Convert(Dark, propertyType, minforetriever, serviceProvider)
				};


			var light = Light.ConvertTo(propertyType, minforetriever, serviceProvider, out Exception converterException);
			
			if (converterException != null)
				throw converterException;

			var dark = Dark.ConvertTo(propertyType, minforetriever, serviceProvider, out converterException);

			if (converterException != null)
				throw converterException;

			var @default = Dark.ConvertTo(propertyType, minforetriever, serviceProvider, out converterException);

			if (converterException != null)
				throw converterException;

			return new OnAppTheme<object> { Light = light, Dark = dark, Default = @default };
		}
	}
}