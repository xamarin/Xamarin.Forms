using Xamarin.Forms;
using Xamarin.Forms.Maps.GTK;

namespace Xamarin
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithMaps(this IFormsInit init)
		{
			init.PostInit(FormsMaps.Init);
			return init;
		}

		public static IFormsInit WithMaps(this IFormsInit init, string authenticationToken)
		{
			init.PostInit(() => FormsMaps.Init(authenticationToken));
			return init;
		}
	}
}