using Xamarin.Forms;

namespace Xamarin.Platform
{
	public abstract class View : IView
	{
		public bool IsEnabled => true;

		public Color BackgroundColor => Color.Transparent;

		public Rectangle Frame { get; set; }

		public IViewHandler? Handler { get; set; }

		public IFrameworkElement? Parent { get; set; }

		public SizeRequest DesiredSize { get; protected set; }

		public virtual bool IsMeasureValid { get; protected set; }

		public bool IsArrangeValid { get; protected set; }

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

		public virtual SizeRequest Measure(double widthConstraint, double heightConstraint)
		{
			if (!IsMeasureValid)
			{
				if (Handler == null)
				{
					DesiredSize = new SizeRequest(Size.Zero, Size.Zero);
				}
				else
				{
					DesiredSize = Handler.GetDesiredSize(widthConstraint, heightConstraint);
				}
			}

			IsMeasureValid = true;
			return DesiredSize;
		}
	}
}
