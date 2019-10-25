using Android.Content;
using Android.OS;

namespace Xamarin.Forms
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithMaterial(this IFormsInit init, Context context, Bundle bundle)
		{
			init.PostInit(() => FormsMaterial.Init(context, bundle));
			return init;
		}

		public static IFormsInit WithMaterial(this IFormsInit init)
		{
			init.PostInit(FormsMaterial.Init);
			return init;
		}
	}
}