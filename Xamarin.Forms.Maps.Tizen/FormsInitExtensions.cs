using Xamarin.Forms;

namespace Xamarin
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithMaps(this IFormsInit init, string provider, string authenticationToken)
		{
			init.PostInit(() => FormsMaps.Init(provider, authenticationToken));
			return init;
		}
	}
}