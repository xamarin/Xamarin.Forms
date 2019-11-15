using Xamarin.Forms;
using Xamarin.Forms.Maps.GTK;

namespace Xamarin
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithMaps(this IFormsBuilder init)
		{
			return init.PreInit(FormsMaps.Init);
		}

		public static IFormsBuilder WithMaps(this IFormsBuilder init, string authenticationToken)
		{
			return init.PreInit(() => FormsMaps.Init(authenticationToken));
		}
	}
}