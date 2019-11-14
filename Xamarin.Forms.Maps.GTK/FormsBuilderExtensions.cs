using Xamarin.Forms;
using Xamarin.Forms.Maps.GTK;

namespace Xamarin
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithMaps(this IFormsBuilder init)
		{
			init.PreInit(FormsMaps.Init);
			return init;
		}

		public static IFormsBuilder WithMaps(this IFormsBuilder init, string authenticationToken)
		{
			init.PreInit(() => FormsMaps.Init(authenticationToken));
			return init;
		}
	}
}