using System.Collections.Generic;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.ControlGallery.iOS.Tests
{
	[Internals.Preserve(AllMembers = true)]
	public class PlatformTestFixture
	{
		// Sequence for generating test cases
		protected static IEnumerable<VisualElement> BasicElements
		{
			get
			{
				yield return new Button { };
				yield return new CheckBox { };
				yield return new DatePicker { };
				yield return new Editor { };
				yield return new Entry { };
				yield return new Image { };
				yield return new Label { };
				yield return new Picker { };
				yield return new ProgressBar { };
				yield return new SearchBar { };
				yield return new Stepper { };
				yield return new Switch { };
				yield return new TimePicker { };
			}
		}

		protected IVisualElementRenderer GetRenderer(VisualElement element)
		{
			return Platform.iOS.Platform.CreateRenderer(element);
		}

		protected UIView GetNativeControl(VisualElement visualElement)
		{
			var renderer = GetRenderer(visualElement);
			var viewRenderer = renderer as IVisualNativeElementRenderer;
			return viewRenderer.Control;
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
	}
}