using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture(true)]
	[TestFixture(false)]
	public class BindableObjectMultiBindExtensionsTests : MarkupBaseTestFixture
	{
		ViewModel viewModel;
		List<BindingBase> testBindings;
		List<object> testConvertValues;

		public BindableObjectMultiBindExtensionsTests(bool withExperimentalFlag) : base(withExperimentalFlag) { }

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			viewModel = new ViewModel();

			testBindings = new List<BindingBase> {
				new Binding(nameof(viewModel.Text)),
				new Binding(nameof(viewModel.Id)),
				new Binding(nameof(viewModel.IsDone)),
				new Binding(nameof(viewModel.Fraction)),
				new Binding(nameof(viewModel.Count))
			};

			testConvertValues = new List<object> {
				"Hi",
				Guid.Parse("{272383A4-92E3-46BA-99DC-438D81E039AB}"),
				true,
				0.5,
				3
			};
		}

		[TearDown]
		public override void TearDown()
		{
			viewModel = null;
			testBindings = null;
			testConvertValues = null;
			base.TearDown();
		}

		[Test, TestCase(true, false), TestCase(false, true), TestCase(true, true)]
		public void BindSpecifiedPropertyWith2BindingsAndInlineConvert(bool testConvert, bool testConvertBack) => AssertExperimental(() =>
		{
			var label = new Label();

			// Repeat inline converter code to test that the Bind overloads allow inferring the generic parameter types
			if (testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1],
					((string text, Guid id) v) => Format(0, v.text, v.id),
					(string formatted) => { var u = Unformat(0, formatted); return (u.Text, u.Id); }
				);
			}
			else if (testConvert && !testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1],
					((string text, Guid id) v) => Format(0, v.text, v.id)
				);
			}
			else if (!testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1],
					convertBack: (string formatted) => { var u = Unformat(0, formatted); return (u.Text, u.Id); }
				);
			}

			AssertLabelTextMultiBound(label, 2, testConvert, testConvertBack);
		});

		[Test, TestCase(true, false), TestCase(false, true), TestCase(true, true)]
		public void BindSpecifiedPropertyWith2BindingsAndInlineConvertAndParameter(bool testConvert, bool testConvertBack) => AssertExperimental(() =>
		{
			var label = new Label();

			// Repeat inline converter code to test that the Bind overloads allow inferring the generic parameter types
			if (testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1],
					((string text, Guid id) v, int parameter) => Format(parameter, v.text, v.id),
					(string formatted, int parameter) => { var u = Unformat(parameter, formatted); return (u.Text, u.Id); },
					converterParameter: 2
				);
			}
			else if (testConvert && !testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1],
					((string text, Guid id) v, int parameter) => Format(parameter, v.text, v.id),
					converterParameter: 2
				);
			}
			else if (!testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1],
					convertBack: (string formatted, int parameter) => { var u = Unformat(parameter, formatted); return (u.Text, u.Id); },
					converterParameter: 2
				);
			}

			AssertLabelTextMultiBound(label, 2, testConvert, testConvertBack, 2);
		});

		[Test, TestCase(true, false), TestCase(false, true), TestCase(true, true)]
		public void BindSpecifiedPropertyWith3BindingsAndInlineConvert(bool testConvert, bool testConvertBack) => AssertExperimental(() =>
		{
			var label = new Label();

			// Repeat inline converter code to test that the Bind overloads allow inferring the generic parameter types
			if (testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2],
					((string text, Guid id, bool isDone) v) => Format(0, v.text, v.id, v.isDone),
					(string formatted) => { var u = Unformat(0, formatted); return (u.Text, u.Id, u.IsDone); }
				);
			}
			else if (testConvert && !testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2],
					((string text, Guid id, bool isDone) v) => Format(0, v.text, v.id, v.isDone)
				);
			}
			else if (!testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2],
					convertBack: (string formatted) => { var u = Unformat(0, formatted); return (u.Text, u.Id, u.IsDone); }
				);
			}

			AssertLabelTextMultiBound(label, 3, testConvert, testConvertBack);
		});

		[Test, TestCase(true, false), TestCase(false, true), TestCase(true, true)]
		public void BindSpecifiedPropertyWith3BindingsAndInlineConvertAndParameter(bool testConvert, bool testConvertBack) => AssertExperimental(() =>
		{
			var label = new Label();

			// Repeat inline converter code to test that the Bind overloads allow inferring the generic parameter types
			if (testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2],
					((string text, Guid id, bool isDone) v, int parameter) => Format(parameter, v.text, v.id, v.isDone),
					(string formatted, int parameter) => { var u = Unformat(parameter, formatted); return (u.Text, u.Id, u.IsDone); },
					converterParameter: 2
				);
			}
			else if (testConvert && !testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2],
					((string text, Guid id, bool isDone) v, int parameter) => Format(parameter, v.text, v.id, v.isDone),
					converterParameter: 2
				);
			}
			else if (!testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2],
					convertBack: (string formatted, int parameter) => { var u = Unformat(parameter, formatted); return (u.Text, u.Id, u.IsDone); },
					converterParameter: 2
				);
			}

			AssertLabelTextMultiBound(label, 3, testConvert, testConvertBack, 2);
		});

		[Test, TestCase(true, false), TestCase(false, true), TestCase(true, true)]
		public void BindSpecifiedPropertyWith4BindingsAndInlineConvert(bool testConvert, bool testConvertBack) => AssertExperimental(() =>
		{
			var label = new Label();

			// Repeat inline converter code to test that the Bind overloads allow inferring the generic parameter types
			if (testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2], testBindings[3],
					((string text, Guid id, bool isDone, double fraction) v) => Format(0, v.text, v.id, v.isDone, v.fraction),
					(string formatted) => { var u = Unformat(0, formatted); return (u.Text, u.Id, u.IsDone, u.Fraction); }
				);
			} else if (testConvert && !testConvertBack) {
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2], testBindings[3],
					((string text, Guid id, bool isDone, double fraction) v) => Format(0, v.text, v.id, v.isDone, v.fraction)
				);
			}
			else if (!testConvert && testConvertBack) {
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2], testBindings[3],
					convertBack: (string formatted) => { var u = Unformat(0, formatted); return (u.Text, u.Id, u.IsDone, u.Fraction); }
				);
			}

			AssertLabelTextMultiBound(label, 4, testConvert, testConvertBack);
		});

		[Test, TestCase(true, false), TestCase(false, true), TestCase(true, true)]
		public void BindSpecifiedPropertyWith4BindingsAndInlineConvertAndParameter(bool testConvert, bool testConvertBack) => AssertExperimental(() =>
		{
			var label = new Label();

			// Repeat inline converter code to test that the Bind overloads allow inferring the generic parameter types
			if (testConvert && testConvertBack)
			{
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2], testBindings[3],
					((string text, Guid id, bool isDone, double fraction) v, int parameter) => Format(parameter, v.text, v.id, v.isDone, v.fraction),
					(string formatted, int parameter) => { var u = Unformat(parameter, formatted); return (u.Text, u.Id, u.IsDone, u.Fraction); },
					converterParameter: 2
				);
			} else if (testConvert && !testConvertBack) {
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2], testBindings[3],
					((string text, Guid id, bool isDone, double fraction) v, int parameter) => Format(parameter, v.text, v.id, v.isDone, v.fraction),
					converterParameter: 2
				);
			}
			else if (!testConvert && testConvertBack) {
				label.Bind(
					Label.TextProperty,
					testBindings[0], testBindings[1], testBindings[2], testBindings[3],
					convertBack: (string formatted, int parameter) => { var u = Unformat(parameter, formatted); return (u.Text, u.Id, u.IsDone, u.Fraction); },
					converterParameter: 2
				);
			}

			AssertLabelTextMultiBound(label, 4, testConvert, testConvertBack, 2);
		});

		[Test, TestCase(true, false), TestCase(false, true), TestCase(true, true)]
		public void BindSpecifiedPropertyWithMultipleBindings(bool testConvert, bool testConvertBack) => AssertExperimental(() =>
		{
			Func<object[], string> convert = null;
			if (testConvert) convert = (object[] v) => Format(0, v[0], v[1], v[2], v[3], v[4]);

			Func<string, object[]> convertBack = null;
			if (testConvertBack) convertBack = (string formatted) => { var u = Unformat(0, formatted); return new object[]
				{ u.Text, u.Id, u.IsDone, u.Fraction, u.Count }; };

			var converter = new FuncMultiConverter<string, object>(convert, convertBack);
			var label = new Label { } .Bind (Label.TextProperty, GetTestBindings(5), converter);
			AssertLabelTextMultiBound(label, 5, testConvert, testConvertBack, converter: converter);
		});

		[Test, TestCase(true, false), TestCase(false, true), TestCase(true, true)]
		public void BindSpecifiedPropertyWithMultipleBindingsAndParameter(bool testConvert, bool testConvertBack) => AssertExperimental(() =>
		{
			Func<object[], int, string> convert = null;
			if (testConvert) convert = (object[] v, int parameter) => Format(parameter, 
				v[0], v[1], v[2], v[3], v[4]);

			Func<string, int, object[]> convertBack = null;
			if (testConvertBack) convertBack = (string text, int parameter) => { var u = Unformat(parameter, text); return new object[]
				{ u.Text, u.Id, u.IsDone, u.Fraction, u.Count }; };

			var converter = new FuncMultiConverter<string, int>(convert, convertBack);
			var label = new Label { } .Bind (Label.TextProperty, GetTestBindings(5), converter, 2);
			AssertLabelTextMultiBound(label, 5, testConvert, testConvertBack, 2, converter);
		});

		List<BindingBase> GetTestBindings(int count) => testBindings.Take(count).ToList();

		object[] GetTestConvertValues(int count) => testConvertValues.Take(count).ToArray();

		string PrefixDots(object value, int count) => $"{new string('.', count)}{value}";

		string RemoveDots(string text, int count) => text.Substring(count);

		string Format(int parameter, params object[] values)
		{
			string formatted = $"'{PrefixDots(values[0], parameter)}'";
			for (int i = 1; i < values.Length; i++) formatted += $", '{values[i]}'";
			return formatted;
		}

		(string Text, Guid Id, bool IsDone, double Fraction, int Count) Unformat(int parameter, string formatted)
		{
			var split = formatted.Split('\'');
			int n = split.Length;

			return (
				n > 1 ? RemoveDots(split[1], parameter) : null, 
				n > 3 ? Guid.Parse(split[3]) : Guid.Empty,
				n > 5 ? bool.Parse(split[5]) : false,
				n > 7 ? double.Parse(split[7]) : 0,
				n > 9 ? int.Parse(split[9]) : 0
			);
		}

		void AssertLabelTextMultiBound(Label label, int nBindings, bool testConvert, bool testConvertBack, int parameter, IMultiValueConverter converter = null)
		{
			var values = GetTestConvertValues(nBindings);
			string expected = Format(parameter, values);

			BindingHelpers.AssertBindingExists<string, int>(
				label,
				targetProperty: Label.TextProperty,
				GetTestBindings(nBindings),
				converter,
				parameter,
				assertConverterInstanceIsAnyNotNull: converter == null,
				assertConvert: c => c.AssertConvert(values, parameter, expected, twoWay: testConvert && testConvertBack, backOnly: !testConvert && testConvertBack)
			);
		}

		void AssertLabelTextMultiBound(Label label, int nBindings, bool testConvert, bool testConvertBack, IMultiValueConverter converter = null)
		{
			var values = GetTestConvertValues(nBindings);
			string expected = Format(0, values);

			BindingHelpers.AssertBindingExists<string>(
				label,
				targetProperty: Label.TextProperty,
				GetTestBindings(nBindings),
				converter,
				assertConverterInstanceIsAnyNotNull: converter == null,
				assertConvert: c => c.AssertConvert(values, expected, twoWay: testConvert && testConvertBack, backOnly: !testConvert && testConvertBack)
			);
		}

		class ViewModel
		{
			public Guid Id { get; set; }
			public string Text { get; set; }
			public bool IsDone { get; set; }
			public double Fraction { get; set; }
			public int Count { get; set; }
		}
	}
}