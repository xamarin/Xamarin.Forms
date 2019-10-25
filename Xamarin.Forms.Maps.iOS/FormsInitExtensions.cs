using Xamarin.Forms;

namespace Xamarin
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithMaps(this IFormsInit init)
		{
			init.PostInit(FormsMaps.Init);
			return init;
		}
	}
}