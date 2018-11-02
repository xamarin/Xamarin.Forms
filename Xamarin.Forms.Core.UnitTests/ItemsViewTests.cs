using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class ItemsViewTests : BaseTestFixture
	{
		[SetUp]
		public override void Setup()
		{
			Application.Current = new MockApplication();
			Application.SetFlags(CoreFlags.CollectionViewExperimental);
			base.Setup();
			var mockDeviceInfo = new TestDeviceInfo();
			Device.Info = mockDeviceInfo;
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown ();
			Device.Info = null;
		}

		[Test]
		public void VerticalListMeasurement()
		{
			var itemsView = new ItemsView();

			var sizeRequest = itemsView.Measure(double.PositiveInfinity, double.PositiveInfinity);

			Assert.That(sizeRequest.Request.Height, Is.EqualTo(Device.Info.ScaledScreenSize.Height));
			Assert.That(sizeRequest.Request.Width, Is.EqualTo(Device.Info.ScaledScreenSize.Width));
		}

		[Test]
		public void HorizontalListMeasurement()
		{
			var itemsView = new ItemsView();

			itemsView.ItemsLayout = new ListItemsLayout(ItemsLayoutOrientation.Horizontal);

			var sizeRequest = itemsView.Measure(double.PositiveInfinity, double.PositiveInfinity);

			Assert.That(sizeRequest.Request.Height, Is.EqualTo(Device.Info.ScaledScreenSize.Height));
			Assert.That(sizeRequest.Request.Width, Is.EqualTo(Device.Info.ScaledScreenSize.Width));
		}
	}
}