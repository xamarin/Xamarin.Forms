using Android.Content;
using Android.OS;

namespace Xamarin.Forms
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithVisualMaterial(this IFormsBuilder init, Context context, Bundle bundle)
		{
			init.PostInit(() => FormsMaterial.Init(context, bundle));
			return init;
		}

		public static IFormsBuilder WithVisualMaterial(this IFormsBuilder init)
		{
			init.PostInit(FormsMaterial.Init);
			return init;
		}
	}
}