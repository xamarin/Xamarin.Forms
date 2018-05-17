using System;

using UIKit;
using CoreGraphics;

namespace Xamarin.Forms.Platform.iOS
{
	public class NativePageWrapperRenderer : IVisualElementRenderer {

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public NativePageWrapper Element { get; private set; }

		VisualElement IVisualElementRenderer.Element => Element;

		public UIView NativeView => Element?.ViewController.View;

		public UIViewController ViewController => Element?.ViewController;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
		}

		public void SetElement(VisualElement element)
		{
			VisualElement oldElement = Element;
			Element = (NativePageWrapper)element;

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			ElementChanged?.Invoke(this, e);
		}

		public void SetElementSize(Size size)
		{
			NativeView.Frame = new CGRect(0, 0, size.Width, size.Height);
			Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && Element != null) {
				Platform.SetRenderer(Element, null);
				Element = null;
			}
		}

		public void Dispose() => Dispose(true);
	}
}
