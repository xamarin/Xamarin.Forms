using Android.App;

namespace Xamarin.Forms.Platform.Android.AppLinks
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithAppLinks(this IFormsBuilder init)
		{
			var androidFormsBuilder = (AndroidFormsBuilder)init;
			return init.PostInit(() => AndroidAppLinks.Init(androidFormsBuilder.Activity.GetActivity()));
		}

		public static IFormsBuilder WithAppLinks(this IFormsBuilder init, Activity activity)
		{
			return init.PostInit(() => AndroidAppLinks.Init(activity));
		}
	}
}