namespace Xamarin.Forms
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithVisualMaterial(this IFormsBuilder init)
		{
			return init.PostInit(FormsMaterial.Init);
		}
	}
}