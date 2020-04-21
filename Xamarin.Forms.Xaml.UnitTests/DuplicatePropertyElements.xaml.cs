using NUnit.Framework;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class DuplicatePropertyElements : BindableObject
	{
		public DuplicatePropertyElements(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public static class Tests
		{
			[TestCase(false)]
			[TestCase(true)]
			public static void ThrowXamlParseException(bool useCompiledXaml)
			{
				Assert.Catch<XamlParseException>(useCompiledXaml ?
					(TestDelegate)(() => MockCompiler.Compile(typeof(DuplicatePropertyElements))) :
					() => new DuplicatePropertyElements(useCompiledXaml));
			}
		}
	}
}
