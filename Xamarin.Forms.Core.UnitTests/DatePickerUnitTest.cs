using System;

using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class DatePickerUnitTest : BaseTestFixture
	{
		[Test]
		public void TestMinimumDateException ()
		{
			DatePicker picker = new DatePicker ();

			picker.MinimumDate = new DateTime (1950, 1, 1);

			Assert.AreEqual (new DateTime (1950, 1, 1), picker.MinimumDate);

			Assert.That (() => picker.MinimumDate = new DateTime (2200, 1, 1), Throws.ArgumentException);
		}

		[Test]
		public void TestMaximumDateException ()
		{
			DatePicker picker = new DatePicker ();
			
			picker.MaximumDate = new DateTime (2050, 1, 1);
			
			Assert.AreEqual (new DateTime (2050, 1, 1), picker.MaximumDate);

			Assert.That (() => picker.MaximumDate = new DateTime (1800, 1, 1), Throws.ArgumentException);
		}

		[Test]
		public void TestMaximumDateClamping ()
		{
			DatePicker picker = new DatePicker ();

			picker.Date = new DateTime (2050, 1, 1);

			Assert.AreEqual (new DateTime (2050, 1, 1), picker.Date);
			Assert.AreEqual(new DateTime(2050, 1, 1), picker.SelectedDate);

			bool dateChanged = false;
			bool selectedDateChanged = false;
			bool maximumDateChanged = false;
			picker.PropertyChanged += (sender, e) => {
				switch (e.PropertyName) {
				case "MaximumDate":
					maximumDateChanged = true;
					break;
				case "Date":
					dateChanged = true;
					Assert.IsFalse (maximumDateChanged);
					break;
				case nameof(DatePicker.SelectedDate):
					selectedDateChanged = true;
					Assert.IsFalse(maximumDateChanged);
					break;
				}
			};

			var newDate = new DateTime (2000, 1, 1);
			picker.MaximumDate = newDate;

			Assert.IsTrue (maximumDateChanged);
			Assert.IsTrue (dateChanged);
			Assert.IsTrue (selectedDateChanged);

			Assert.AreEqual (newDate, picker.MaximumDate);
			Assert.AreEqual (newDate, picker.Date);
			Assert.AreEqual (newDate, picker.SelectedDate);
			Assert.AreEqual (picker.MaximumDate, picker.Date);
		}

		[Test]
		public void SelectedDateShouldStayNullWhenMaximumDateIsChanged()
		{
			DatePicker picker = new DatePicker();
			picker.SelectedDate = null;
			Assert.IsNull(picker.SelectedDate);

			var newDate = ((DateTime)DatePicker.MinimumDateProperty.DefaultValue).AddDays(1);
			Assert.IsTrue(picker.Date > newDate);

			bool dateChanged = false;
			bool selectedDateChanged = false;
			bool maximumDateChanged = false;
			picker.PropertyChanged += (sender, e) => {
				switch (e.PropertyName)
				{
					case nameof(DatePicker.MaximumDate):
						maximumDateChanged = true;
						break;
					case nameof(DatePicker.Date):
						dateChanged = true;
						Assert.IsFalse(maximumDateChanged);
						break;
					case nameof(DatePicker.SelectedDate):
						selectedDateChanged = true;
						Assert.IsFalse(maximumDateChanged);
						break;
				}
			};

			picker.MaximumDate = newDate;

			Assert.IsTrue(maximumDateChanged);
			Assert.IsTrue(dateChanged);
			Assert.IsFalse(selectedDateChanged);

			Assert.AreEqual(newDate, picker.MaximumDate);
			Assert.AreEqual(newDate, picker.Date);
			Assert.IsNull(picker.SelectedDate);
			Assert.AreEqual(picker.MaximumDate, picker.Date);
		}

		[Test]
		public void TestMinimumDateClamping ()
		{
			DatePicker picker = new DatePicker ();
			
			picker.Date = new DateTime (1950, 1, 1);
			
			Assert.AreEqual (new DateTime (1950, 1, 1), picker.Date);
			
			bool dateChanged = false;
			bool selectedDateChanged = false;
			bool minimumDateChanged = false;
			picker.PropertyChanged += (sender, e) => {
				switch (e.PropertyName) {
				case "MinimumDate":
					minimumDateChanged = true;
					break;
				case "Date":
					dateChanged = true;
					Assert.IsFalse (minimumDateChanged);
					break;
				case nameof(DatePicker.SelectedDate):
					selectedDateChanged = true;
					Assert.IsFalse(minimumDateChanged);
					break;
				}
			};
			
			var newDate = new DateTime (2000, 1, 1);
			picker.MinimumDate = newDate;
			
			Assert.IsTrue (minimumDateChanged);
			Assert.IsTrue (dateChanged);
			Assert.IsTrue (selectedDateChanged);

			Assert.AreEqual (newDate, picker.MinimumDate);
			Assert.AreEqual (newDate, picker.Date);
			Assert.AreEqual (newDate, picker.SelectedDate);
			Assert.AreEqual (picker.MinimumDate, picker.Date);
		}

		[Test]
		public void SelectedDateShouldStayNullWhenMinimumDateIsChanged()
		{
			DatePicker picker = new DatePicker();
			picker.SelectedDate = null;
			Assert.IsNull(picker.SelectedDate);

			var newDate = ((DateTime)DatePicker.MaximumDateProperty.DefaultValue).AddDays(-1);
			Assert.IsTrue(picker.Date < newDate);

			bool dateChanged = false;
			bool selectedDateChanged = false;
			bool minDateChanged = false;
			picker.PropertyChanged += (sender, e) => {
				switch (e.PropertyName)
				{
					case nameof(DatePicker.MinimumDate):
						minDateChanged = true;
						break;
					case nameof(DatePicker.Date):
						dateChanged = true;
						Assert.IsFalse(minDateChanged);
						break;
					case nameof(DatePicker.SelectedDate):
						selectedDateChanged = true;
						Assert.IsFalse(minDateChanged);
						break;
				}
			};

			picker.MinimumDate = newDate;

			Assert.IsTrue(minDateChanged);
			Assert.IsTrue(dateChanged);
			Assert.IsFalse(selectedDateChanged);

			Assert.AreEqual(newDate, picker.MinimumDate);
			Assert.AreEqual(newDate, picker.Date);
			Assert.IsNull(picker.SelectedDate);
			Assert.AreEqual(picker.MinimumDate, picker.Date);
		}

		[Test]
		public void TestDateClamping ()
		{
			DatePicker picker = new DatePicker ();

			picker.Date = new DateTime (1500, 1, 1);

			Assert.AreEqual (picker.MinimumDate, picker.Date);
			Assert.AreEqual (picker.MinimumDate, picker.SelectedDate);

			picker.Date = new DateTime (2500, 1, 1);

			Assert.AreEqual (picker.MaximumDate, picker.Date);
			Assert.AreEqual (picker.MaximumDate, picker.SelectedDate);
		}

		[Test]
		public void TestDateClampingWhenSelectedDateIsNull()
		{
			DatePicker picker = new DatePicker();

			picker.Date = new DateTime(1500, 1, 1);
			picker.SelectedDate = null;

			Assert.AreEqual(picker.MinimumDate, picker.Date);
			Assert.IsNull(picker.SelectedDate);

			picker.Date = new DateTime(2500, 1, 1);
			picker.SelectedDate = null;

			Assert.AreEqual(picker.MaximumDate, picker.Date);
			Assert.IsNull(picker.SelectedDate);
		}

		[Test]
		public void TestDateSelected ()
		{
			var picker = new DatePicker ();

			bool selected = false;
			bool selectedDateChanged = false;
			picker.DateSelected += (sender, arg) => selected = true;
			picker.SelectedDateChanged += (sender, arg) => selectedDateChanged = true;

			// we can be fairly sure it wont ever be 2008 again
			picker.Date = new DateTime (2008, 5, 5);

			Assert.True (selected);
			Assert.True (selectedDateChanged);
		}

		[Test]
		public void TestSelectedDate()
		{
			var picker = new DatePicker();

			bool selected = false;
			bool selectedDateChanged = false;
			picker.DateSelected += (sender, arg) => selected = true;
			picker.SelectedDateChanged += (sender, arg) => selectedDateChanged = true;

			// we can be fairly sure it wont ever be 2008 again
			picker.SelectedDate = new DateTime(2008, 5, 5);

			Assert.True(selected);
			Assert.True(selectedDateChanged);
		}

		public static object[] DateTimes = {
			new object[] { new DateTime (2006, 12, 20), new DateTime (2011, 11, 30) },
			new object[] { new DateTime (1900, 1, 1), new DateTime (1999, 01, 15) }, // Minimum Date
			new object[] { new DateTime (2006, 12, 20), new DateTime (2100, 12, 31) } // Maximum Date
		};

		[Test, TestCaseSource(nameof(DateTimes))]
		public void DatePickerSelectedEventArgs (DateTime initialDate, DateTime finalDate)
		{
			var datePicker = new DatePicker ();
			datePicker.Date = initialDate;

			DatePicker pickerFromSender = null;
			DateTime oldDate = new DateTime ();
			DateTime newDate = new DateTime ();

			datePicker.DateSelected += (s, e) => {
				pickerFromSender = (DatePicker)s;
				oldDate = e.OldDate;
				newDate = e.NewDate;
			};

			datePicker.Date = finalDate;

			Assert.AreEqual (datePicker, pickerFromSender);
			Assert.AreEqual (initialDate, oldDate);
			Assert.AreEqual (finalDate, newDate);
		}

		[Test, TestCaseSource("DateTimes")]
		public void DatePickerDateSelectedEventArgs(DateTime initialDate, DateTime finalDate)
		{
			var datePicker = new DatePicker();
			datePicker.Date = initialDate;

			DatePicker pickerFromSender = null;
			DateTime? oldDate = new DateTime();
			DateTime? newDate = new DateTime();

			datePicker.SelectedDateChanged += (s, e) => {
				pickerFromSender = (DatePicker)s;
				oldDate = e.OldDate;
				newDate = e.NewDate;
			};

			datePicker.SelectedDate = finalDate;

			Assert.AreEqual(datePicker, pickerFromSender);
			Assert.AreEqual(initialDate, oldDate);
			Assert.AreEqual(finalDate, newDate);
		}

		[Test]
		//https://bugzilla.xamarin.com/show_bug.cgi?id=32144
		public void SetNullValueDoesNotThrow ()
		{
			var datePicker = new DatePicker ();
			Assert.DoesNotThrow (() => datePicker.SetValue (DatePicker.DateProperty, null));
			Assert.AreEqual (DateTime.Today, datePicker.Date);
			Assert.AreEqual (DateTime.Today, datePicker.SelectedDate);
		}

		[Test]
		public void SetNullableDateTime ()
		{
			var datePicker = new DatePicker ();
			var dateTime = new DateTime (2015, 7, 21);
			DateTime? nullableDateTime = dateTime;
			datePicker.SetValue (DatePicker.DateProperty, nullableDateTime);
			Assert.AreEqual (dateTime, datePicker.Date);
			Assert.AreEqual (dateTime, datePicker.SelectedDate);
		}

		[Test]
		public void SelectedDateIsSyncedWithDate()
		{
			var dp = new DatePicker();
			Assert.AreEqual (dp.SelectedDate, dp.Date);

			dp = new DatePicker();
			dp.Date = dp.Date.AddDays(1);
			Assert.AreEqual(dp.SelectedDate, dp.Date);

			dp = new DatePicker();
			dp.SelectedDate = dp.SelectedDate?.AddDays(1);
			Assert.AreEqual(dp.SelectedDate, dp.Date);

			// Setting a new SelectedDate after SelectedDate was null
			dp = new DatePicker();
			DateTime dateTime = dp.Date;
			dp.SelectedDate = null;
			Assert.AreEqual(dateTime, dp.Date);

			dp.SelectedDate = dateTime.AddDays(1);
			Assert.AreEqual(dp.SelectedDate, dp.Date);

			// Setting a new Date after SelectedDate was null
			dp = new DatePicker();
			dp.SelectedDate = null;
			dp.Date = dp.Date.AddDays(1);
			Assert.AreEqual(dp.SelectedDate, dp.Date);
		}

		[Test]
		//https://github.com/xamarin/Xamarin.Forms/issues/5784
		public void SetMaxAndMinDateTimeToNow()
		{
			var datePicker = new DatePicker();
			datePicker.SetValue(DatePicker.MaximumDateProperty, DateTime.Now);
			Assert.DoesNotThrow(() => datePicker.SetValue(DatePicker.MinimumDateProperty, DateTime.Now));
		}
	}
}
