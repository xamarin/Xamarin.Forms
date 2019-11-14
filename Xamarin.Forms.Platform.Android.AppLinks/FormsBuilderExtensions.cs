using Android.App;

namespace Xamarin.Forms.Platform.Android.AppLinks
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithAppLinks(this IFormsBuilder init)
		{
			var androidFormsBuilder = (AndroidFormsBuilder)init;
			init.PostInit(() => AndroidAppLinks.Init(androidFormsBuilder.Activity.GetActivity()));
			return init;
		}

		public static IFormsBuilder WithAppLinks(this IFormsBuilder init, Activity activity)
		{
			init.PostInit(() => AndroidAppLinks.Init(activity));
			return init;
		}
	}
}