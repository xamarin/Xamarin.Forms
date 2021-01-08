using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Platform.iOS.UnitTests
{
	[TestFixture]
	public class DatePickerTests : PlatformTestFixture 
	{
		[Test, Category("DatePicker")]
		[Description("DatePicker should be using wheels-style picker")]
		public async Task UsingWheelPicker() 
		{
			var datePicker = new DatePicker();
			var expected = UIKit.UIDatePickerStyle.Wheels;
			var actual = await GetControlProperty(datePicker, uiDatePicker => uiDatePicker.PreferredDatePickerStyle);
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}