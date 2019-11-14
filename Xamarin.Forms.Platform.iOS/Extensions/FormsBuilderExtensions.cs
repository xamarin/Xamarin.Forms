namespace Xamarin.Forms
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithFlags(this IFormsBuilder init, params string[] flags)
		{
			init.PreInit(() => Forms.SetFlags(flags));
			return init;
		}
	}
}