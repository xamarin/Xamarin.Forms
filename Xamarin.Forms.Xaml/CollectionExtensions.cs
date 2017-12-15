using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Xaml
{
	public static class CollectionExtensions
	{
		public static object EnsureCollectionInitialized(this BindableObject bindable, BindableProperty property)
		{
			// Retrieve the value
			var propertyValue = bindable.GetValue(property);

			if (propertyValue != null)
			{
				return propertyValue;
			}

			// If it's not initialized, create it
			propertyValue = CreateValue(property.ReturnType);
			bindable.SetValue(property, propertyValue);

			return propertyValue;
		}

		public static object EnsureCollectionInitialized(this Setter setter)
		{
			// Retrieve the value
			var propertyValue = setter.Value;
			
			if (propertyValue != null)
			{
				return propertyValue;
			}

			// If it's not initialized, create it
			propertyValue = CreateValue(setter.Property.ReturnType);
			setter.Value = propertyValue;

			return propertyValue;
		}

		static object CreateValue(Type type)
		{
			return type == typeof(IList<VisualStateGroup>)
				? new List<VisualStateGroup>()
				: Activator.CreateInstance(type);
		}
	}
}