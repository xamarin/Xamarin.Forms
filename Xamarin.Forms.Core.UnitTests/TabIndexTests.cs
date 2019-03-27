using NUnit.Framework;
using System.Linq;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class TabIndexTests : BaseTestFixture
	{
		[Test]
		public void GetFirstTabStop()
		{
			Label target = new Label { TabIndex = 0 };
			Label nextElement = new Label { TabIndex = 1 };
			var stackLayout = new StackLayout
			{
				Children = {
					new Label { TabIndex = 3 },
					target,
					nextElement,
					new Label { TabIndex = 2 },
				}
			};

			var page = new ContentPage { Content = stackLayout };

			var tabIndexes = stackLayout.GetTabIndexesOnParentPage(out int maxAttempts);

			var first = TabIndexExtensions.GetFirstNonLayoutTabStop(tabIndexes);

			Assert.AreEqual(target, first);
		}

		[Test]
		public void GetFirstTabStop_Negative()
		{
			Label target = new Label { TabIndex = -1 };
			Label nextElement = new Label { TabIndex = 1 };
			var stackLayout = new StackLayout
			{
				Children = {
					new Label { TabIndex = 3 },
					target,
					nextElement,
					new Label { TabIndex = 2 },
				}
			};

			var page = new ContentPage { Content = stackLayout };

			var tabIndexes = stackLayout.GetTabIndexesOnParentPage(out int maxAttempts);

			var first = TabIndexExtensions.GetFirstNonLayoutTabStop(tabIndexes);

			Assert.AreEqual(target, first);
		}
	}
}