#if __ANDROID__
namespace Xamarin.Forms.Platform.Android
#elif __IOS__
namespace Xamarin.Forms.Platform.iOS
#endif
{
	internal static class BindableObjectExtensions
	{

		public static T GetPropertyIfSet<T>(this BindableObject bindableObject, BindableProperty bindableProperty, T returnIfNotSet)
		{
			if (bindableObject == null)
				return returnIfNotSet;

			if (bindableObject.IsSet(bindableProperty))
				return (T)bindableObject.GetValue(bindableProperty);

			return returnIfNotSet;
		}
	}
}