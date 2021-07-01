using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{

	[TestFixture]
	public class TimerUnitTest : BaseTestFixture
	{
		[Test]
		public void TestTimeSpanOneTime()
		{
			int count = 0;
			Device.StartTimer(TimeSpan.FromMilliseconds(0), () =>
			{
				count++;
				return false;
			});

			Assert.AreEqual(1, count);
		}

		[Test]
		public void TestTimeSpanLoop()
		{
			int count = 0;
			Device.StartTimer(TimeSpan.FromMilliseconds(0), () =>
			{
				count++;
				return count < 10;
			});

			Assert.AreEqual(10, count);
		}
	}
}
