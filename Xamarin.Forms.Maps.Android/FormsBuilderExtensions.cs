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
			init.PostInit(() => FormsMaps.Init(androidFormsBuilder.Activity.GetActivity(), androidFormsBuilder.Bundle));
			return init;
		}

		public static IFormsBuilder WithMaps(this IFormsBuilder init, Activity activity, Bundle bundle)
		{
			init.PostInit(() => FormsMaps.Init(activity, bundle));
			return init;
		}
	}
}