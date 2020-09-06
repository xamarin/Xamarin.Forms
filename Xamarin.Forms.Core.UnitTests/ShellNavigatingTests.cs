using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class ShellNavigatingTests : ShellTestBase
	{

		[Test]
		public void CancelNavigation()
		{
			var shell = new Shell();

			var one = new ShellItem { Route = "one" };
			var two = new ShellItem { Route = "two" };

			var tabone = MakeSimpleShellSection("tabone", "content");
			var tabtwo = MakeSimpleShellSection("tabtwo", "content");
			var tabthree = MakeSimpleShellSection("tabthree", "content");
			var tabfour = MakeSimpleShellSection("tabfour", "content");

			one.Items.Add(tabone);
			one.Items.Add(tabtwo);

			two.Items.Add(tabthree);
			two.Items.Add(tabfour);

			shell.Items.Add(one);
			shell.Items.Add(two);

			Assume.That(shell.CurrentState.Location.ToString(), Is.EqualTo("//one/tabone/content"));

			shell.Navigating += (s, e) =>
			{
				e.Cancel();
			};

			shell.GoToAsync(new ShellNavigationState("//two/tabfour/"));

			Assume.That(shell.CurrentState.Location.ToString(), Is.EqualTo("//one/tabone/content"));
		}

		[Test]
		public async Task DeferPopNavigation()
		{
			TestShell shell = new TestShell()
			{
				Items = { CreateShellItem<FlyoutItem>() }
			};


			await shell.Navigation.PushAsync(new ContentPage());
			await shell.Navigation.PushAsync(new ContentPage());

			ShellNavigatingDeferral _token = null;
			shell.Navigating += (_, args) =>
			{
				_token = args.GetDeferral();
			};

			var source = new TaskCompletionSource<object>();
			shell.Navigated += (_, args) =>
			{
				source.SetResult(true);
			};

			await shell.Navigation.PopAsync();
			Assert.AreEqual(3, shell.Navigation.NavigationStack.Count);
			_token.Complete();

			await source.Task;

			Assert.AreEqual(2, shell.Navigation.NavigationStack.Count);
		}

		public async Task DeferredOnlyFiresOnce()
		{
			var shell = new NavigatingShell()
			{
				Items = { CreateShellItem<FlyoutItem>() }
			};

			await shell.Navigation.PushAsync(new ContentPage());
		}


		public class NavigatingShell : TestShell
		{
			public bool Defer { get; set; }
			public ShellNavigatingDeferral DeferToken { get; private set; }

			protected override void OnNavigating(ShellNavigatingEventArgs args)
			{
				base.OnNavigating(args);

				if (Defer)
					DeferToken = args.GetDeferral();
			}
		}
	}
}
