using System;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public interface IVisualElementRenderer : IDisposable, IRegisterable
	{
		VisualElement Element { get; }

		FormsNSView NativeView { get; }

		NSViewController ViewController { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);

		void SetElementSize(Size size);
	}
}