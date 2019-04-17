using System;
using Windows.UI.Xaml;

namespace Xamarin.Forms.Platform.UWP
{
	internal class ShellHeaderRenderer : Windows.UI.Xaml.Controls.ContentControl
	{
		public ShellHeaderRenderer(Shell element)
		{
			SetElement(element);
			SizeChanged += ShellHeaderRenderer_SizeChanged;
		}

		void ShellHeaderRenderer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (Element is Layout layout)
				layout.ForceLayout();
		}

		internal VisualElement Element { get; private set; }

		public void SetElement(Shell shell)
		{
			if (Element != null)
			{
				Element.MeasureInvalidated -= RootElementOnMeasureInvalidated;
				Element = null;
			}
			var header = shell.FlyoutHeader;
			if (header is VisualElement visualElement)
			{
				Element = visualElement;
				visualElement.MeasureInvalidated += RootElementOnMeasureInvalidated;
				Content = Platform.CreateRenderer(visualElement).ContainerElement;
			}
			else
			{
				var content = new ItemContentControl()
				{
					FormsDataContext = shell.FlyoutHeader,
					FormsDataTemplate = shell.FlyoutHeaderTemplate
				};
				Content = content;
			}
		}

		void RootElementOnMeasureInvalidated(object sender, EventArgs e)
		{
			InvalidateMeasure();
		}

		protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
		{
			if (Element == null)
			{
				return base.MeasureOverride(availableSize);
			}

			Size request = Element.Measure(availableSize.Width, availableSize.Height, MeasureFlags.IncludeMargins).Request;
			Element.Layout(new Rectangle(Point.Zero, request));

			return new Windows.Foundation.Size(request.Width, request.Height);
		}

		protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
		{
			if (!(Content is FrameworkElement frameworkElement) || Element == null)
			{
				return finalSize;
			}
			frameworkElement.Arrange(new Windows.Foundation.Rect(Element.X, Element.Y, Element.Width, Element.Height));
			return base.ArrangeOverride(finalSize);
		}
	}
}
