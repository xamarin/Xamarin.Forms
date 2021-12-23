using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.WPF.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	public class PageRenderer : VisualPageRenderer<Page, FormsContentPage>
	{
		VisualElement _currentView = null;

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
					SetNativeControl(new FormsContentPage());

				UpdateContent();
			}
			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ContentPage.ContentProperty.PropertyName
				|| e.PropertyName == TemplatedPage.ControlTemplateProperty.PropertyName)
			{
				UpdateContent();
			}
		}

		void UpdateContent()
		{
			if (Element is ContentPage page)
			{
				if (_currentView != null) // destroy current view
				{
					_currentView?.Cleanup();
					_currentView = null;
				}

				_currentView = page.LogicalChildren.OfType<VisualElement>().FirstOrDefault() ?? page.Content;
				Control.Content = _currentView != null ? Platform.GetOrCreateRenderer(_currentView).GetNativeElement() : null;
			}
		}
	}
}