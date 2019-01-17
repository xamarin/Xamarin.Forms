using Android.Content;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using AView = Android.Views.View;
using AColor = Android.Graphics.Color;
using AViewPager = Android.Support.V4.View.ViewPager;
using Android.Support.V4.View;
using AShapes = Android.Graphics.Drawables.Shapes;

namespace Xamarin.Forms.Platform.Android
{
	internal class ScrollListener : global::Android.Support.V7.Widget.RecyclerView.OnScrollListener
	{
		public ScrollListener(PageIndicator indicator)
		{

		}

		public override void OnScrolled(global::Android.Support.V7.Widget.RecyclerView recyclerView, int dx, int dy)
		{
			base.OnScrolled(recyclerView, dx, dy);
		}
		//var midPos = 0;
		//var scrollX = 0;

		//override fun onScrolled(recyclerView: RecyclerView, dx: Int, dy: Int)
		//{
		//	super.onScrolled(recyclerView, dx, dy)

		//scrollX += dx

		//recyclerView.getChildAt(0)?.width?.let {
		//	val midPos = Math.floor(((scrollX + it / 2f) / it).toDouble()).toInt()

		// if (this.midPos != midPos)
		//	{
		//		when {
		//			this.midPos < midPos->indicator.swipeNext()
		//          else -> indicator.swipePrevious()


		//}
		//}
		//this.midPos = midPos
	}

	public class PageIndicator : LinearLayout
	{
		global::Android.Support.V7.Widget.RecyclerView _recycler;
		int _selectedIndex = 0;
		AColor _currentPageIndicatorTintColor;
		ShapeType _shapeType = ShapeType.Oval;
		Drawable _currentPageShape = null;
		Drawable _pageShape = null;
		AColor _pageIndicatorTintColor;
		bool IsVisible => Visibility != ViewStates.Gone;
		ScrollListener scrollListener;

		public PageIndicator(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		internal void UpdatePageIndicatorTintColor(AColor value)
		{
			_pageIndicatorTintColor = value;
			ResetIndicators();
		}

		internal void UpdateCurrentPageIndicatorTintColor(AColor value)
		{
			_currentPageIndicatorTintColor = value;
			ResetIndicators();
		}


		internal void UpdateShapeType(ShapeType value)
		{
			_shapeType = value;
			ResetIndicators();
		}

		internal void UpdateIndicatorCount()
		{
			if (!IsVisible)
				return;

			var count = _recycler.GetAdapter().ItemCount;
			var childCount = ChildCount;

			for (int i = ChildCount; i < count; i++)
			{
				var imageView = new ImageView(Context);
				if (Orientation == Orientation.Horizontal)
					imageView.SetPadding((int)Context.ToPixels(4), 0, (int)Context.ToPixels(4), 0);
				else
					imageView.SetPadding(0, (int)Context.ToPixels(4), 0, (int)Context.ToPixels(4));

				imageView.SetImageDrawable(_selectedIndex == i ? _currentPageShape : _pageShape);
				AddView(imageView);
			}

			childCount = ChildCount;

			for (int i = count; i < childCount; i++)
			{
				RemoveViewAt(ChildCount - 1);
			}
		}

		internal void ResetIndicators()
		{
			if (!IsVisible)
				return;

			_pageShape = null;
			_currentPageShape = null;
			UpdateShapes();
			UpdateIndicators();
		}

		internal void UpdateIndicators()
		{
			if (!IsVisible)
				return;

			var count = ChildCount;
			for (int i = 0; i < count; i++)
			{
				ImageView view = (ImageView)GetChildAt(i);
				var drawableToUse = _selectedIndex == i ? _currentPageShape : _pageShape;
				if (drawableToUse != view.Drawable)
					view.SetImageDrawable(drawableToUse);
			}
		}

		internal void UpdateRecyclerView(global::Android.Support.V7.Widget.RecyclerView recycler)
		{
			if (_recycler != null)
				_recycler.RemoveOnScrollListener(scrollListener);
			scrollListener = new ScrollListener(this);
			_recycler = recycler;
			_recycler.AddOnScrollListener(scrollListener);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_recycler != null)
				{
					if (scrollListener != null)
						_recycler.RemoveOnScrollListener(scrollListener);
					_recycler = null;
				}
			}
			base.Dispose(disposing);
		}


		void UpdateShapes()
		{
			if (_currentPageShape != null)
				return;

			_currentPageShape = GetCircle(_currentPageIndicatorTintColor);
			_pageShape = GetCircle(_pageIndicatorTintColor);
		}

		Drawable GetCircle(AColor color)
		{
			ShapeDrawable shape = null;

			if (_shapeType == ShapeType.Oval)
				shape = new ShapeDrawable(new AShapes.OvalShape());
			else
				shape = new ShapeDrawable(new AShapes.RectShape());

			shape.SetIntrinsicHeight((int)Context.ToPixels(6));
			shape.SetIntrinsicWidth((int)Context.ToPixels(6));
			shape.Paint.Color = Xamarin.Forms.Color.Pink.ToAndroid();

			return shape;
		}


		//void IOnScrollChangeListener.OnScrollChange(AView v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
		//{
		//	var position = 1;
		//	_selectedIndex = position;
		//	UpdateIndicators();
		//}
	}
}
