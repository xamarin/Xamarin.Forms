﻿using System;
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
		public async Task RouteWithGlobalPageRoute()
		{

			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellItemRoute: "animals", shellSectionRoute: "domestic", shellContentRoute: "dogs");
			var item2 = CreateShellItem(asImplicit: true, shellItemRoute: "animals", shellSectionRoute: "domestic", shellContentRoute: "cats");

			shell.Items.Add(item1);
			shell.Items.Add(item2);

			Routing.RegisterRoute("catdetails", typeof(ContentPage));
			await shell.GoToAsync("//cats/catdetails?name=3");

			Assert.AreEqual("//animals/domestic/cats/catdetails", shell.CurrentState.Location.ToString());
		}

		[Test]
		public async Task AbsoluteRoutingToPage()
		{

			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellItemRoute: "animals", shellSectionRoute: "domestic", shellContentRoute: "dogs");
			shell.Items.Add(item1);

			Routing.RegisterRoute("catdetails", typeof(ContentPage));

			Assert.That(async () => await shell.GoToAsync($"//catdetails"), Throws.Exception);
		}


		[Test]
		public async Task LocationRemovesImplicit()
		{

			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1");

			shell.Items.Add(item1);

			Assert.AreEqual("//rootlevelcontent1", shell.CurrentState.Location.ToString());
		}


		[Test]
		public async Task GlobalNavigateTwice()
		{

			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1");

			shell.Items.Add(item1);
			Routing.RegisterRoute("cat", typeof(ContentPage));
			Routing.RegisterRoute("details", typeof(ContentPage));

			await shell.GoToAsync("cat");
			await shell.GoToAsync("details");

			Assert.AreEqual("//rootlevelcontent1/cat/details", shell.CurrentState.Location.ToString());
			await shell.GoToAsync("//rootlevelcontent1/details");
			Assert.AreEqual("//rootlevelcontent1/details", shell.CurrentState.Location.ToString());
		}

		[Test]
		public async Task GlobalRoutesRegisteredHierarchicallyNavigateCorrectly()
		{
			Routing.RegisterRoute("first", typeof(TestPage1));
			Routing.RegisterRoute("first/second", typeof(TestPage2));
			Routing.RegisterRoute("first/second/third", typeof(TestPage3));
			var shell = new TestShell(
				CreateShellItem(shellContentRoute: "MainPage")
			);

			await shell.GoToAsync("//MainPage/first/second");

			Assert.AreEqual(typeof(TestPage1), shell.Navigation.NavigationStack[1].GetType());
			Assert.AreEqual(typeof(TestPage2), shell.Navigation.NavigationStack[2].GetType());

			await shell.GoToAsync("//MainPage/first/second/third");

			Assert.AreEqual(typeof(TestPage1), shell.Navigation.NavigationStack[1].GetType());
			Assert.AreEqual(typeof(TestPage2), shell.Navigation.NavigationStack[2].GetType());
			Assert.AreEqual(typeof(TestPage3), shell.Navigation.NavigationStack[3].GetType());
		}

		[Test]
		public async Task GlobalRoutesRegisteredHierarchicallyNavigateCorrectlyVariation()
		{
			Routing.RegisterRoute("monkeys/monkeyDetails", typeof(TestPage1));
			Routing.RegisterRoute("monkeyDetails/monkeygenome", typeof(TestPage2));
			var shell = new TestShell(
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals2"),
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals")
			);

			await shell.GoToAsync("//animals/monkeys/monkeyDetails?id=123");
			await shell.GoToAsync("monkeygenome");
			Assert.AreEqual("//animals/monkeys/monkeyDetails/monkeygenome", shell.CurrentState.Location.ToString());
		}

		[Test]
		public async Task GlobalRoutesRegisteredHierarchicallyWithDoublePop()
		{
			Routing.RegisterRoute("monkeys/monkeyDetails", typeof(TestPage1));
			Routing.RegisterRoute("monkeyDetails/monkeygenome", typeof(TestPage2));
			var shell = new TestShell(
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals2"),
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals")
			);

			await shell.GoToAsync("//animals/monkeys/monkeyDetails?id=123");
			await shell.GoToAsync("monkeygenome");
			await shell.GoToAsync("../..");
			Assert.AreEqual("//animals/monkeys", shell.CurrentState.Location.ToString());
		}

		[Test]
		public async Task GlobalRoutesRegisteredHierarchicallyWithDoubleSplash()
		{
			Routing.RegisterRoute("//animals/monkeys/monkeyDetails", typeof(TestPage1));
			var shell = new TestShell(
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals")
			);

			await shell.GoToAsync("//animals/monkeys/monkeyDetails?id=123");
			Assert.AreEqual("//animals/monkeys/monkeyDetails", shell.CurrentState.Location.ToString());
		}


		[Test]
		public async Task RemovePageWithNestedRoutes()
		{
			Routing.RegisterRoute("monkeys/monkeyDetails", typeof(TestPage1));
			Routing.RegisterRoute("monkeyDetails/monkeygenome", typeof(TestPage2));
			var shell = new TestShell(
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals")
			);

			await shell.GoToAsync("//animals/monkeys/monkeyDetails");
			await shell.GoToAsync("monkeygenome");
			shell.Navigation.RemovePage(shell.Navigation.NavigationStack[1]);
			await shell.Navigation.PopAsync();
		}

		[Test]
		public async Task GlobalRoutesRegisteredHierarchicallyNavigateCorrectlyWithAdditionalItems()
		{
			Routing.RegisterRoute("monkeys/monkeyDetails", typeof(TestPage1));
			Routing.RegisterRoute("monkeyDetails/monkeygenome", typeof(TestPage2));
			var shell = new TestShell(
				CreateShellItem(shellContentRoute: "cats", shellSectionRoute: "domestic", shellItemRoute: "animals")
			);

			shell.Items[0].Items.Add(CreateShellContent(shellContentRoute: "monkeys"));
			shell.Items[0].Items.Add(CreateShellContent(shellContentRoute: "elephants"));
			shell.Items[0].Items.Add(CreateShellContent(shellContentRoute: "bears"));
			shell.Items[0].Items[0].Items.Add(CreateShellContent(shellContentRoute: "dogs"));
			shell.Items.Add(CreateShellContent(shellContentRoute: "about"));
			await shell.GoToAsync("//animals/monkeys/monkeyDetails?id=123");
			await shell.GoToAsync("monkeygenome");
			Assert.AreEqual("//animals/monkeys/monkeyDetails/monkeygenome", shell.CurrentState.Location.ToString());
		}

		[Test]
		public async Task GoBackFromRouteWithMultiplePaths()
		{
			Routing.RegisterRoute("monkeys/monkeyDetails", typeof(TestPage1));

			var shell = new TestShell(
				CreateShellItem()
			);

			await shell.GoToAsync("monkeys/monkeyDetails");
			await shell.GoToAsync("monkeys/monkeyDetails");
			await shell.Navigation.PopAsync();
			await shell.Navigation.PopAsync();
		}


		[Test]
		public async Task GoBackFromRouteWithMultiplePathsHierarchical()
		{
			Routing.RegisterRoute("monkeys/monkeyDetails", typeof(TestPage1));
			Routing.RegisterRoute("monkeyDetails/monkeygenome", typeof(TestPage2));

			var shell = new TestShell(
				CreateShellItem()
			);

			await shell.GoToAsync("monkeys/monkeyDetails");
			await shell.GoToAsync("monkeygenome");
			await shell.Navigation.PopAsync();
			await shell.Navigation.PopAsync();
		}

		[Test]
		public void NodeWalkingBasic()
		{
			var shell = new TestShell(
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals2"),
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals")
			);

			ShellUriHandler.NodeLocation nodeLocation = new ShellUriHandler.NodeLocation();
			nodeLocation.SetNode(shell);

			nodeLocation = nodeLocation.WalkToNextNode();
			Assert.AreEqual(nodeLocation.Content, shell.Items[0].Items[0].Items[0]);

			nodeLocation = nodeLocation.WalkToNextNode();
			Assert.AreEqual(nodeLocation.Content, shell.Items[1].Items[0].Items[0]);
		}


		[Test]
		public void NodeWalkingMultipleContent()
		{
			var shell = new TestShell(
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals1"),
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals2"),
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals3"),
				CreateShellItem(shellContentRoute: "monkeys", shellItemRoute: "animals4")
			);

			var content = CreateShellContent();
			shell.Items[1].Items[0].Items.Add(content);
			shell.Items[2].Items[0].Items.Add(CreateShellContent());

			// add a section with now content
			shell.Items[0].Items.Add(new ShellSection());

			ShellUriHandler.NodeLocation nodeLocation = new ShellUriHandler.NodeLocation();
			nodeLocation.SetNode(content);

			nodeLocation = nodeLocation.WalkToNextNode();
			Assert.AreEqual(shell.Items[2].Items[0].Items[0], nodeLocation.Content);

			nodeLocation = nodeLocation.WalkToNextNode();
			Assert.AreEqual(shell.Items[2].Items[0].Items[1], nodeLocation.Content);

			nodeLocation = nodeLocation.WalkToNextNode();
			Assert.AreEqual(shell.Items[3].Items[0].Items[0], nodeLocation.Content);
		}

		[Test]
		public async Task GlobalRegisterAbsoluteMatching()
		{
			var shell = new Shell();
			Routing.RegisterRoute("/seg1/seg2/seg3", typeof(object));
			var request = ShellUriHandler.GetNavigationRequest(shell, CreateUri("/seg1/seg2/seg3"));

			Assert.AreEqual("app://shell/IMPL_shell/seg1/seg2/seg3", request.Request.FullUri.ToString());
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
		public async Task ShellRelativeGlobalRegistration()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellItemRoute: "item1", shellContentRoute: "rootlevelcontent1", shellSectionRoute: "section1");
			var item2 = CreateShellItem(asImplicit: true, shellItemRoute: "item2", shellContentRoute: "rootlevelcontent1", shellSectionRoute: "section1");

			Routing.RegisterRoute("section0/edit", typeof(ContentPage));
			Routing.RegisterRoute("item1/section1/edit", typeof(ContentPage));
			Routing.RegisterRoute("item2/section1/edit", typeof(ContentPage));
			Routing.RegisterRoute("//edit", typeof(ContentPage));
			shell.Items.Add(item1);
			shell.Items.Add(item2);
			await shell.GoToAsync("//item1/section1/rootlevelcontent1");
			var request = ShellUriHandler.GetNavigationRequest(shell, CreateUri("section1/edit"), true);

			Assert.AreEqual(1, request.Request.GlobalRoutes.Count);
			Assert.AreEqual("item1/section1/edit", request.Request.GlobalRoutes.First());
		}

		[Test]
		public async Task ShellSectionWithRelativeEditUpOneLevelMultiple()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1", shellSectionRoute: "section1");

			Routing.RegisterRoute("section1/edit", typeof(ContentPage));
			Routing.RegisterRoute("section1/add", typeof(ContentPage));

			shell.Items.Add(item1);

			var request = ShellUriHandler.GetNavigationRequest(shell, CreateUri("//rootlevelcontent1/add/edit"));

			Assert.AreEqual(2, request.Request.GlobalRoutes.Count);
			Assert.AreEqual("section1/add", request.Request.GlobalRoutes.First());
			Assert.AreEqual("section1/edit", request.Request.GlobalRoutes.Skip(1).First());
		}

		[Test]
		public async Task ShellSectionWithGlobalRouteAbsolute()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1", shellSectionRoute: "section1");

			Routing.RegisterRoute("edit", typeof(ContentPage));

			shell.Items.Add(item1);

			var request = ShellUriHandler.GetNavigationRequest(shell, CreateUri("//rootlevelcontent1/edit"));

			Assert.AreEqual(1, request.Request.GlobalRoutes.Count);
			Assert.AreEqual("edit", request.Request.GlobalRoutes.First());
		}

		[Test]
		public async Task ShellSectionWithGlobalRouteRelative()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1", shellSectionRoute: "section1");

			Routing.RegisterRoute("edit", typeof(ContentPage));

			shell.Items.Add(item1);

			await shell.GoToAsync("//rootlevelcontent1");
			var request = ShellUriHandler.GetNavigationRequest(shell, CreateUri("edit"));

			Assert.AreEqual(1, request.Request.GlobalRoutes.Count);
			Assert.AreEqual("edit", request.Request.GlobalRoutes.First());
		}

		[TestCase(true, 2)]
		[TestCase(false, 2)]
		[TestCase(true, 3)]
		[TestCase(false, 3)]
		public async Task ShellItemContentRouteWithGlobalRouteRelative(bool modal, int depth)
		{
			var shell = new Shell();
			var item1 = CreateShellItem<FlyoutItem>(asImplicit: true, shellItemRoute: "animals", shellContentRoute: "monkeys");

			string route = "monkeys/details";

			if (depth == 3)
			{
				route = "animals/monkeys/details";
			}

			if (modal)
				Routing.RegisterRoute(route, typeof(ShellModalTests.ModalTestPage));
			else
				Routing.RegisterRoute(route, typeof(ContentPage));

			shell.Items.Add(item1);

			await shell.GoToAsync("details");
			Assert.That(shell.CurrentState.Location.ToString(), Is.EqualTo("//animals/monkeys/details"));
		}

		[TestCase(true)]
		[TestCase(false)]
		public async Task GotoSameGlobalRoutesCollapsesUriCorrectly(bool modal)
		{
			var shell = new Shell();
			var item1 = CreateShellItem<FlyoutItem>(asImplicit: true, shellItemRoute: "animals", shellContentRoute: "monkeys");

			if (modal)
				Routing.RegisterRoute("details", typeof(ShellModalTests.ModalTestPage));
			else
				Routing.RegisterRoute("details", typeof(ContentPage));

			shell.Items.Add(item1);

			await shell.GoToAsync("details");
			await shell.GoToAsync("details");
			Assert.That(shell.CurrentState.Location.ToString(), Is.EqualTo("//animals/monkeys/details/details"));
		}


		[Test]
		public async Task ShellSectionWithRelativeEditUpOneLevel()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1", shellSectionRoute: "section1");

			Routing.RegisterRoute("section1/edit", typeof(ContentPage));

			shell.Items.Add(item1);

			await shell.GoToAsync("//rootlevelcontent1");
			var request = ShellUriHandler.GetNavigationRequest(shell, CreateUri("edit"), true);

			Assert.AreEqual("section1/edit", request.Request.GlobalRoutes.First());
		}

		[Test]
		public async Task ShellSectionWithRelativeEdit()
		{
			var shell = new Shell();
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent1", shellSectionRoute: "section1");
			var editShellContent = CreateShellContent(shellContentRoute: "edit");


			item1.Items[0].Items.Add(editShellContent);
			shell.Items.Add(item1);

			await shell.GoToAsync("//rootlevelcontent1");
			var location = shell.CurrentState.FullLocation;
			await shell.NavigationManager.GoToAsync("edit", false, true);

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
			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent", shellSectionRoute: "section1");
			var item2 = CreateShellItem(asImplicit: true, shellContentRoute: "rootlevelcontent", shellSectionRoute: "section2");

			shell.Items.Add(item1);
			shell.Items.Add(item2);


			var builders = ShellUriHandler.GenerateRoutePaths(shell, CreateUri("//section1/rootlevelcontent")).Select(x => x.PathNoImplicit).ToArray();

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
		public async Task AbsoluteNavigationToRelativeWithGlobal()
		{
			var shell = new Shell();

			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "dogs");
			var item2 = CreateShellItem(asImplicit: true, shellSectionRoute: "domestic", shellContentRoute: "cats", shellItemRoute: "animals");

			shell.Items.Add(item1);
			shell.Items.Add(item2);

			Routing.RegisterRoute("catdetails", typeof(ContentPage));
			await shell.GoToAsync($"//animals/domestic/cats/catdetails?name=domestic");

			Assert.AreEqual(
				"//animals/domestic/cats/catdetails",
				shell.CurrentState.FullLocation.ToString()
				);
		}

		[Test]
		public async Task RelativeNavigationToShellElementThrows()
		{
			var shell = new Shell();

			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "dogs");
			var item2 = CreateShellItem(asImplicit: true, shellSectionRoute: "domestic", shellContentRoute: "cats", shellItemRoute: "animals");

			shell.Items.Add(item1);
			shell.Items.Add(item2);

			Assert.That(async () => await shell.GoToAsync($"domestic"), Throws.Exception);
		}


		[Test]
		public async Task RelativeNavigationWithRoute()
		{
			var shell = new Shell();

			var item1 = CreateShellItem(asImplicit: true, shellContentRoute: "dogs");
			var item2 = CreateShellItem(asImplicit: true, shellSectionRoute: "domestic", shellContentRoute: "cats", shellItemRoute: "animals");

			shell.Items.Add(item1);
			shell.Items.Add(item2);

			Routing.RegisterRoute("catdetails", typeof(ContentPage));
			Assert.That(async () => await shell.GoToAsync($"cats/catdetails?name=domestic"), Throws.Exception);

			// once relative routing with a stack is fixed then we can remove the above exception check and add below back in
			// await shell.GoToAsync($"cats/catdetails?name=domestic")
			//Assert.AreEqual(
			//	"//animals/domestic/cats/catdetails",
			//	shell.CurrentState.Location.ToString()
			//	);

		}

		[Test]
		public async Task ConvertToStandardFormat()
		{
			var shell = new Shell();

			Uri[] TestUris = new Uri[] {
				CreateUri("path"),
				CreateUri("//path"),
				CreateUri("/path"),
				CreateUri("shell/path"),
				CreateUri("//shell/path"),
				CreateUri("/shell/path"),
				CreateUri("IMPL_shell/path"),
				CreateUri("//IMPL_shell/path"),
				CreateUri("/IMPL_shell/path"),
				CreateUri("shell/IMPL_shell/path"),
				CreateUri("//shell/IMPL_shell/path"),
				CreateUri("/shell/IMPL_shell/path"),
				CreateUri("app://path"),
				CreateUri("app:/path"),
				CreateUri("app://shell/path"),
				CreateUri("app:/shell/path"),
				CreateUri("app://shell/IMPL_shell/path"),
				CreateUri("app:/shell/IMPL_shell/path"),
				CreateUri("app:/shell/IMPL_shell\\path")
			};


			foreach (var uri in TestUris)
			{
				Assert.AreEqual(new Uri("app://shell/IMPL_shell/path"), ShellUriHandler.ConvertToStandardFormat(shell, uri), $"{uri}");

				if (!uri.IsAbsoluteUri)
				{
					var reverse = new Uri(uri.OriginalString.Replace("/", "\\"), UriKind.Relative);
					Assert.AreEqual(new Uri("app://shell/IMPL_shell/path"), ShellUriHandler.ConvertToStandardFormat(shell, reverse));
				}

			}
		}
	}
}
