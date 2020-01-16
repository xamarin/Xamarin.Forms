using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class BrushUnitTests : BaseTestFixture
	{
		[Test]
		[TestCase("linear-gradient(90deg, rgb(255, 0, 0),rgb(255, 153, 51))")]
		[TestCase("radial-gradient(circle, rgb(255, 0, 0) 25%, rgb(0, 255, 0) 50%, rgb(0, 0, 255) 75%)")]
		public void TestColorTypeConverter(string parameter)
		{
			var converter = new BrushTypeConverter();
			Assert.True(converter.CanConvertFrom(typeof(string)));
			Assert.NotNull(converter.ConvertFromInvariantString(parameter));
		}
	}
}