using Mono.Cecil;
using NUnit.Framework;
using IOPath = System.IO.Path;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public class MockAssemblyResolver : BaseAssemblyResolver
	{
		public override AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			AssemblyDefinition assembly;
			var localPath = IOPath.GetFullPath(IOPath.Combine(TestContext.CurrentContext.TestDirectory, $"{name.Name}.dll"));
			if (IOPath.Exists(localPath))
				assembly = AssemblyDefinition.ReadAssembly(localPath);
			else
				assembly = base.Resolve(name);
			return assembly;
		}
	}
}