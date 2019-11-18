using System.Linq;

namespace Xamarin.Forms
{
	public static class FormsBuilderExtensions
	{
		public static IFormsBuilder WithFlags(this IFormsBuilder init, params string[] flags)
		{
			var f = Forms.Flags.ToList();
			f.AddRange(flags);
			return init.PreInit(() => Forms.SetFlags(f.Distinct().ToArray()));
		}
	}
}