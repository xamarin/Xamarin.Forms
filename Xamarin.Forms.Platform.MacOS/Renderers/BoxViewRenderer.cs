using System;
using System.ComponentModel;
using AppKit;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;

namespace Xamarin.Forms.Platform.MacOS
{
	public class BoxViewRenderer : ViewRenderer<BoxView, FormsNSView>
	{
		NSColor _colorToRenderer;

		SizeF _previousSize;


		//public override void Layout()
		//{
		//	if (_previousSize != Bounds.Size)
		//		SetNeedsDisplayInRect();
		//}

		//public override void LayoutSubviews()
		//{

		//}
		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new FormsNSView(false, RectangleF.Empty) { });
				}
				SetBackgroundColor(Element.Color);
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == BoxView.ColorProperty.PropertyName)
				SetBackgroundColor(Element.BackgroundColor);
			else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && Element.IsVisible)
				SetNeedsDisplayInRect(Bounds);
		}

		protected override void SetBackgroundColor(Color color)
		{
			if (Element == null)
				return;

			Control.BackgroundColor = color.ToNSColor();
		}
	}
}

