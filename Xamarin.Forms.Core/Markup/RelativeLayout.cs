using Bounds = System.Linq.Expressions.Expression<System.Func<Xamarin.Forms.Rectangle>>;
using Expression = System.Linq.Expressions.Expression<System.Func<double>>;
using ParentMeasure = System.Func<Xamarin.Forms.RelativeLayout, double>;
using ViewMeasure = System.Func<Xamarin.Forms.RelativeLayout, Xamarin.Forms.View, double>;
using static Xamarin.Forms.Constraint;
using static Xamarin.Forms.Core.Markup.Markup;

namespace Xamarin.Forms.Markup
{
    public static class RelativeLayoutExtensions
    {
		// TODO: Mark obsolete after we implement factory method in MAUI: public static RelativeLayout RelativeLayout(params ConstrainedView?[] constrainedViews) { ... }
        public static TRelativeLayout Children<TRelativeLayout>(this TRelativeLayout layout, params ConstrainedView[] constrainedViews) where TRelativeLayout : RelativeLayout
        {
			VerifyExperimental();
            foreach (var constrainedView in constrainedViews) constrainedView?.AddTo(layout);
            return layout;
        }
	}

    public static class ConstrainViewExtensions
    {
        public static UnconstrainedView Unconstrained<TView>(this TView view) where TView : View => new UnconstrainedView(view);

        public static ConstraintsConstrainedView Constraints<TView>(this TView view) where TView : View => new ConstraintsConstrainedView(view);

		public static BoundsConstrainedView Constrain<TView>(this TView view, Bounds bounds) where TView : View => new BoundsConstrainedView(view, bounds);

		public static ExpressionsConstrainedView Constrain<TView>(this TView view) where TView : View => new ExpressionsConstrainedView(view);
	}

	public class UnconstrainedView : ConstrainedView
    {
        public UnconstrainedView(View view) : base(view) { }

        public override void AddTo(RelativeLayout layout) => ((Layout<View>)layout).Children.Add(view);
    }

    public class BoundsConstrainedView : ConstrainedView
    {
        readonly Bounds _bounds;

        public BoundsConstrainedView(View view, Bounds bounds) : base(view) { _bounds = bounds; }

        public override void AddTo(RelativeLayout layout) => layout.Children.Add(view, _bounds);
    }

    public class ExpressionsConstrainedView : ConstrainedView
    {
        Expression _x, _y, _width, _height;

        public ExpressionsConstrainedView(View view) : base(view) { }

        public ExpressionsConstrainedView X     (Expression x     ) { _x      = x;      return this; }
        public ExpressionsConstrainedView Y     (Expression y     ) { _y      = y;      return this; }
        public ExpressionsConstrainedView Width (Expression width ) { _width  = width;  return this; }
        public ExpressionsConstrainedView Height(Expression height) { _height = height; return this; }

        public override void AddTo(RelativeLayout layout) => layout.Children.Add(view, _x, _y, _width, _height);
    }

    public class ConstraintsConstrainedView : ConstrainedView
    {
        Constraint _x, _y, _width, _height;

        public ConstraintsConstrainedView(View view) : base(view) { }

        public ConstraintsConstrainedView X     (double x)      { _x      = Constant(x); return this; }
        public ConstraintsConstrainedView Y     (double y)      { _y      = Constant(y); return this; }
        public ConstraintsConstrainedView Width (double width)  { _width  = Constant(width); return this; }
        public ConstraintsConstrainedView Height(double height) { _height = Constant(height); return this; }

        public ConstraintsConstrainedView X     (ParentMeasure x     ) { _x      = RelativeToParent(x); return this; }
        public ConstraintsConstrainedView Y     (ParentMeasure y     ) { _y      = RelativeToParent(y); return this; }
        public ConstraintsConstrainedView Width (ParentMeasure width ) { _width  = RelativeToParent(width); return this; }
        public ConstraintsConstrainedView Height(ParentMeasure height) { _height = RelativeToParent(height); return this; }
               
        public ConstraintsConstrainedView X     (View view, ViewMeasure x     ) { _x      = RelativeToView(view, x     ); return this; }
        public ConstraintsConstrainedView Y     (View view, ViewMeasure y     ) { _y      = RelativeToView(view, y     ); return this; }
        public ConstraintsConstrainedView Width (View view, ViewMeasure width ) { _width  = RelativeToView(view, width ); return this; }
        public ConstraintsConstrainedView Height(View view, ViewMeasure height) { _height = RelativeToView(view, height); return this; }

        public override void AddTo(RelativeLayout layout) => layout.Children.Add(view, _x, _y, _width, _height);
    }

    public abstract class ConstrainedView
    {
        readonly protected View view;

		protected ConstrainedView(View view) { VerifyExperimental(constructorHint: nameof(ConstrainedView)); this.view = view; }

        public abstract void AddTo(RelativeLayout layout);
    }
}