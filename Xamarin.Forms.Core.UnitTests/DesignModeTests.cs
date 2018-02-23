using NUnit;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	public class DesignModeTests : BaseTestFixture
	{
		[Test]
		public void DesignModeEnabledIsFalseByDefault()
		{
			Assert.IsFalse(DesignMode.DesignModeEnabled);
		}

		[Test]
		public void DesignModeEnabledIsTrueWhenSet()
		{
			DesignMode.DesignModeEnabled = true;

			Assert.IsTrue(DesignMode.DesignModeEnabled);
		}
	}
}
