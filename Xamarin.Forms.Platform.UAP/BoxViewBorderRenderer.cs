﻿using System.ComponentModel;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using WShape = Windows.UI.Xaml.Shapes.Shape;

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
					var rect = new Border
					{
						DataContext = Element
					};

					SetNativeControl(rect);
				}

				SetColor(Element.Color);
				SetCornerRadius(Element.CornerRadius);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == BoxView.ColorProperty.PropertyName)
				SetColor(Element.Color);
			else if (e.PropertyName == BoxView.CornerRadiusProperty.PropertyName)
				SetCornerRadius(Element.CornerRadius);
			else if (e.PropertyName == BoxView.ColorProperty.PropertyName)
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
			// BackgroundColor change must be handled separately	
			// because the background would protrude through the border if the corners are rounded
			// as the background would be applied to the renderer's FrameworkElement
			if (Control == null)
				return;
			Color backgroundColor = Element.Color;
			if (backgroundColor.IsDefault)
			{
				backgroundColor = Element.BackgroundColor;
			}

			Control.Background = backgroundColor.IsDefault ? null : backgroundColor.ToBrush();
		}

		protected override void UpdateBackground()
		{
			if (Control == null)
				return;

			Brush background = Element.Background;

			if (Brush.IsNullOrEmpty(background))
			{
				Color backgroundColor = Element.BackgroundColor;

				if (!backgroundColor.IsDefault)
					Control.Background = backgroundColor.ToBrush();
				else
				{
					if (Element.Color.IsDefault)
						Control.Background = null;
				}
			}
			else
				Control.Background = background.ToBrush();
		}

		void SetColor(Color color)
		{
			if (color.IsDefault)
				UpdateBackground();
			else
				Control.Background = color.ToBrush();
		}

		void SetCornerRadius(CornerRadius cornerRadius)
		{
			Control.CornerRadius = new Windows.UI.Xaml.CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft);
		}
	}
}