using Xamarin.Forms;

namespace Xamarin
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithMaps(this IFormsBuilder init)
		{
			init.PostInit(FormsMaps.Init);
			return init;
		}

		public static IFormsBuilder WithMaps(this IFormsBuilder init, string authenticationToken)
		{
			init.PostInit(() => FormsMaps.Init(authenticationToken));
			return init;
		}
	}
}