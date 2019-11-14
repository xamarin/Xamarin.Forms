using Android.App;

namespace Xamarin.Forms.Platform.Android.AppLinks
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithAppLinks(this IFormsInit init)
		{
			var androidFormsInit = (AndroidFormsInit)init;
			init.PostInit(() => AndroidAppLinks.Init(androidFormsInit.Activity.GetActivity()));
			return init;
		}

		public static IFormsInit WithAppLinks(this IFormsInit init, Activity activity)
		{
			init.PostInit(() => AndroidAppLinks.Init(activity));
			return init;
		}
	}
}