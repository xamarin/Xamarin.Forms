using System.Globalization;
using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class FuncConverter : MarkupBaseTestFixture
	{
		[Test]
		public void FullyTypedTwoWayWithParam()
		{
			var converter = new FuncConverter<bool, Color, double>(
				(isRed, alpha) => (isRed ? Color.Red : Color.Green).MultiplyAlpha(alpha),
				(color, alpha) => color == Color.Red.MultiplyAlpha(alpha)
			).AssertConvert(true, 0.5, Color.Red.MultiplyAlpha(0.5), twoWay: true)
			 .AssertConvert(false, 0.2, Color.Green.MultiplyAlpha(0.2), twoWay: true);

			Assert.That(converter.Convert(null, typeof(object), null, CultureInfo.InvariantCulture), Is.EqualTo(Color.Green.MultiplyAlpha(default(double))));
			Assert.That(converter.ConvertBack(null, typeof(object), null, CultureInfo.InvariantCulture), Is.EqualTo(default(bool)));
		}

		[Test]
		public void FullyTypedTwoWay()
		{
			var converter = new FuncConverter<bool, Color, object>(
				isRed => isRed ? Color.Red : Color.Green,
				color => color == Color.Red
			).AssertConvert(true, Color.Red, twoWay: true)
			 .AssertConvert(false, Color.Green, twoWay: true);

			Assert.That(converter.Convert(null, typeof(object), null, CultureInfo.InvariantCulture), Is.EqualTo(Color.Green));
			Assert.That(converter.ConvertBack(null, typeof(object), null, CultureInfo.InvariantCulture), Is.EqualTo(default(bool)));
		}

		[Test]
		public void FullyTypedOneWayWithParam()
		{
			new FuncConverter<bool, Color, double>(
				(isRed, alpha) => (isRed ? Color.Red : Color.Green).MultiplyAlpha(alpha)
			).AssertConvert(true, 0.5, Color.Red.MultiplyAlpha(0.5))
			 .AssertConvert(false, 0.2, Color.Green.MultiplyAlpha(0.2));
		}

		[Test]
		public void FullyTypedOneWay()
		{
			new FuncConverter<bool, Color, object>(
				isRed => isRed ? Color.Red : Color.Green
			).AssertConvert(true, Color.Red)
			 .AssertConvert(false, Color.Green);
		}

		[Test]
		public void FullyTypedBackOnlyWithParam()
		{
			new FuncConverter<bool, Color, double>(
				null,
				(color, alpha) => color == Color.Red.MultiplyAlpha(alpha)
			).AssertConvert(true, 0.5, Color.Red.MultiplyAlpha(0.5), backOnly: true)
			 .AssertConvert(false, 0.2, Color.Green.MultiplyAlpha(0.2), backOnly: true);
		}

		[Test]
		public void FullyTypedBackOnly()
		{
			new FuncConverter<bool, Color, object>(
				null,
				color => color == Color.Red
			).AssertConvert(true, Color.Red, backOnly: true)
			 .AssertConvert(false, Color.Green, backOnly: true);
		}

		[Test]
		public void TwoWay()
		{
			new FuncConverter<bool, Color>(
				isRed => isRed ? Color.Red : Color.Green,
				color => color == Color.Red
			).AssertConvert(true, Color.Red, twoWay: true)
			 .AssertConvert(false, Color.Green, twoWay: true);
		}

		[Test]
		public void OneWay()
		{
			new FuncConverter<bool, Color>(
				isRed => isRed ? Color.Red : Color.Green
			).AssertConvert(true, Color.Red)
			 .AssertConvert(false, Color.Green);
		}

		[Test]
		public void BackOnly()
		{
			new FuncConverter<bool, Color>(
				null,
				color => color == Color.Red
			).AssertConvert(true, Color.Red, backOnly: true)
			 .AssertConvert(false, Color.Green, backOnly: true);
		}

		[Test]
		public void TypedSourceTwoWay()
		{
			new FuncConverter<bool>(
				isRed => isRed ? Color.Red : Color.Green,
				color => (Color)color == Color.Red
			).AssertConvert(true, Color.Red, twoWay: true)
			 .AssertConvert(false, Color.Green, twoWay: true);
		}

		[Test]
		public void TypedSourceOneWay()
		{
			new FuncConverter<bool>(
				isRed => isRed ? Color.Red : Color.Green
			).AssertConvert(true, Color.Red)
			 .AssertConvert(false, Color.Green);
		}

		[Test]
		public void TypedSourceBackOnly()
		{
			new FuncConverter<bool>(
				null,
				color => (Color)color == Color.Red
			).AssertConvert(true, (object)Color.Red, backOnly: true)
			 .AssertConvert(false, (object)Color.Green, backOnly: true);
		}

		[Test]
		public void UntypedTwoWay()
		{
			new Markup.FuncConverter(
				isRed => (bool)isRed ? Color.Red : Color.Green,
				color => (Color)color == Color.Red
			).AssertConvert((object)true, (object)Color.Red, twoWay: true)
			 .AssertConvert((object)false, (object)Color.Green, twoWay: true);
		}

		[Test]
		public void UntypedOneWay()
		{
			new Markup.FuncConverter(
				isRed => (bool)isRed ? Color.Red : Color.Green
			).AssertConvert((object)true, (object)Color.Red)
			 .AssertConvert((object)false, (object)Color.Green);
		}

		[Test]
		public void UntypedBackOnly()
		{
			new Markup.FuncConverter(
				null,
				color => (Color)color == Color.Red
			).AssertConvert((object)true, (object)Color.Red, backOnly: true)
			 .AssertConvert((object)false, (object)Color.Green, backOnly: true);
		}

		[Test]
		public void ToStringConverter()
		{
			new ToStringConverter("Converted {0}")
				.AssertConvert((object)3, "Converted 3");
		}

		[Test]
		public void ToStringConverterDefault()
		{
			new ToStringConverter()
				.AssertConvert((object)3, "3");
		}

		[Test]
		public void NotConverter()
		{
			Markup.NotConverter.Instance // Ensure instance create path covered
				.AssertConvert(true, false, twoWay: true)
				.AssertConvert(false, true, twoWay: true);

			Markup.NotConverter.Instance // Ensure instance reuse path covered
				.AssertConvert(true, false, twoWay: true)
				.AssertConvert(false, true, twoWay: true);
		}
	}
}