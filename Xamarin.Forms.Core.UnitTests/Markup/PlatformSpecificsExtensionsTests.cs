using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests.iOS
{
	using PlatformConfiguration.iOSSpecific;

	[TestFixture]
	public class PlatformSpecificsExtensionsTests : MarkupBaseTestFixture
	{
		[Test]
		public void iOSSetDefaultBackgroundColor()
		{
			var cell = new TextCell();
			Assume.That(Cell.GetDefaultBackgroundColor(cell), Is.Not.EqualTo(Color.Gold));
			TextCell _ = cell.iOSSetDefaultBackgroundColor(Color.Gold);
			Assert.That(Cell.GetDefaultBackgroundColor(cell), Is.EqualTo(Color.Gold));
		}

		[Test]
		public void iOSSetGroupHeaderStyleGrouped()
		{
			var listView = new Forms.ListView();
			Assume.That(ListView.GetGroupHeaderStyle(listView), Is.Not.EqualTo(GroupHeaderStyle.Grouped));
			listView.iOSSetGroupHeaderStyleGrouped();
			Assert.That(ListView.GetGroupHeaderStyle(listView), Is.EqualTo(GroupHeaderStyle.Grouped));
		}

		[Test]
		public void SupportDerived()
		{
			DerivedFromListView _ =
				new DerivedFromListView()
					.iOSSetGroupHeaderStyleGrouped();
		}

		class DerivedFromListView : Forms.ListView { }
	}
}