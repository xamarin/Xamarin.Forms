
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
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
	public class PageIndicator : LinearLayout, AViewPager.IOnPageChangeListener
	{
		AViewPager _pager;
		int _selectedIndex = 0;
		AColor _currentPageIndicatorTintColor;
		ShapeType _shapeType = ShapeType.Oval;
		Drawable _currentPageShape = null;
		Drawable _pageShape = null;
		AColor _pageIndicatorTintColor;
		bool IsVisible => Visibility != ViewStates.Gone;

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
			if (!IsVisible) return;

			var count = _pager.Adapter.Count;
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
			if (!IsVisible) return;

			_pageShape = null;
			_currentPageShape = null;
			UpdateShapes();
			UpdateIndicators();
		}

		internal void UpdateIndicators()
		{
			if (!IsVisible) return;

			var count = ChildCount;
			for (int i = 0; i < count; i++)
			{
				ImageView view = (ImageView)GetChildAt(i);
				var drawableToUse = _selectedIndex == i ? _currentPageShape : _pageShape;
				if (drawableToUse != view.Drawable)
					view.SetImageDrawable(drawableToUse);
			}
		}

		internal void UpdateViewPager(AViewPager pager)
		{
			_pager = pager;
			_pager.AddOnPageChangeListener(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_pager != null)
				{
					_pager.RemoveOnPageChangeListener(this);
					_pager = null;
				}
			}
			base.Dispose(disposing);
		}


		void UpdateShapes()
		{
			if (_currentPageShape != null) return;

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
			shape.Paint.Color = color;

			return shape;
		}




		void AViewPager.IOnPageChangeListener.OnPageScrollStateChanged(int state)
		{
		}

		void AViewPager.IOnPageChangeListener.OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{
			_selectedIndex = position;
			UpdateIndicators();
		}

		void AViewPager.IOnPageChangeListener.OnPageSelected(int position)
		{
		}

	}
}