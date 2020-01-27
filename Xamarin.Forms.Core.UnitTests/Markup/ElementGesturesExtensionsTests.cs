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
			BindingHelpers.AssertBindingExists(gestureRecognizer, ClickGestureRecognizer.CommandParameterProperty);
		}

		[Test]
		public void BindClickGesturePositionalParameters()
		{
			var gestureElement = new TGestureElement();
			object commandSource = new ViewModel();
			object parameterSource = new ViewModel();

			if (initExistingGestureRecognizer)
				gestureElement.BindClickGesture();
			gestureElement.BindClickGesture(commandPath, commandSource, parameterPath, parameterSource);

			var gestureRecognizer = AssertHasGestureRecognizer<ClickGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, ClickGestureRecognizer.CommandProperty, commandPath, source: commandSource);
			BindingHelpers.AssertBindingExists(gestureRecognizer, ClickGestureRecognizer.CommandParameterProperty, parameterPath, source: parameterSource);
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
			BindingHelpers.AssertBindingExists(gestureRecognizer, TapGestureRecognizer.CommandParameterProperty);
		}

		[Test]
		public void BindTapGesturePositionalParameters()
		{
			var gestureElement = new TGestureElement();
			object commandSource = new ViewModel();
			object parameterSource = new ViewModel();

			if (initExistingGestureRecognizer)
				gestureElement.BindTapGesture();
			gestureElement.BindTapGesture(commandPath, commandSource, parameterPath, parameterSource);

			var gestureRecognizer = AssertHasGestureRecognizer<TapGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, TapGestureRecognizer.CommandProperty, commandPath, source: commandSource);
			BindingHelpers.AssertBindingExists(gestureRecognizer, TapGestureRecognizer.CommandParameterProperty, parameterPath, source: parameterSource);
		}

		[Test]
		public void ClickGesture()
		{
			var gestureElement = new TGestureElement();
			ClickGestureRecognizer gestureRecognizer = null;

			if (initExistingGestureRecognizer)
				gestureElement.ClickGesture(g => { });
			gestureElement.ClickGesture(g => gestureRecognizer = g);

			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void TapGesture()
		{
			var gestureElement = new TGestureElement();
			TapGestureRecognizer gestureRecognizer = null;

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
			BindingHelpers.AssertBindingExists(gestureRecognizer, SwipeGestureRecognizer.CommandParameterProperty);
		}

		[Test]
		public void BindSwipeGesturePositionalParameters()
		{
			var gestureElement = new Label();
			object commandSource = new ViewModel();
			object parameterSource = new ViewModel();

			if (initExistingGestureRecognizer)
				gestureElement.BindSwipeGesture();
			gestureElement.BindSwipeGesture(commandPath, commandSource, parameterPath, parameterSource);

			var gestureRecognizer = AssertHasGestureRecognizer<SwipeGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, SwipeGestureRecognizer.CommandProperty, commandPath, source: commandSource);
			BindingHelpers.AssertBindingExists(gestureRecognizer, SwipeGestureRecognizer.CommandParameterProperty, parameterPath, source: parameterSource);
		}

		[Test]
		public void PanGesture()
		{
			var gestureElement = new Label();
			PanGestureRecognizer gestureRecognizer = null;

			if (initExistingGestureRecognizer)
				gestureElement.PanGesture(g => { });
			gestureElement.PanGesture(g => gestureRecognizer = g);

			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void PinchGesture()
		{
			var gestureElement = new Label();
			PinchGestureRecognizer gestureRecognizer = null;

			if (initExistingGestureRecognizer)
				gestureElement.PinchGesture(g => { });
			gestureElement.PinchGesture(g => gestureRecognizer = g);

			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void SwipeGesture()
		{
			var gestureElement = new Label();
			SwipeGestureRecognizer gestureRecognizer = null;

			if (initExistingGestureRecognizer)
				gestureElement.SwipeGesture(g => { });
			gestureElement.SwipeGesture(g => gestureRecognizer = g);

			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void Gesture()
		{
			var gestureElement = new Label();
			DerivedFromGestureRecognizer gestureRecognizer = null;

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
		protected const string commandPath = nameof(ViewModel.Command), parameterPath = nameof(ViewModel.Id);

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
			public Guid Id { get; set; }

			public ICommand Command { get; set; }
		}
	}
}