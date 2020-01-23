using System;
using System.Linq;
using System.Windows.Input;
using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture(typeof(Label), false)] // Derived from View
	[TestFixture(typeof(Label), true)]
	[TestFixture(typeof(Span), false)]  // Derived from GestureElement
	[TestFixture(typeof(Span), true)]
	public class ElementGesturesExtensionsTests<TGestureElement> : ElementGesturesBaseTestFixture where TGestureElement : Element, IGestureRecognizers, new()
	{
		readonly bool initExistingGestureRecognizer;

		public ElementGesturesExtensionsTests(bool initExistingGestureRecognizer) { this.initExistingGestureRecognizer = initExistingGestureRecognizer; }

		[Test]
		public void BindClickGestureDefaults()
		{
			var gestureElement = new TGestureElement();
			if (initExistingGestureRecognizer)
				gestureElement.BindClickGesture();
			gestureElement.BindClickGesture(commandPath);
			var gestureRecognizer = AssertHasGestureRecognizer<ClickGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, ClickGestureRecognizer.CommandProperty, commandPath);
		}

		[Test]
		public void BindClickGesturePositionalParameters()
		{
			var gestureElement = new TGestureElement();

			if (initExistingGestureRecognizer)
				gestureElement.BindClickGesture();

			object converterParameter = 1;
			string stringFormat = nameof(BindClickGesturePositionalParameters) + " {0}";
			IValueConverter converter = new ToStringConverter();
			object source = new ViewModel();
			object targetNullValue = nameof(BindClickGesturePositionalParameters) + " null";
			object fallbackValue = nameof(BindClickGesturePositionalParameters) + " fallback";

			gestureElement.BindClickGesture(
				commandPath,
				BindingMode.OneWay,
				converter,
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue
			);

			var gestureRecognizer = AssertHasGestureRecognizer<ClickGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(
				gestureRecognizer, 
				ClickGestureRecognizer.CommandProperty, 
				commandPath,
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
		public void BindTapGestureDefaults()
		{
			var gestureElement = new TGestureElement();
			if (initExistingGestureRecognizer)
				gestureElement.BindTapGesture();
			gestureElement.BindTapGesture(commandPath);
			var gestureRecognizer = AssertHasGestureRecognizer<TapGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, TapGestureRecognizer.CommandProperty, commandPath);
		}

		[Test]
		public void BindTapGesturePositionalParameters()
		{
			var gestureElement = new TGestureElement();

			if (initExistingGestureRecognizer)
				gestureElement.BindTapGesture();

			object converterParameter = 1;
			string stringFormat = nameof(BindTapGesturePositionalParameters) + " {0}";
			IValueConverter converter = new ToStringConverter();
			object source = new ViewModel();
			object targetNullValue = nameof(BindTapGesturePositionalParameters) + " null";
			object fallbackValue = nameof(BindTapGesturePositionalParameters) + " fallback";

			gestureElement.BindTapGesture(
				commandPath,
				BindingMode.OneWay,
				converter,
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue
			);

			var gestureRecognizer = AssertHasGestureRecognizer<TapGestureRecognizer>(gestureElement);
			
			BindingHelpers.AssertBindingExists(
				gestureRecognizer, 
				TapGestureRecognizer.CommandProperty, 
				commandPath,
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
		public void ClickGesture()
		{
			ClickGestureRecognizer gestureRecognizer = null;
			var gestureElement = new TGestureElement();
			if (initExistingGestureRecognizer)
				gestureElement.ClickGesture(g => { });
			gestureElement.ClickGesture(g => gestureRecognizer = g);
			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void TapGesture()
		{
			TapGestureRecognizer gestureRecognizer = null;
			var gestureElement = new TGestureElement();
			if (initExistingGestureRecognizer)
				gestureElement.TapGesture(g => { });
			gestureElement.TapGesture(g => gestureRecognizer = g);
			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}
	}

	[TestFixture(false)]
	[TestFixture(true)]
	public class ElementGesturesExtensionsTests : ElementGesturesBaseTestFixture
	{
		readonly bool initExistingGestureRecognizer;

		public ElementGesturesExtensionsTests(bool initExistingGestureRecognizer) { this.initExistingGestureRecognizer = initExistingGestureRecognizer; }

		[Test]
		public void BindSwipeGestureDefaults()
		{
			var gestureElement = new Label();
			if (initExistingGestureRecognizer)
				gestureElement.BindSwipeGesture();
			gestureElement.BindSwipeGesture(commandPath);
			var gestureRecognizer = AssertHasGestureRecognizer<SwipeGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, SwipeGestureRecognizer.CommandProperty, commandPath);
		}

		[Test]
		public void BindSwipeGesturePositionalParameters()
		{
			var gestureElement = new Label();

			if (initExistingGestureRecognizer)
				gestureElement.BindSwipeGesture();

			object converterParameter = 1;
			string stringFormat = nameof(BindSwipeGesturePositionalParameters) + " {0}";
			IValueConverter converter = new ToStringConverter();
			object source = new ViewModel();
			object targetNullValue = nameof(BindSwipeGesturePositionalParameters) + " null";
			object fallbackValue = nameof(BindSwipeGesturePositionalParameters) + " fallback";

			gestureElement.BindSwipeGesture(
				commandPath,
				BindingMode.OneWay,
				converter,
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue
			);

			var gestureRecognizer = AssertHasGestureRecognizer<SwipeGestureRecognizer>(gestureElement);

			BindingHelpers.AssertBindingExists(
				gestureRecognizer, 
				SwipeGestureRecognizer.CommandProperty, 
				commandPath,
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
		public void PanGesture()
		{
			PanGestureRecognizer gestureRecognizer = null;
			var gestureElement = new Label();
			if (initExistingGestureRecognizer)
				gestureElement.PanGesture(g => { });
			gestureElement.PanGesture(g => gestureRecognizer = g);
			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void PinchGesture()
		{
			PinchGestureRecognizer gestureRecognizer = null;
			var gestureElement = new Label();
			if (initExistingGestureRecognizer)
				gestureElement.PinchGesture(g => { });
			gestureElement.PinchGesture(g => gestureRecognizer = g);
			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void SwipeGesture()
		{
			SwipeGestureRecognizer gestureRecognizer = null;
			var gestureElement = new Label();
			if (initExistingGestureRecognizer)
				gestureElement.SwipeGesture(g => { });
			gestureElement.SwipeGesture(g => gestureRecognizer = g);
			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void Gesture()
		{
			DerivedFromGestureRecognizer gestureRecognizer = null;
			var gestureElement = new Label();
			if (initExistingGestureRecognizer)
				gestureElement.Gesture((DerivedFromGestureRecognizer g) => { });
			gestureElement.Gesture((DerivedFromGestureRecognizer g) => gestureRecognizer = g);
			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void SupportDerivedFromLabel() // A View
		{
			DerivedFromLabel _ =
				new DerivedFromLabel()
				.Gesture((TapGestureRecognizer g) => g.Bind(nameof(ViewModel.Command)));
		}

		[Test]
		public void SupportDerivedFromSpan() // A GestureElement
		{
			DerivedFromSpan _ =
				new DerivedFromSpan()
				.Gesture((TapGestureRecognizer g) => g.Bind(nameof(ViewModel.Command)));
		}
	}

	public class ElementGesturesBaseTestFixture : MarkupBaseTestFixture
	{
		protected const string commandPath = nameof(ViewModel.Command);

		protected TGestureRecognizer AssertHasGestureRecognizer<TGestureRecognizer>(IGestureRecognizers element, TGestureRecognizer gestureRecognizer = null)
			where TGestureRecognizer : GestureRecognizer
		{
			if (gestureRecognizer == null)
				gestureRecognizer = (TGestureRecognizer)element?.GestureRecognizers?.FirstOrDefault(g => g is TGestureRecognizer);

			Assert.That(gestureRecognizer, Is.Not.Null);
			Assert.That(element?.GestureRecognizers?.Count(g => Object.ReferenceEquals(g, gestureRecognizer)) ?? 0, Is.EqualTo(1));

			return gestureRecognizer;
		}

		protected class DerivedFromLabel : Label { }

		protected class DerivedFromSpan : Span { }

		protected class DerivedFromGestureRecognizer : GestureRecognizer { }

		protected class ViewModel
		{
			public ICommand Command { get; set; }
		}
	}
}