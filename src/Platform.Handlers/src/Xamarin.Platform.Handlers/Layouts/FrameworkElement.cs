using System;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public abstract class FrameworkElement : IFrameworkElement
	{
		public bool IsEnabled => true;

		public Color BackgroundColor { get; set; } = Color.Transparent;

		public Rectangle Frame { get; set; }

		public IViewHandler? Handler { get; set; }

		public IFrameworkElement? Parent { get; set; }

		public Size DesiredSize { get; protected set; }

		public virtual bool IsMeasureValid { get; protected set; }

		public bool IsArrangeValid { get; protected set; }

		public double Width { get; set; } = -1;
		public double Height { get; set; } = -1;

		public virtual void Arrange(Rectangle bounds)
		{
			Frame = bounds;
		}

		public void InvalidateMeasure()
		{
			IsMeasureValid = false;
			IsArrangeValid = false;
		}

		public void InvalidateArrange()
		{
			IsArrangeValid = false;
		}

		public virtual Size Measure(double widthConstraint, double heightConstraint)
		{
			if (!IsMeasureValid)
			{
				if (Handler == null)
				{
					DesiredSize = Size.Zero;
				}
				else
				{
					widthConstraint = ResolveConstraints(widthConstraint, Width);
					heightConstraint = ResolveConstraints(heightConstraint, Height);

					DesiredSize = Handler.GetDesiredSize(widthConstraint, heightConstraint);
				}
			}

			IsMeasureValid = true;
			return DesiredSize;
		}

		// These methods will eventually factor in Min/Max values
		public static double ResolveConstraints(double externalConstraint, double desiredLength) 
		{
			if (desiredLength == -1)
			{
				return externalConstraint;
			}

			return Math.Min(externalConstraint, desiredLength);
		}

		public static double ResolveConstraints(double externalConstraint, double desiredLength, double measuredLength) 
		{
			if (desiredLength == -1)
			{
				// No user-specified length, so the measured value will be limited by the external constraint
				return Math.Min(measuredLength, externalConstraint);
			}

			// User-specified length wins, subject to external constraints
			return Math.Min(desiredLength, externalConstraint);
		}
	}
}
