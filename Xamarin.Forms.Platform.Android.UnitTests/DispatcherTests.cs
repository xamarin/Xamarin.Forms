using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Platform.Android.UnitTests
{
	[TestFixture]
	public class DispatcherTests : PlatformTestFixture
	{
		[Test]
		[Description("Can retrieve the thread ID of an IDispatcher")]
		public async Task CanGetDispatcherThreadId()
		{
			var label = new Label { Text = "foo" };

			await Device.InvokeOnMainThreadAsync(() => {
				GetRenderer(label);
			});

			Assert.That(label.Dispatcher.GetThreadId(), Is.GreaterThan(0));
		}

		[Test]
		[Description("Dispatcher thread Ids match")]
		public async Task DispatcherThreadIdsMatch()
		{
			var label = new Label { Text = "foo" };

			var a = label.Dispatcher;

			Assert.That(a.GetThreadId(), Is.GreaterThan(0));

			await Device.InvokeOnMainThreadAsync(() => {
				GetRenderer(label);
			});

			var b = label.Dispatcher;

			Assert.That(b.GetThreadId(), Is.GreaterThan(0));

			Assert.That(a.GetThreadId(), Is.EqualTo(b.GetThreadId()));
		}
	}
}