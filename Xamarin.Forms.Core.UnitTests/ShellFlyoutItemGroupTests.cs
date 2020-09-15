using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class ShellFlyoutItemGroupTests : ShellTestBase
	{
		[Test]
		public void FlyoutGroupsNumbersForDifferentFlyoutDisplayOptions()
		{
			var shell = new Shell();
			var shellItem = new ShellItem() { FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems, };
			var shellItem2 = new ShellItem();
			var shellSection1 = CreateShellSection(new ContentPage());
			var shellSection2 = CreateShellSection(new ContentPage());
			var shellSection3 = CreateShellSection(new ContentPage(), asImplicit: true);
			var shellSection4 = CreateShellSection(new ContentPage());

			shellItem.Items.Add(shellSection1);
			shellItem.Items.Add(shellSection2);
			shellItem2.Items.Add(shellSection3);
			shellItem2.Items.Add(shellSection4);

			shell.Items.Add(shellItem);
			shell.Items.Add(shellItem2);
			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();

			Assert.AreEqual(groups.Count, 2);
			Assert.AreEqual(groups[0].Count, 2);
			Assert.AreEqual(groups[1].Count, 1);
		}

		[Test]
		public void FlyoutGroupsNumbersForFlyoutDisplayOptionsAsMultipleItems()
		{
			var shell = new Shell();
			var shellItem = new ShellItem() { FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems, };
			var shellItem2 = new ShellItem() { FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems, };

			shellItem.Items.Add(CreateShellSection());
			shellItem.Items.Add(CreateShellSection());
			shellItem2.Items.Add(CreateShellSection(asImplicit: true));
			shellItem2.Items.Add(CreateShellSection());

			shell.Items.Add(shellItem);
			shell.Items.Add(shellItem2);
			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();

			Assert.AreEqual(groups.Count, 2);
			Assert.AreEqual(groups[0].Count, 2);
			Assert.AreEqual(groups[1].Count, 2);
		}

		[Test]
		public void FlyoutGroupsNumbersForFlyoutDisplayOptionsAsSingleItems()
		{
			var shell = new Shell();
			var shellItem = new ShellItem() { FlyoutDisplayOptions = FlyoutDisplayOptions.AsSingleItem, };
			var shellItem2 = new ShellItem() { FlyoutDisplayOptions = FlyoutDisplayOptions.AsSingleItem, };
			var shellSection1 = CreateShellSection(new ContentPage());
			var shellSection2 = CreateShellSection(new ContentPage());
			var shellSection3 = CreateShellSection(new ContentPage(), asImplicit: true);
			var shellSection4 = CreateShellSection(new ContentPage());

			shellItem.Items.Add(shellSection1);
			shellItem.Items.Add(shellSection2);
			shellItem2.Items.Add(shellSection3);
			shellItem2.Items.Add(shellSection4);


			shell.Items.Add(shellItem);
			shell.Items.Add(shellItem2);
			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();

			Assert.AreEqual(groups.Count, 1);
			Assert.AreEqual(groups[0].Count, 2);
		}


		[Test]
		public void FlyoutDisabledItemsDontGenerateIntoFlyoutList()
		{
			var shell = new TestShell();
			var shellItemFlyoutDisabled = CreateShellItem<ShellItem>();
			Shell.SetFlyoutBehavior(shellItemFlyoutDisabled, FlyoutBehavior.Disabled);

			var flyoutItem = CreateShellItem<FlyoutItem>();
			flyoutItem.FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems;
			Shell.SetFlyoutBehavior(flyoutItem.Items[0], FlyoutBehavior.Disabled);

			var shellContent = CreateShellContent();
			Shell.SetFlyoutBehavior(shellContent, FlyoutBehavior.Disabled);

			shell.Items.Add(shellItemFlyoutDisabled);
			shell.Items.Add(flyoutItem);
			shell.Items.Add(shellContent);

			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();
			Assert.AreEqual(groups.Count, 0);
		}

		[Test]
		public void MenuItemGeneratesForShellContent()
		{
			var shell = new TestShell();

			var shellContent = CreateShellContent();
			shellContent.MenuItems.Add(new MenuItem());
			shell.Items.Add(shellContent);

			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();
			Assert.AreEqual(groups.SelectMany(x => x.OfType<IMenuItemController>()).Count(), 1);
		}


		[Test]
		public void MenuItemGeneratesForShellSection()
		{
			var shell = new TestShell();

			var shellSection = CreateShellSection<Tab>();
			shellSection.CurrentItem.MenuItems.Add(new MenuItem());
			shellSection.FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems;
			shell.Items.Add(shellSection);

			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();
			Assert.AreEqual(1, groups.SelectMany(x => x.OfType<IMenuItemController>()).Count());
		}


		[Test]
		public void FlyoutItemVisibleWorksForMenuItemsAddedAsShellItem()
		{
			var shell = new TestShell();
			var item = new MenuShellItem(CreateNonVisibleMenuItem());
			shell.Items.Add(item);

			var itemsAreEquals = item.Equals(shell.Items[0]);
			Assert.IsTrue(itemsAreEquals);

			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();

			var r = groups.SelectMany(x => x.OfType<IMenuItemController>());

			Assert.AreEqual(r.Count(), 0);
		}

		[Test]
		public void FlyoutItemVisibleWorksForMenuItemsAddedAsTab()
		{
			var shell = new TestShell();

			var shellSection = CreateShellSection<Tab>();
			shellSection.Items[0].MenuItems.Add(CreateNonVisibleMenuItem());
			shell.Items.Add(shellSection);

			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();
			Assert.AreEqual(groups.SelectMany(x => x.OfType<IMenuItemController>()).Count(), 0);
		}

		[Test]
		public void FlyoutItemVisibleWorksForMenuItemsAddedAsShellContent()
		{
			var shell = new TestShell();

			var shellContent = CreateShellContent();
			shellContent.MenuItems.Add(CreateNonVisibleMenuItem());
			shell.Items.Add(shellContent);

			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();
			Assert.AreEqual(groups.SelectMany(x => x.OfType<IMenuItemController>()).Count(), 0);
		}

		[Test]
		public void FlyoutItemVisibleWorksForMenuItemsFlyoutItemAsMultipleItems()
		{
			var shell = new TestShell();

			var flyoutItem = CreateShellItem<FlyoutItem>();
			flyoutItem.FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems;
			flyoutItem.CurrentItem.CurrentItem.MenuItems.Add(CreateNonVisibleMenuItem());
			shell.Items.Add(flyoutItem);


			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();
			Assert.AreEqual(groups.SelectMany(x => x.OfType<IMenuItemController>()).Count(), 0);
		}

		[Test]
		public void FlyoutItemVisibleWorksForMenuItemsTabAsMultipleItems()
		{
			var shell = new TestShell();

			var flyoutItem = CreateShellItem<FlyoutItem>();
			flyoutItem.FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems;
			flyoutItem.CurrentItem.FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems;
			flyoutItem.CurrentItem.CurrentItem.MenuItems.Add(CreateNonVisibleMenuItem());
			shell.Items.Add(flyoutItem);

			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();
			Assert.AreEqual(0, groups.SelectMany(x => x.OfType<IMenuItemController>()).Count());
		}

		[Test]
		public void FlyoutItemNotVisibleWhenShellContentSetToNotVisible()
		{
			var shell = new TestShell();
			var shellSection = CreateShellSection();
			shellSection.FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems;
			shellSection.Items.Add(CreateShellContent());
			shellSection.Items[0].IsVisible = false;
			shell.Items.Add(shellSection);

			IShellController shellController = (IShellController)shell;
			var groups = shellController.GenerateFlyoutGrouping();
			Assert.AreEqual(1, groups.Count);
			Assert.AreEqual(1, groups[0].Count);
		}


		MenuItem CreateNonVisibleMenuItem()
		{
			MenuItem item = new MenuItem();
			FlyoutItem.SetIsVisible(item, false);
			return item;
		}
	}
}
