using Android.App;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Xamarin
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithMaps(this IFormsInit init)
		{
			var androidFormsInit = (AndroidFormsInit)init;
			init.PostInit(() => FormsMaps.Init(androidFormsInit.Activity.GetActivity(), androidFormsInit.Bundle));
			return init;
		}

		public static IFormsInit WithMaps(this IFormsInit init, Activity activity, Bundle bundle)
		{
			init.PostInit(() => FormsMaps.Init(activity, bundle));
			return init;
		}
	}
}