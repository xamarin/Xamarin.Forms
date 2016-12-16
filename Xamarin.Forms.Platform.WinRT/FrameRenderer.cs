﻿using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public class FrameRenderer : ViewRenderer<Frame, Border>
	{
		public FrameRenderer()
		{
			AutoPackage = false;
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
			else if (e.PropertyName == Frame.OutlineColorProperty.PropertyName || e.PropertyName == Frame.HasShadowProperty.PropertyName)
			{
				UpdateBorder();
			}
			else if (e.PropertyName == Frame.CornerRadiusProperty.PropertyName)
			{
				UpdateCornerRadius();
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
			if (Element.OutlineColor != Color.Default)
			{
				Control.BorderBrush = Element.OutlineColor.ToBrush();
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

			Control.CornerRadius = new CornerRadius(cornerRadius);
		}
	}
}