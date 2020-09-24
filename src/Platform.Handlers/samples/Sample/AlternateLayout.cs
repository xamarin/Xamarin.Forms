using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Platform;

namespace Sample
{
	public class AlternateView : IView
	{
		public bool IsEnabled => true;

		public Color BackgroundColor => Color.Transparent;

		public Rectangle Frame { get; set; }

		public IViewHandler Handler { get; set; }

		public IFrameworkElement Parent { get; set; }

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
			InvalidateMeasure();
		}

		public void InvalidateArrange()
		{
			IsArrangeValid = false;
		}

		public virtual SizeRequest Measure(double widthConstraint, double heightConstraint)
		{
			if (!IsMeasureValid)
			{
				DesiredSize = Handler.GetDesiredSize(widthConstraint, heightConstraint);
			}

			IsMeasureValid = true;
			return DesiredSize;
		}
	}

	public abstract class AlternateLayout : AlternateView, Xamarin.Platform.ILayout, IEnumerable<IView>
	{
		public IList<IView> Children { get; } = new List<IView>();

		public void Add(IView view)
		{
			if (view == null)
				return;

			Children.Add(view);
		}

		public IEnumerator<IView> GetEnumerator() => Children.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
	}

	public class AlternateStackLayout : AlternateLayout, IStackLayout
	{
		public override SizeRequest Measure(double widthConstraint, double heightConstraint)
		{
			if (IsMeasureValid)
			{
				return DesiredSize;
			}

			//DesiredSize = Stack.MeasureStack(widthConstraint, heightConstraint, Children, Orientation);

			IsMeasureValid = true;

			return DesiredSize;
		}

		public override void Arrange(Rectangle bounds)
		{
			if (IsArrangeValid)
			{
				return;
			}

			//Stack.ArrangeStack(bounds, Children, Orientation);
			IsArrangeValid = true;
			Handler?.SetFrame(bounds);
		}

		bool isMeasureValid;
		public override bool IsMeasureValid
		{
			get
			{
				return isMeasureValid
					&& Children.All(child => child.IsMeasureValid);
			}

			protected set
			{
				isMeasureValid = value;
			}
		}

		public Orientation Orientation { get; set; } = Orientation.Vertical;
	}
}