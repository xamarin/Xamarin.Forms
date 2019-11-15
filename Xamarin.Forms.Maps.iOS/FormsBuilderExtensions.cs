using Xamarin.Forms;

namespace Xamarin
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithMaps(this IFormsBuilder init)
		{
			return init.PostInit(FormsMaps.Init);
		}
	}
}