using System.Windows.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	public class PageRenderer : VisualElementRenderer<Page, Panel>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			// Set prior to calling base
			Tracker = new BackgroundTracker<Panel>(BackgroundProperty) { Model = Element, Element = this };
			
			base.OnElementChanged(e);

			Loaded += (sender, args) =>
			{
			    ((IPageController)Element).SendAppearing();
			    Element.BackgroundColor = Color.White;
			};
			Unloaded += (sender, args) => ((IPageController)Element).SendDisappearing();
		}
	}
}