namespace Xamarin.Forms.Xaml.UnitTests.XamlG
{
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using Xamarin.Forms.Build.Tasks;
	using Xamarin.Forms.Core.UnitTests;

	public class XamlGTaskTests : BaseTestFixture
	{
		[Test]
		public void NamespaceUsingsAreGlobal()
		{
			var namesAndTypes = new Dictionary<string, CodeTypeReference>();
			var baseType = new CodeTypeReference(typeof(object));

			CodeCompileUnit code = XamlGTask.GenerateCode("TestType", "MyProject.Xamarin", baseType, namesAndTypes);

			var codeNamespace = code.Namespaces.Cast<CodeNamespace>().First();
			var imports = codeNamespace.Imports.Cast<CodeNamespaceImport>().Select(ns => ns.Namespace).ToArray();
			Assert.Contains("global::System", imports);
			Assert.Contains("global::Xamarin.Forms", imports);
			Assert.Contains("global::Xamarin.Forms.Xaml", imports);
		}
	}
}
