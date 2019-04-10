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
		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			Routing.Clear();
		}

		[Test]
		public async Task GlobalRegisterAbsoluteMatching()
		{
			var shell = new Shell() { RouteScheme = "app", Route = "shellroute" };
			Routing.RegisterRoute("/seg1/seg2/seg3", typeof(object));
			var request = ShellUriHandler.GetNavigationRequest(shell, CreateUri("app://seg1/seg2/seg3"));

			Assert.AreEqual("/seg1/seg2/seg3", request.Request.ShortUri.ToString());
		}

		[Test]
		public async Task ShellContentOnlyWithGlobalEdit()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1");
			var item2 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent2");

			shell.Items.Add(item1);
			shell.Items.Add(item2);

			Routing.RegisterRoute("//rootlevelcontent1/edit", typeof(ContentPage));
			await shell.GoToAsync("//rootlevelcontent1/edit");
		}

		[Test]
		public async Task ShellSectionWithRelativeEdit()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1", shellSectionRoute:"section1");
			var editShellContent = CreateShellContent(shellContentRoute: "edit");


			item1.Items[0].Items.Add(editShellContent);
			shell.Items.Add(item1);

			await shell.GoToAsync("//rootlevelcontent1");
			var location = shell.CurrentState.Location;
			await shell.GoToAsync("edit");

			Assert.AreEqual(editShellContent, shell.CurrentItem.CurrentItem.CurrentItem);
		}


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

			Assert.AreEqual(1, builders.Length);
			Assert.IsTrue(builders.Contains("//section1/rootlevelcontent"));

			builders = ShellUriHandler.GenerateRoutePaths(shell, CreateUri("//section2/rootlevelcontent")).Select(x => x.PathNoImplicit).ToArray();
			Assert.AreEqual(1, builders.Length);
			Assert.IsTrue(builders.Contains("//section2/rootlevelcontent"));
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

			Assert.AreEqual(1, builders.Length);
			Assert.IsTrue(builders.Contains("//item1/rootlevelcontent"));

			builders = ShellUriHandler.GenerateRoutePaths(shell, CreateUri("//item2/rootlevelcontent")).Select(x => x.PathNoImplicit).ToArray();
			Assert.AreEqual(1, builders.Length);
			Assert.IsTrue(builders.Contains("//item2/rootlevelcontent"));
		}


		[Test]
		public async Task ConvertToStandardFormat()
		{
			var shell = new Shell() { RouteScheme = "app", Route = "shellroute", RouteHost = "host" };

			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("//path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("/path")));

			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("host/path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("//host/path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("/host/path")));

			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("shellroute/path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("//shellroute/path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("/shellroute/path")));

			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("host/shellroute/path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("//host/shellroute/path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("/host/shellroute/path")));

			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("app://path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("app:/path")));

			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("app://host/path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("app:/host/path")));

			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("app://shellroute/path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("app:/shellroute/path")));

			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("app://host/shellroute/path")));
			Assert.AreEqual(new Uri("app://host/shellroute/path"), ShellUriHandler.ConvertToStandardFormat(shell, CreateUri("app:/host/shellroute/path")));

		}
	}
}
