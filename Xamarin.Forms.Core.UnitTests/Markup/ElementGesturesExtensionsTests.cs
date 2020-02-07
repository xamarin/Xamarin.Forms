using System;
using System.Linq;
using System.Windows.Input;
using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture(typeof(Label))] // Derived from View
	[TestFixture(typeof(Span))]  // Derived from GestureElement
	public class ElementGesturesExtensionsTests<TGestureElement> : ElementGesturesBaseTestFixture where TGestureElement : Element, IGestureRecognizers, new()
	{
		[Test]
		public void BindClickGestureDefaults()
		{
			var gestureElement = new TGestureElement();

			gestureElement.BindClickGesture(commandPath);

			var gestureRecognizer = AssertHasGestureRecognizer<ClickGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, ClickGestureRecognizer.CommandProperty, commandPath);
		}

		[Test]
		public void BindClickGesturePositionalParameters()
		{
			var gestureElement = new TGestureElement();
			object commandSource = new ViewModel();
			object parameterSource = new ViewModel();

			gestureElement.BindClickGesture(commandPath, commandSource, parameterPath, parameterSource);

			var gestureRecognizer = AssertHasGestureRecognizer<ClickGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, ClickGestureRecognizer.CommandProperty, commandPath, source: commandSource);
			BindingHelpers.AssertBindingExists(gestureRecognizer, ClickGestureRecognizer.CommandParameterProperty, parameterPath, source: parameterSource);
		}

		[Test]
		public void BindTapGestureDefaults()
		{
			var gestureElement = new TGestureElement();

			gestureElement.BindTapGesture(commandPath);

			var gestureRecognizer = AssertHasGestureRecognizer<TapGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, TapGestureRecognizer.CommandProperty, commandPath);
		}

		[Test]
		public void BindTapGesturePositionalParameters()
		{
			var gestureElement = new TGestureElement();
			object commandSource = new ViewModel();
			object parameterSource = new ViewModel();

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

			gestureElement.ClickGesture(g => gestureRecognizer = g);

			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void TapGesture()
		{
			var gestureElement = new TGestureElement();
			TapGestureRecognizer gestureRecognizer = null;

			gestureElement.TapGesture(g => gestureRecognizer = g);

			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}
	}

	[TestFixture]
	public class ElementGesturesExtensionsTests : ElementGesturesBaseTestFixture
	{
		[Test]
		public void BindSwipeGestureDefaults()
		{
			var gestureElement = new Label();

			gestureElement.BindSwipeGesture(commandPath);

			var gestureRecognizer = AssertHasGestureRecognizer<SwipeGestureRecognizer>(gestureElement);
			BindingHelpers.AssertBindingExists(gestureRecognizer, SwipeGestureRecognizer.CommandProperty, commandPath);
		}

		[Test]
		public void BindSwipeGesturePositionalParameters()
		{
			var gestureElement = new Label();
			object commandSource = new ViewModel();
			object parameterSource = new ViewModel();

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

			gestureElement.PanGesture(g => gestureRecognizer = g);

			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void PinchGesture()
		{
			var gestureElement = new Label();
			PinchGestureRecognizer gestureRecognizer = null;

			gestureElement.PinchGesture(g => gestureRecognizer = g);

			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void SwipeGesture()
		{
			var gestureElement = new Label();
			SwipeGestureRecognizer gestureRecognizer = null;

			gestureElement.SwipeGesture(g => gestureRecognizer = g);

			AssertHasGestureRecognizer(gestureElement, gestureRecognizer);
		}

		[Test]
		public void MultipleGestures()
		{
			var gestureElement = new Label();
			TapGestureRecognizer gestureRecognizer1 = null, gestureRecognizer2 = null;
			SwipeGestureRecognizer gestureRecognizer3 = null;

			gestureElement.TapGesture(g => gestureRecognizer1 = g);
			gestureElement.TapGesture(g => gestureRecognizer2 = g);
			gestureElement.SwipeGesture(g => gestureRecognizer3 = g);

			AssertHasGestureRecognizers(gestureElement, gestureRecognizer1, gestureRecognizer2);
			AssertHasGestureRecognizer(gestureElement, gestureRecognizer3);
		}

		[Test]
		public void Gesture()
		{
			var gestureElement = new Label();
			DerivedFromGestureRecognizer gestureRecognizer = null;

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

		protected TGestureRecognizer AssertHasGestureRecognizer<TGestureRecognizer>(IGestureRecognizers element)
			where TGestureRecognizer : GestureRecognizer
			=> AssertHasGestureRecognizers<TGestureRecognizer>(element, 1)[0];

		protected TGestureRecognizer AssertHasGestureRecognizer<TGestureRecognizer>(IGestureRecognizers element, TGestureRecognizer gestureRecognizer)
			where TGestureRecognizer : GestureRecognizer
			=> AssertHasGestureRecognizers(element, 1, gestureRecognizer)[0];

		protected TGestureRecognizer[] AssertHasGestureRecognizers<TGestureRecognizer>(IGestureRecognizers element, params TGestureRecognizer[] gestureRecognizers)
			where TGestureRecognizer : GestureRecognizer
			=> AssertHasGestureRecognizers(element, gestureRecognizers.Length, gestureRecognizers: gestureRecognizers);

		protected TGestureRecognizer[] AssertHasGestureRecognizers<TGestureRecognizer>(IGestureRecognizers element, int count, params TGestureRecognizer[] gestureRecognizers)
			where TGestureRecognizer : GestureRecognizer
		{
			if (gestureRecognizers.Length == 0)
				gestureRecognizers = element?.GestureRecognizers?.Where(g => g is TGestureRecognizer).Cast<TGestureRecognizer>().ToArray();

			Assert.That(gestureRecognizers.Length, Is.EqualTo(count));

			foreach (var gestureRecognizer in gestureRecognizers)
				Assert.That(element?.GestureRecognizers?.Count(g => Object.ReferenceEquals(g, gestureRecognizer)) ?? 0, Is.EqualTo(1));

			return gestureRecognizers;
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