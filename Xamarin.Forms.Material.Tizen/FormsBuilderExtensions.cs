namespace Xamarin.Forms
{
	public static class FormsBuilderExtensions
	{

		public static IFormsBuilder WithVisualMaterial(this IFormsBuilder init)
		{
			init.PostInit(FormsMaterial.Init);
			return init;
		}
	}
}