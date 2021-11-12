using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using NUnit.Framework;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class RefreshViewTests : BaseTestFixture
	{
		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			Device.PlatformServices = null;
			Device.Info = null;
		}

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			Device.PlatformServices = new MockPlatformServices();
			Device.Info = new TestDeviceInfo();
		}

		[Test]
		public void StartsEnabled()
		{
			RefreshView refreshView = new RefreshView();
			Assert.IsTrue(refreshView.IsEnabled);
			Assert.IsTrue(refreshView.IsRefreshAllowed);
		}

		[Test]
		public void CanExecuteDisablesRefresh()
		{
			RefreshView refreshView = new RefreshView();
			refreshView.Command = new Command(() => { }, () => false);
			Assert.IsTrue(refreshView.IsEnabled);
			Assert.IsFalse(refreshView.IsRefreshAllowed);
		}

		[Test]
		public void CanExecuteEnablesRefresh()
		{
			RefreshView refreshView = new RefreshView();
			refreshView.Command = new Command(() => { }, () => true);
			Assert.IsTrue(refreshView.IsEnabled);
			Assert.IsTrue(refreshView.IsRefreshAllowed);
		}

		[Test]
		public void CanExecuteChangesEnabled()
		{
			RefreshView refreshView = new RefreshView();

			bool canExecute = true;
			var command = new Command(() => { }, () => canExecute);
			refreshView.Command = command;

			canExecute = false;
			command.ChangeCanExecute();
			Assert.IsTrue(refreshView.IsEnabled);
			Assert.IsFalse(refreshView.IsRefreshAllowed);


			canExecute = true;
			command.ChangeCanExecute();
			Assert.IsTrue(refreshView.IsEnabled);
			Assert.IsTrue(refreshView.IsRefreshAllowed);
		}

		[Test]
		public void CommandPropertyChangesRefreshEnabled()
		{
			RefreshView refreshView = new RefreshView();

			bool canExecute = true;
			var command = new Command((p) => { }, (p) => p != null && (bool)p);
			refreshView.CommandParameter = true;
			refreshView.Command = command;

			Assert.IsTrue(refreshView.IsRefreshAllowed);
			refreshView.CommandParameter = false;
			Assert.IsFalse(refreshView.IsRefreshAllowed);
			refreshView.CommandParameter = true;
			Assert.IsTrue(refreshView.IsRefreshAllowed);
		}

		[Test]
		public void CanExecuteDoesNotDisableRefreshView()
		{
			// NOTE: RefreshView.IsEnabled should NOT be set to false based on CanExec to avoid problems like:
			// https://github.com/xamarin/Xamarin.Forms/issues/14350
			RefreshView refreshView = new RefreshView();
			Assert.IsTrue(refreshView.IsEnabled);
			refreshView.Command = new Command(() => { }, () => false);
			Assert.IsTrue(refreshView.IsEnabled);
		}

		[Test]
		public void RemovedCommandEnablesRefresh()
		{
			RefreshView refreshView = new RefreshView();

			bool canExecute = true;
			var command = new Command(() => { }, () => false);
			refreshView.Command = command;
			Assert.IsTrue(refreshView.IsEnabled);
			Assert.IsFalse(refreshView.IsRefreshAllowed);
			refreshView.Command = null;
			Assert.IsTrue(refreshView.IsEnabled);
			Assert.IsTrue(refreshView.IsRefreshAllowed);
			refreshView.Command = command;
			Assert.IsTrue(refreshView.IsEnabled);
			Assert.IsFalse(refreshView.IsRefreshAllowed);
		}

		[Test]
		public void IsRefreshingStaysFalseWithDisabledCommand()
		{
			RefreshView refreshView = new RefreshView();

			bool canExecute = true;
			refreshView.Command = new Command(() => { }, () => false);
			refreshView.IsRefreshing = true;
			Assert.IsFalse(refreshView.IsRefreshing);
		}

		[Test]
		public void IsRefreshingSettableToTrue()
		{
			RefreshView refreshView = new RefreshView();
			Assert.IsFalse(refreshView.IsRefreshing);

			refreshView.IsRefreshing = true;
			Assert.IsTrue(refreshView.IsRefreshing);
		}

		[Test]
		public void IsRefreshingStaysFalseWithDisabledRefreshView()
		{
			RefreshView refreshView = new RefreshView();
			refreshView.IsEnabled = false;
			refreshView.IsRefreshing = true;
			Assert.IsFalse(refreshView.IsRefreshing);
		}

		[Test]
		public void IsRefreshingTogglesFalseWhenIsEnabledSetToFalse()
		{
			RefreshView refreshView = new RefreshView();
			refreshView.IsRefreshing = true;
			refreshView.IsEnabled = false;
			Assert.IsFalse(refreshView.IsRefreshing);
		}

		[Test]
		public void IsRefreshingEventFires()
		{
			RefreshView refreshView = new RefreshView();
			bool eventFired = false;
			refreshView.Refreshing += (_, __) => eventFired = true;
			Assert.IsFalse(eventFired);
			refreshView.IsRefreshing = true;
			Assert.IsTrue(eventFired);
		}
	}
}
