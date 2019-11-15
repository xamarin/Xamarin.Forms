using Android.App;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Xamarin
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithMaps(this IFormsBuilder init)
		{
			var androidFormsBuilder = (AndroidFormsBuilder)init;
			return init.PostInit(() => FormsMaps.Init(androidFormsBuilder.Activity.GetActivity(), androidFormsBuilder.Bundle));
		}

		public static IFormsBuilder WithMaps(this IFormsBuilder init, Activity activity, Bundle bundle)
		{
			return init.PostInit(() => FormsMaps.Init(activity, bundle));
		}
	}
}