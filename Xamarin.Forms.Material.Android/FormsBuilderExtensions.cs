using Android.Content;
using Android.OS;

namespace Xamarin.Forms
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithVisualMaterial(this IFormsBuilder init, Context context, Bundle bundle)
		{
			return init.PostInit(() => FormsMaterial.Init(context, bundle));
		}

		public static IFormsBuilder WithVisualMaterial(this IFormsBuilder init)
		{
			return init.PostInit(FormsMaterial.Init);
		}
	}
}