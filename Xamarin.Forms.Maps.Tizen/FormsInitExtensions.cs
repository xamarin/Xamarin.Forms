using Xamarin.Forms;

namespace Xamarin
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithMaps(this IFormsInit init, string provider, string authenticationToken)
		{
			init.PreInit(() => FormsMaps.Init(provider, authenticationToken));
			return init;
		}
	}
}