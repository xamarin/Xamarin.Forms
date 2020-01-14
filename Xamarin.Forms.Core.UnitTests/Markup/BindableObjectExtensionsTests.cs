using System;
using System.Reflection;
using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	using XamarinFormsMarkupUnitTestsBindableObjectViews;

	[TestFixture]
	public class BindableObjectExtensionsTests : MarkupBaseTestFixture
	{
		MethodInfo getContextMethodInfo;
		FieldInfo bindingFieldInfo;
		ViewModel viewModel;

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			getContextMethodInfo = typeof(BindableObject).GetMethod("GetContext", BindingFlags.NonPublic | BindingFlags.Instance);
			viewModel = new ViewModel();
		}

		[TearDown]
		public override void TearDown()
		{
			getContextMethodInfo = null;
			bindingFieldInfo = null;
			viewModel = null;
			base.TearDown();
		}

		[Test]
		public void BindSpecifiedPropertyWithDefaults()
		{
			var label = new Label();
			label.Bind(Label.TextColorProperty, nameof(viewModel.TextColor));
			AssertBindingExists(label, Label.TextColorProperty, nameof(viewModel.TextColor));
		}

		// Note that we test positional parameters to catch API parameter order changes (which would be breaking).
		// Testing named parameters is not useful because a parameter rename operation in the API would also rename it in the test
		[Test]
		public void BindSpecifiedPropertyWithPositionalParameters()
		{
			var button = new Button();
			object converterParameter = 1;
			string stringFormat = nameof(BindSpecifiedPropertyWithPositionalParameters) + " {0}";
			IValueConverter converter = new ToStringConverter();
			object source = new ViewModel();
			object targetNullValue = nameof(BindSpecifiedPropertyWithPositionalParameters) + " null";
			object fallbackValue = nameof(BindSpecifiedPropertyWithPositionalParameters) + " fallback";

			button.Bind(
				Button.TextProperty,
				nameof(viewModel.Text),
				BindingMode.OneWay,
				converter,
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue
			);

			AssertBindingExists(
				button,
				targetProperty: Button.TextProperty,
				path: nameof(viewModel.Text),
				mode: BindingMode.OneWay,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
		}

		[Test]
		public void BindSpecifiedPropertyWithOneWayInlineConvertAndDefaults()
		{
			var label = new Label();
			label.Bind(
				Label.TextColorProperty,
				nameof(viewModel.IsRed),
				convert: (bool isRed) => isRed ? Color.Red : Color.Transparent
			);

			AssertBindingExists(
				label,
				Label.TextColorProperty,
				nameof(viewModel.IsRed),
				assertConverterInstanceIsAnyNotNull: true
			);
		}

		[Test]
		public void BindSpecifiedPropertyWithTwoWayInlineConvertAndDefaults()
		{
			var label = new Label();
			label.Bind(
				Label.TextColorProperty,
				nameof(viewModel.IsRed),
				BindingMode.TwoWay,
				(bool isRed) => isRed ? Color.Red : Color.Transparent,
				color => color == Color.Red
			);

			AssertBindingExists(
				label,
				Label.TextColorProperty,
				nameof(viewModel.IsRed),
				BindingMode.TwoWay,
				assertConverterInstanceIsAnyNotNull: true
			);
		}

		[Test]
		public void BindSpecifiedPropertyWithOneWayInlineConvertAndPositionalParameters()
		{
			var button = new Button();
			object converterParameter = 1;
			string stringFormat = nameof(BindSpecifiedPropertyWithOneWayInlineConvertAndPositionalParameters) + " {0}";
			object source = new ViewModel();
			object targetNullValue = nameof(BindSpecifiedPropertyWithOneWayInlineConvertAndPositionalParameters) + " null";
			object fallbackValue = nameof(BindSpecifiedPropertyWithOneWayInlineConvertAndPositionalParameters) + " fallback";

			button.Bind<Button, string, string>(
				Button.TextProperty,
				nameof(viewModel.Text),
				BindingMode.OneWay,
				(string text) => $"'{text?.Trim('\'')}'",
				null,
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue
			);

			AssertBindingExists(
				button,
				targetProperty: Button.TextProperty,
				path: nameof(viewModel.Text),
				mode: BindingMode.OneWay,
				assertConverterInstanceIsAnyNotNull: true,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
		}


		[Test]
		public void BindSpecifiedPropertyWithTwoWayInlineConvertAndPositionalParameters()
		{
			var button = new Button();
			object converterParameter = 1;
			string stringFormat = nameof(BindSpecifiedPropertyWithTwoWayInlineConvertAndPositionalParameters) + " {0}";
			object source = new ViewModel();
			object targetNullValue = nameof(BindSpecifiedPropertyWithTwoWayInlineConvertAndPositionalParameters) + " null";
			object fallbackValue = nameof(BindSpecifiedPropertyWithTwoWayInlineConvertAndPositionalParameters) + " fallback";

			button.Bind(
				Button.TextProperty,
				nameof(viewModel.Text),
				BindingMode.TwoWay,
				(string text) => $"'{text?.Trim('\'')}'",
				text => text?.Trim('\''),
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue
			);

			AssertBindingExists(
				button,
				targetProperty: Button.TextProperty,
				path: nameof(viewModel.Text),
				mode: BindingMode.TwoWay,
				assertConverterInstanceIsAnyNotNull: true,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
		}

		[Test]
		public void BindDefaultPropertyWithDefaults()
		{
			var label = new Label();
			label.Bind(nameof(viewModel.Text));
			AssertBindingExists(label, Label.TextProperty, nameof(viewModel.Text));
		}

		[Test]
		public void BindDefaultPropertyWithPositionalParameters()
		{
			var label = new Label();
			object converterParameter = 1;
			string stringFormat = nameof(BindDefaultPropertyWithPositionalParameters) + " {0}";
			IValueConverter converter = new ToStringConverter();
			object source = new ViewModel();
			object targetNullValue = nameof(BindDefaultPropertyWithPositionalParameters) + " null";
			object fallbackValue = nameof(BindDefaultPropertyWithPositionalParameters) + " fallback";

			label.Bind(
				nameof(viewModel.Text),
				BindingMode.OneWay,
				converter,
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue
			);

			AssertBindingExists(
				label,
				targetProperty: Label.TextProperty,
				path: nameof(viewModel.Text),
				mode: BindingMode.OneWay,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
		}

		[Test]
		public void BindDefaultPropertyWithOneWayInlineConvertAndDefaults()
		{
			var label = new Label();
			label.Bind(
				nameof(viewModel.Text),
				convert: (string text) => $"'{text}'"
			);

			AssertBindingExists(
				label,
				Label.TextProperty,
				nameof(viewModel.Text),
				assertConverterInstanceIsAnyNotNull: true
			);
		}

		[Test]
		public void BindDefaultPropertyWithTwoWayInlineConvertAndDefaults()
		{
			var label = new Label();
			label.Bind(
				nameof(viewModel.Text),
				BindingMode.TwoWay,
				(string text) => $"'{text?.Trim('\'')}'",
				text => text?.Trim('\'')
			);

			AssertBindingExists(
				label,
				Label.TextProperty,
				nameof(viewModel.Text),
				BindingMode.TwoWay,
				assertConverterInstanceIsAnyNotNull: true
			);
		}

		[Test]
		public void BindDefaultPropertyWithOneWayInlineConvertAndPositionalParameters()
		{
			var label = new Label();
			object converterParameter = 1;
			string stringFormat = nameof(BindDefaultPropertyWithOneWayInlineConvertAndPositionalParameters) + " {0}";
			object source = new ViewModel();
			object targetNullValue = nameof(BindDefaultPropertyWithOneWayInlineConvertAndPositionalParameters) + " null";
			object fallbackValue = nameof(BindDefaultPropertyWithOneWayInlineConvertAndPositionalParameters) + " fallback";

			label.Bind<Label, string, string>(
				nameof(viewModel.Text),
				BindingMode.OneWay,
				(string text) => $"'{text?.Trim('\'')}'",
				null,
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue
			);

			AssertBindingExists(
				label,
				targetProperty: Label.TextProperty,
				path: nameof(viewModel.Text),
				mode: BindingMode.OneWay,
				assertConverterInstanceIsAnyNotNull: true,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
		}


		[Test]
		public void BindDefaultPropertyWithTwoWayInlineConvertAndPositionalParameters()
		{
			var label = new Label();
			object converterParameter = 1;
			string stringFormat = nameof(BindDefaultPropertyWithTwoWayInlineConvertAndPositionalParameters) + " {0}";
			object source = new ViewModel();
			object targetNullValue = nameof(BindDefaultPropertyWithTwoWayInlineConvertAndPositionalParameters) + " null";
			object fallbackValue = nameof(BindDefaultPropertyWithTwoWayInlineConvertAndPositionalParameters) + " fallback";

			label.Bind(
				nameof(viewModel.Text),
				BindingMode.TwoWay,
				(string text) => $"'{text?.Trim('\'')}'",
				text => text?.Trim('\''),
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue
			);

			AssertBindingExists(
				label,
				targetProperty: Label.TextProperty,
				path: nameof(viewModel.Text),
				mode: BindingMode.TwoWay,
				assertConverterInstanceIsAnyNotNull: true,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
		}

		[Test]
		public void Assign()
		{
			var createdLabel = new Label().Assign(out Label assignLabel);
			Assert.That(Object.ReferenceEquals(createdLabel, assignLabel));
		}

		[Test]
		public void Invoke()
		{
			var createdLabel = new Label().Invoke(null).Invoke(l => l.Text = nameof(Invoke));
			Assert.That(createdLabel.Text, Is.EqualTo(nameof(Invoke)));
		}

		[Test]
		public void SupportDerivedFromLabel()
		{
			DerivedFromLabel createdDerivedFromLabel =
				new DerivedFromLabel()
				.Bind(nameof(viewModel.Text))
				.Bind(
					nameof(viewModel.Text),
					convert: (string text) => $"'{text}'")
				.Bind(
					DerivedFromLabel.TextColorProperty, 
					nameof(viewModel.TextColor))
				.Bind(
					DerivedFromLabel.BackgroundColorProperty,
					nameof(viewModel.IsRed),
					convert: (bool isRed) => isRed ? Color.Black : Color.Transparent)
				.Invoke(l => l.Text = nameof(SupportDerivedFromLabel))
				.Assign(out DerivedFromLabel assignDerivedFromLabel);
		}

		void AssertBindingExists(
			BindableObject bindable,
			BindableProperty targetProperty,
			string path = ".",
			BindingMode mode = BindingMode.Default,
			bool assertConverterInstanceIsAnyNotNull = false,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		)
		{
			var binding =  GetBinding(bindable, targetProperty);
			Assert.That(binding, Is.Not.Null);
			Assert.That(binding.Path, Is.EqualTo(path));
			Assert.That(binding.Mode, Is.EqualTo(mode));
			if (assertConverterInstanceIsAnyNotNull)
				Assert.That(binding.Converter, Is.Not.Null);
			else
				Assert.That(binding.Converter, Is.EqualTo(converter));
			Assert.That(binding.ConverterParameter, Is.EqualTo(converterParameter));
			Assert.That(binding.StringFormat, Is.EqualTo(stringFormat));
			Assert.That(binding.Source, Is.EqualTo(source));
			Assert.That(binding.TargetNullValue, Is.EqualTo(targetNullValue));
			Assert.That(binding.FallbackValue, Is.EqualTo(fallbackValue));
		}

		/// <remarks>
		/// Note that we are only testing whether the Markup helpers create the correct bindings,
		/// we are not testing the binding mechanism itself; this is why it is justified to access
		/// private binding API's here for testing.
		/// </remarks>
		Binding GetBinding(BindableObject bindable, BindableProperty property)
		{
			// return bindable.GetContext(property)?.Binding as Binding;
			// Both BindableObject.GetContext and BindableObject.BindablePropertyContext are private; 
			// use reflection instead of above line.

			var context = getContextMethodInfo?.Invoke(bindable, new object[] { property });

			if (bindingFieldInfo == null)
				bindingFieldInfo = context?.GetType().GetField("Binding");

			return bindingFieldInfo?.GetValue(context) as Binding;
		}

		class ViewModel
		{
			public string Text { get; set; }
			public Color TextColor { get; set; }
			public bool IsRed { get; set; }
		}
	}
}

namespace XamarinFormsMarkupUnitTestsBindableObjectViews
{
	using Xamarin.Forms;

	class DerivedFromLabel : Label { }
}