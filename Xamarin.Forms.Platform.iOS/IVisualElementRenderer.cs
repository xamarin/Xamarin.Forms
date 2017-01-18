using System;
#if __MOBILE__
using UIKit;
namespace Xamarin.Forms.Platform.iOS
#else
using UIView = AppKit.NSView;
using UIViewController = AppKit.NSViewController;

namespace Xamarin.Forms.Platform.MacOS
#endif
{
	public interface IVisualElementRenderer : IDisposable, IRegisterable
	{
		VisualElement Element { get; }

		UIView NativeView { get; }

		UIViewController ViewController { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);

		void SetElementSize(Size size);
	}
}