using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.ControlGallery.WindowsUniversal.Tests
{
	public class PlatformTestFixture
	{
		protected IVisualElementRenderer GetRenderer(VisualElement element)
		{
			return Platform.UWP.Platform.CreateRenderer(element);
		}

		protected TextBlock GetNativeControl(Label label)
		{
			return GetRenderer(label).GetNativeElement() as TextBlock;
		}
	}
}
