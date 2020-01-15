using System.Collections.Generic;
using CoreGraphics;
using NUnit.Framework;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.ControlGallery.iOS.Tests
{
	[Internals.Preserve(AllMembers = true)]
	public class PlatformTestFixture
	{
		// Sequence for generating test cases
		protected static IEnumerable<View> BasicViews
		{
			get
			{
				yield return new BoxView { };
				yield return new Button { };
				yield return new CheckBox { };
				yield return new DatePicker { };
				yield return new Editor { };
				yield return new Entry { };
				yield return new Frame { };
				yield return new Image { };
				yield return new ImageButton { };
				yield return new Label { };
				yield return new Picker { };
				yield return new ProgressBar { };
				yield return new SearchBar { };
				yield return new Slider { };
				yield return new Stepper { };
				yield return new Switch { };
				yield return new TimePicker { };
			}
		}

		protected static TestCaseData CreateTestCase(VisualElement element)
		{
			// We set the element type as a category on the test so that if you 
			// filter by category, say, "Button", you'll get any Button test 
			// generated from here. 

			return new TestCaseData(element).SetCategory(element.GetType().Name);
		}

		protected IVisualElementRenderer GetRenderer(VisualElement element)
		{
			return Platform.iOS.Platform.CreateRenderer(element);
		}

		protected UIView GetNativeControl(VisualElement visualElement)
		{
			var renderer = GetRenderer(visualElement);
			var viewRenderer = renderer as IVisualNativeElementRenderer;
			return viewRenderer?.Control;
		}

		protected UILabel GetNativeControl(Label label)
		{
			var renderer = GetRenderer(label);
			var viewRenderer = renderer.NativeView as LabelRenderer;
			return viewRenderer.Control;
		}

		protected UITextField GetNativeControl(Entry entry)
		{
			var renderer = GetRenderer(entry);
			var viewRenderer = renderer.NativeView as EntryRenderer;
			return viewRenderer.Control;
		}

		protected UITextView GetNativeControl(Editor editor)
		{
			var renderer = GetRenderer(editor);
			var viewRenderer = renderer.NativeView as EditorRenderer;
			return viewRenderer.Control;
		}

		protected UIButton GetNativeControl(Button button)
		{
			var renderer = GetRenderer(button);
			var viewRenderer = renderer.NativeView as ButtonRenderer;
			return viewRenderer.Control;
		}

		protected UIColor GetColorAtCenter(UIView view)
		{
			var point = new CGPoint(view.Frame.GetMidX(), view.Frame.GetMidY());
			var pixel = new byte[4];
			using (var colorSpace = CGColorSpace.CreateDeviceRGB())
			{
				using (var context = new CGBitmapContext(pixel, 1, 1, 8, 4, colorSpace, CGBitmapFlags.PremultipliedLast))
				{
					context.TranslateCTM(-point.X, -point.Y);

					view.Layer.RenderInContext(context);
					var color = new UIColor(pixel[0] / 255.0f, pixel[1] / 255.0f, pixel[2] / 255.0f, pixel[3] / 255.0f);

					return color;
				}
			}
		}
	}
}