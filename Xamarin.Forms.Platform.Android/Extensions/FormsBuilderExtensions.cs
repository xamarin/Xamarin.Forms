namespace Xamarin.Forms
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithFlags(this IFormsBuilder init, params string[] flags)
		{
			return init.PreInit(() => Forms.SetFlags(flags));
		}
	}
}