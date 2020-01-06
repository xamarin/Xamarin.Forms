using System.Collections.Generic;
using NUnit.Framework;

namespace Xamarin.Forms.ControlGallery.iOS.Tests
{
	[TestFixture]
	public class OpacityTests : PlatformTestFixture
	{
		static readonly double TestOpacity = 0.4;

		static IEnumerable<VisualElement> VisualElements
		{
			get
			{
				return new VisualElement[]
				{
					new Button { Text = "foo", Opacity = TestOpacity },
					new CheckBox { Opacity = TestOpacity },
					new DatePicker { Opacity = TestOpacity },
					new Editor { Text = "foo", Opacity = TestOpacity },
					new Entry { Text = "foo", Opacity = TestOpacity },
					new Image { Opacity = TestOpacity },
					new Picker { Opacity = TestOpacity },
					new ProgressBar { Opacity = TestOpacity },
					new SearchBar { Text = "foo", Opacity = TestOpacity },
					new Stepper { Opacity = TestOpacity },
					new Switch { Opacity = TestOpacity },
					new TimePicker { Opacity = TestOpacity },
				};
			}
		}

		[Test, TestCaseSource(nameof(VisualElements))]
		[Description("VisualElement opacity should match renderer opacity")]
		public void OpacityConsistent(View view)
		{
			var page = new ContentPage() { Content = view };

			using (var pageRenderer = GetRenderer(page))
			{
				using (var uiView = GetRenderer(view).NativeView)
				{
					page.Layout(new Rectangle(0, 0, 200, 200));

					var expected = view.Opacity;

					// Deliberatly casting this to double because Within doesn't seem to grasp nfloat
					// If you write this the other way around (casting expected to an nfloat), it fails
					Assert.That((double)uiView.Alpha, Is.EqualTo(expected).Within(0.001d));
				}
			}
		}

		[Test]
		[Description("Label background color should match renderer background color")]
		public void LabelOpacityConsistent()
		{
			var label = new Label { Text = "foo", Opacity = TestOpacity };

			var page = new ContentPage() { Content = label };

			using (var pageRenderer = GetRenderer(page))
			{
				using (var renderer = GetRenderer(label))
				{
					page.Layout(new Rectangle(0, 0, 200, 200));
					var expected = label.Opacity;
					Assert.That((double)renderer.NativeView.Alpha, Is.EqualTo(expected).Within(0.001d));
				}
			}
		}
	}
}