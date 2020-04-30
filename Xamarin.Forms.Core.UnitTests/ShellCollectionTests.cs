using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class ShellCollectionTests : ShellTestBase
	{
		[Test]
		public void ShellContentCollectionChangedDoesntFireWhenEmpty()
		{
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem());
			var shellSection = shell.CurrentItem.CurrentItem;

			bool eventFired = false;
			(shellSection as IShellSectionController).ItemsCollectionChanged += (_, __) =>
			{
				eventFired = true;
			};

			shellSection.Items.RemoveAt(0);
			Assert.IsFalse(eventFired);
			shellSection.Items.Add(CreateShellContent());
			Assert.IsTrue(eventFired);
		}

		[Test]
		public void CollectionChangedDoesntFireWhenCleared()
		{
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem());
			var shellSection = shell.CurrentItem.CurrentItem;

			bool eventFired = false;
			(shellSection as IShellSectionController).ItemsCollectionChanged += (_, __) =>
			{
				eventFired = true;
			};

			shellSection.Items.Clear();
			Assert.IsFalse(eventFired);
			shellSection.Items.Add(CreateShellContent());
			Assert.IsTrue(eventFired);
		}
	}
}
