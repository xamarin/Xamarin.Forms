namespace Xamarin.Forms
{
	public static class FormsInitExtensions
	{

		public static IFormsInit WithMaterial(this IFormsInit init)
		{
			init.PostInit(FormsMaterial.Init);
			return init;
		}
	}
}