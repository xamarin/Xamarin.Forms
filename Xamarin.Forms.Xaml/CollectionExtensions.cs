using System;

namespace Xamarin.Forms.Xaml
{
	public static class CollectionExtensions
	{
		public static object EnsureCollectionInitialized(this BindableObject bindable, BindableProperty property)
		{
			// Retrieve the value
			var propertyValue = bindable.GetValue(property);

			// If it's not initialized, create it
			if (propertyValue == null)
			{
				propertyValue = Activator.CreateInstance(property.ReturnType);
				bindable.SetValue(property, propertyValue);
			}

			return propertyValue;
		}

		public static object EnsureCollectionInitialized(this Setter setter)
		{
			// Retrieve the value
			var propertyValue = setter.Value;

			// If it's not initialized, create it
			if (propertyValue == null)
			{
				propertyValue = Activator.CreateInstance(setter.Property.ReturnType);
				setter.Value = propertyValue;
			}

			return propertyValue;
		}
	}
}