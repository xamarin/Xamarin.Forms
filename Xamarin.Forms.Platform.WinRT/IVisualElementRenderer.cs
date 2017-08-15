using System;
using Windows.UI.Xaml;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public interface IVisualElementRenderer : IRegisterable, IDisposable
	{
		FrameworkElement ContainerElement { get; }

		VisualElement Element { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);

		UIElement GetNativeElement();
	}

	public sealed class VisualElementRenderer : IVisualElementRenderer
	{
		private IPlatformRenderer _platformRenderer;

		public VisualElementRenderer(IPlatformRenderer platformRenderer)
		{
			_platformRenderer = platformRenderer;

			_platformRenderer.ElementChanged += OnElementChanged;
		}

		private void OnElementChanged(object sender, ElementChangedEventArgs e)
			=> ElementChanged(sender, new VisualElementChangedEventArgs(
				(VisualElement)e.OldElement, 
				(VisualElement)e.NewElement)
			);

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public FrameworkElement ContainerElement
			=> _platformRenderer.ContainerElement;

		public void SetElement(VisualElement element)
			=> _platformRenderer.SetElement(element);

		public VisualElement Element
			=> _platformRenderer.Element;

		public UIElement GetNativeElement()
			=> _platformRenderer.Control;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
			=> _platformRenderer.Measure(widthConstraint, heightConstraint);

		public void Dispose()
			=> _platformRenderer.Dispose();
	}
}