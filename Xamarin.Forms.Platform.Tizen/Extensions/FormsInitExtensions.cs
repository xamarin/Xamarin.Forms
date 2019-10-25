namespace Xamarin.Forms
{
	public static class FormsInitExtensions
	{
		public static IFormsInit WithFlags(this IFormsInit init, params string[] flags)
		{
			init.PreInit(() => Forms.SetFlags(flags));
			return init;
		}
	}
}