using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Xamarin.Forms.Platform.UWP
{
	public class BoxViewBorderRenderer : ViewRenderer<BoxView, Border>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var border = new Border
					{
						DataContext = Element
					};
					SetNativeControl(border);
				}

				SetCornerRadius(Element.CornerRadius);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == BoxView.CornerRadiusProperty.PropertyName)
				SetCornerRadius(Element.CornerRadius);
			if(e.PropertyName == BoxView.ColorProperty.PropertyName)
				UpdateBackgroundColor();
		}

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			// We need an automation peer so we can interact with this in automated tests
			if (Control == null)
			{
				return new FrameworkElementAutomationPeer(this);
			}

			return new FrameworkElementAutomationPeer(Control);
		}

		protected override void UpdateBackgroundColor()
		{
			//background color change must be handled separately
			//because the background would protrude through the border if the corners are rounded
			//as the background would be applied to the renderer's FrameworkElement
			if (Control == null)
				return;

			Color colorToSet = Element.Color;

			if (colorToSet == Color.Default)
				colorToSet = Element.BackgroundColor;
			Control.Background = colorToSet.IsDefault ? null : colorToSet.ToBrush();
		}

		void SetCornerRadius(CornerRadius cornerRadius)
		{
			Control.CornerRadius = new Windows.UI.Xaml.CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft);
		}
	}
}