using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.ControlGallery.iOS.Tests
{
	[Internals.Preserve(AllMembers = true)]
	public class PlatformTestFixture
	{
		protected IVisualElementRenderer GetRenderer(VisualElement element)
		{
			return Platform.iOS.Platform.CreateRenderer(element);
		}

		protected UITextView GetNativeControl(Label label)
		{
			var renderer = GetRenderer(label);
			var viewRenderer = renderer.NativeView as ViewRenderer<Label, UITextView>;
			return viewRenderer.Control;
		}
	}
}