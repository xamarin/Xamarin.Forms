using System;
using Microsoft.Maui;

namespace Microsoft.Maui.Tests
{
	class ViewStub : IViewStub
	{
		public bool IsEnabled => throw new NotImplementedException();

		public Color BackgroundColor => throw new NotImplementedException();

		public Rectangle Frame => throw new NotImplementedException();

		public IViewHandler Handler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public IFrameworkElement Parent => throw new NotImplementedException();

		public Size DesiredSize => throw new NotImplementedException();

		public bool IsMeasureValid => throw new NotImplementedException();

		public bool IsArrangeValid => throw new NotImplementedException();

		public double Width => throw new NotImplementedException();

		public double Height => throw new NotImplementedException();

		public Thickness Margin => throw new NotImplementedException();

		public void Arrange(Rectangle bounds)
		{
			throw new NotImplementedException();
		}

		public void InvalidateArrange()
		{
			throw new NotImplementedException();
		}

		public void InvalidateMeasure()
		{
			throw new NotImplementedException();
		}

		public Size Measure(double widthConstraint, double heightConstraint)
		{
			throw new NotImplementedException();
		}
	}
}
