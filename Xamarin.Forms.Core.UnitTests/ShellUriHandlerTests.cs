using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class ShellUriHandlerTests : ShellTestBase
	{
		[Test]
		public async Task ShellContentOnly()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1");
			var item2 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent2");

			shell.Items.Add(item1);
			shell.Items.Add(item2);


			var builders = ShellUriHandler.GenerateRoutePaths(shell, CreateUri("//rootlevelcontent1"));

			Assert.AreEqual(1, builders.Count);
			Assert.AreEqual("//rootlevelcontent1", builders.First().PathNoImplicit);

			builders = ShellUriHandler.GenerateRoutePaths(shell, CreateUri("//rootlevelcontent2"));
			Assert.AreEqual(1, builders.Count);
			Assert.AreEqual("//rootlevelcontent2", builders.First().PathNoImplicit);
		}


		[Test]
		public async Task ShellSectionAndContentOnly()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent", shellSectionRoute:"section1");
			var item2 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent", shellSectionRoute: "section2");

			shell.Items.Add(item1);
			shell.Items.Add(item2);


			var builders = ShellUriHandler.GenerateRoutePaths(shell, CreateUri("//section1/rootlevelcontent")).Select(x=> x.PathNoImplicit).ToArray();

			Assert.AreEqual(2, builders.Length);
			Assert.IsTrue(builders.Contains("//section1/rootlevelcontent"));
			Assert.IsTrue(builders.Contains("//section1"));

			builders = ShellUriHandler.GenerateRoutePaths(shell, CreateUri("//section2/rootlevelcontent")).Select(x => x.PathNoImplicit).ToArray();
			Assert.IsTrue(builders.Contains("//section2/rootlevelcontent"));
			Assert.IsTrue(builders.Contains("//section2"));
		}

		[Test]
		public async Task ShellItemAndContentOnly()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent", shellItemRoute: "item1");
			var item2 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent", shellItemRoute: "item2");

			shell.Items.Add(item1);
			shell.Items.Add(item2);


			var builders = ShellUriHandler.GenerateRoutePaths(shell, CreateUri("//item1/rootlevelcontent")).Select(x => x.PathNoImplicit).ToArray();

			Assert.AreEqual(2, builders.Length);
			Assert.IsTrue(builders.Contains("//item1/rootlevelcontent"));
			Assert.IsTrue(builders.Contains("//item1"));

			builders = ShellUriHandler.GenerateRoutePaths(shell, CreateUri("//item2/rootlevelcontent")).Select(x => x.PathNoImplicit).ToArray();
			Assert.IsTrue(builders.Contains("//item2/rootlevelcontent"));
			Assert.IsTrue(builders.Contains("//item2"));
		}
	}
}
