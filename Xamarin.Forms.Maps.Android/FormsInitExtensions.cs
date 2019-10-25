using Android.App;
using Android.OS;
using Xamarin.Forms;

namespace Xamarin
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithMaps(this IFormsInit init, Activity activity, Bundle bundle)
		{
			init.PostInit(() => FormsMaps.Init(activity, bundle));
			return init;
		}
	}
}