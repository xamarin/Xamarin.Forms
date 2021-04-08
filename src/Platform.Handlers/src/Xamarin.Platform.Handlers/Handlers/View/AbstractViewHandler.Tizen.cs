using ElmSharp;
using Xamarin.Platform.Tizen;
using Rectangle = Xamarin.Forms.Rectangle;
using Point = Xamarin.Forms.Point;
using Size = Xamarin.Forms.Size;
using ESize = ElmSharp.Size;

namespace Xamarin.Platform.Handlers
{
	public partial class AbstractViewHandler<TVirtualView, TNativeView> : ITizenViewHandler
	{
		public void SetContext(CoreUIAppContext context) => Context = context;

		public void SetParent(ITizenViewHandler parent) => Parent = parent;

		public CoreUIAppContext? Context { get; private set; }

		public ITizenViewHandler? Parent { get; private set; }

		public EvasObject? NativeParent => Context?.BaseLayout;


		public virtual void SetFrame(Rectangle frame)
		{
			var nativeView = View;

			if (nativeView == null || VirtualView == null)
				return;

			if (frame.Width < 0 || frame.Height < 0)
			{
				// This is just some initial Forms value nonsense, nothing is actually laying out yet
				return;
			}

			if (NativeParent == null)
				return;

			var updatedGeometry = new Rectangle(ComputeAbsolutePoint(frame), new Size(frame.Width, frame.Height)).ToPixel();

			if (nativeView.Geometry != updatedGeometry)
			{
				nativeView.Geometry = updatedGeometry;
			}
		}

		public virtual Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (TypedNativeView == null)
			{
				return Size.Zero;
			}

			if (NativeParent == null)
			{
				return new Size(widthConstraint, heightConstraint);
			}

			int availableWidth = widthConstraint.ToScaledPixel();
			int availableHeight = heightConstraint.ToScaledPixel();

			if (availableWidth < 0)
				availableWidth = int.MaxValue;
			if (availableHeight < 0)
				availableHeight = int.MaxValue;

			Size measured;
			var nativeViewMeasurable = TypedNativeView as IMeasurable;
			if (nativeViewMeasurable != null)
			{
				measured = nativeViewMeasurable.Measure(availableWidth, availableHeight).ToDP();
			}
			else
			{
				measured = Measure(availableWidth, availableHeight).ToDP();
			}
			return measured;
		}

		public virtual Rect GetNativeContentGeometry()
		{
			if (TypedNativeView == null)
			{
				return new Rect();
			}
			return TypedNativeView.Geometry;
		}

		protected virtual ESize Measure(int availableWidth, int availableHeight)
		{
			if (TypedNativeView == null)
			{
				return new ESize(0, 0);
			}
			return new ESize(TypedNativeView.MinimumWidth, TypedNativeView.MinimumHeight);
		}

		protected virtual double ComputeAbsoluteX(Rectangle frame)
		{
			if (Parent != null)
			{
				return frame.X + Parent.GetNativeContentGeometry().X.ToScaledDP();
			}
			else
			{
				return frame.X;
			}
		}

		protected virtual double ComputeAbsoluteY(Rectangle frame)
		{
			if (Parent != null)
			{
				return frame.Y + Parent.GetNativeContentGeometry().Y.ToScaledDP();
			}
			else
			{
				return frame.Y;
			}
		}

		protected virtual Point ComputeAbsolutePoint(Rectangle frame)
		{
			return new Point(ComputeAbsoluteX(frame), ComputeAbsoluteY(frame));
		}

		void SetupContainer()
		{

		}

		void RemoveContainer()
		{

		}
	}
}