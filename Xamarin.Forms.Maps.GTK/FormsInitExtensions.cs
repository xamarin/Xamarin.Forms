using Xamarin.Forms;
using Xamarin.Forms.Maps.GTK;

namespace Xamarin
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithMaps(this IFormsInit init)
		{
			init.PreInit(FormsMaps.Init);
			return init;
		}

		public static IFormsInit WithMaps(this IFormsInit init, string authenticationToken)
		{
			init.PreInit(() => FormsMaps.Init(authenticationToken));
			return init;
		}
	}
}