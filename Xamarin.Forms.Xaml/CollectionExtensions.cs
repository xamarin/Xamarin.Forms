namespace Xamarin.Forms.Xaml
{
	public static class CollectionExtensions
	{
		public static object EnsureSetterCollectionInitialized(this Setter setter)
		{
			// Retrieve the value
			var propertyValue = setter.Value;

			if (propertyValue != null)
			{
				return propertyValue;
			}

			// If it's not initialized, create it
			propertyValue = setter.Property.DefaultValueCreator.Invoke(null);
			setter.Value = propertyValue;

			return propertyValue;
		}
	}
}