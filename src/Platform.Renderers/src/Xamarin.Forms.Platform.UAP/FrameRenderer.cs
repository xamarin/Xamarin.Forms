﻿using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public class FrameRenderer : ViewRenderer<Frame, Border>
	{
		public FrameRenderer()
		{
			AutoPackage = false;
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

		protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
					SetNativeControl(new Border());

				PackChild();
				UpdateBorder();
				UpdateCornerRadius();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == "Content")
			{
				PackChild();
			}
			else if (e.PropertyName == Frame.BorderColorProperty.PropertyName || e.PropertyName == Frame.HasShadowProperty.PropertyName)
			{
				UpdateBorder();
			}
			else if (e.PropertyName == Frame.CornerRadiusProperty.PropertyName)
			{
				UpdateCornerRadius();
			}
		}

		protected override void UpdateBackgroundColor()
		{
			// Background color change must be handled separately
			// because the background would protrude through the border if the corners are rounded
			// as the background would be applied to the renderer's FrameworkElement
			Color backgroundColor = Element.BackgroundColor;

			if (Control != null)
			{
				Control.Background = backgroundColor.IsDefault ? null : backgroundColor.ToBrush();
			}
		}

		protected override void UpdateBackground()
		{
			Color backgroundColor = Element.BackgroundColor;
			Brush background = Element.Background;

			if (Control != null)
			{
				if (Brush.IsNullOrEmpty(background))
					Control.Background = backgroundColor.IsDefault ? null : backgroundColor.ToBrush();
				else
					Control.Background = background.ToBrush();
			}
		}

		void PackChild()
		{
			if (Element.Content == null)
				return;

			IVisualElementRenderer renderer = Element.Content.GetOrCreateRenderer();
			Control.Child = renderer.ContainerElement;
		}

		void UpdateBorder()
		{
			if (Element.BorderColor != Color.Default)
			{
				Control.BorderBrush = Element.BorderColor.ToBrush();
				Control.BorderThickness = new Windows.UI.Xaml.Thickness(1);
			}
			else
			{
				Control.BorderBrush = new Color(0, 0, 0, 0).ToBrush();
			}
		}

		void UpdateCornerRadius()
		{
			float cornerRadius = Element.CornerRadius;

			if (cornerRadius == -1f)
				cornerRadius = 5f; // default corner radius

			Control.CornerRadius = new Windows.UI.Xaml.CornerRadius(cornerRadius);
		}
	}
}