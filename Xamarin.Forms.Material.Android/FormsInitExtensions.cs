using Android.Content;
using Android.OS;

namespace Xamarin.Forms
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithVisualMaterial(this IFormsInit init, Context context, Bundle bundle)
		{
			init.PostInit(() => FormsMaterial.Init(context, bundle));
			return init;
		}

		public static IFormsInit WithVisualMaterial(this IFormsInit init)
		{
			init.PostInit(FormsMaterial.Init);
			return init;
		}
	}
}