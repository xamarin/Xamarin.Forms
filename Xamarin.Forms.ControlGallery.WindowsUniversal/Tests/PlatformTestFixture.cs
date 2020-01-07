using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.ControlGallery.WindowsUniversal.Tests
{
	public class PlatformTestFixture
	{
		protected IVisualElementRenderer GetRenderer(VisualElement element)
		{
			return element.GetOrCreateRenderer();
		}

		protected Control GetNativeControl(VisualElement element)
		{
			return GetRenderer(element).GetNativeElement() as Control;
		}

		protected Panel GetContainer(VisualElement element) 
		{
			return GetRenderer(element).ContainerElement as Panel;
		}
		
		protected TextBlock GetNativeControl(Label label)
		{
			return GetRenderer(label).GetNativeElement() as TextBlock;
		}

		protected FormsButton GetNativeControl(Button button)
		{
			return GetRenderer(button).GetNativeElement() as FormsButton;
		}

		protected FormsTextBox GetNativeControl(Entry entry)
		{
			return GetRenderer(entry).GetNativeElement() as FormsTextBox;
		}

		protected FormsTextBox GetNativeControl(Editor editor)
		{
			return GetRenderer(editor).GetNativeElement() as FormsTextBox;
		}
	}
}
