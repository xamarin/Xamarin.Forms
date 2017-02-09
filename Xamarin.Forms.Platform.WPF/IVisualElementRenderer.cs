using System;
using System.Windows;

namespace Xamarin.Forms.Platform.WPF
{
	public interface IVisualElementRenderer : IRegisterable
	{
		UIElement ContainerElement { get; }

		VisualElement Element { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);
	}
}