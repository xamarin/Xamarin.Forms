using Xamarin.Forms;

namespace Xamarin
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithMaps(this IFormsBuilder init, string provider, string authenticationToken)
		{
			init.PreInit(() => FormsMaps.Init(provider, authenticationToken));
			return init;
		}
	}
}