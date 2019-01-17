using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Support.V4.View;
using System.ComponentModel;

using AViews = Android.Views;
using System.Collections.Specialized;
using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using Android.App;
using AShapeType = Android.Graphics.Drawables.ShapeType;
using AImageViewCompat = Android.Support.V4.Widget.ImageViewCompat;
using AColorStateList = Android.Content.Res.ColorStateList;
using AndroidAppCompat = Android.Support.V7.Content.Res.AppCompatResources;
using Android.Graphics;
using Android.Support.V7.Widget;

namespace Xamarin.Forms.Platform.Android
{

	public class IndicatorsDecoration: global::Android.Support.V7.Widget.RecyclerView.ItemDecoration
	{
		PageIndicator _pageIndicator;
		float mIndicatorItemLength = 10;
		float mIndicatorItemPadding = 3;
		int indicatorHeight = 200;
		public IndicatorsDecoration(PageIndicator indicator)
		{
			_pageIndicator = indicator;

			_pageIndicator.SetBackgroundColor(Xamarin.Forms.Color.Red.ToAndroid());
		}

		public override void GetItemOffsets(Rect outRect, AViews.View view, RecyclerView parent, RecyclerView.State state)
		{
			base.GetItemOffsets(outRect, view, parent, state);
			outRect.Bottom = indicatorHeight;
			_pageIndicator.Layout(0, parent.Height - indicatorHeight, parent.Width, parent.Height);

		}

		public override void OnDrawOver(Canvas c, RecyclerView parent, RecyclerView.State state)
		{
			base.OnDrawOver(c, parent, state);
			int itemCount = parent.GetAdapter().ItemCount;

			// center horizontally, calculate width and subtract half from center
			float totalLength = mIndicatorItemLength * itemCount;
			float paddingBetweenItems = Math.Max(0, itemCount - 1) * mIndicatorItemPadding;
			float indicatorTotalWidth = totalLength + paddingBetweenItems;
			float indicatorStartX = (parent.Width - indicatorTotalWidth) / 2F;

			// center vertically in the allotted space
			float indicatorPosY = parent.Height - indicatorHeight / 2F;
			_pageIndicator.UpdateIndicatorCount();
			//_pageIndicator.Draw(c);
			drawInactiveIndicators(c, indicatorStartX, indicatorPosY, itemCount);
		}

		void drawInactiveIndicators(Canvas c, float indicatorStartX, float indicatorPosY, int itemCount)
		{
			var mPaint = new Paint();
			mPaint.Color = Color.Yellow.ToAndroid();
			

			// width of item indicator including padding
			var itemWidth = mIndicatorItemLength + mIndicatorItemPadding;

			float start = indicatorStartX;
			for (int i = 0; i < itemCount; i++)
			{
				// draw the line for every item
				c.DrawLine(start, indicatorPosY,
					start + mIndicatorItemLength, indicatorPosY, mPaint);
				start += itemWidth;
			}
		}
	}

	public class CarouselViewRenderer : ItemsViewRenderer
	{
		// TODO hartez 2018/08/29 17:13:17 Does this need to override SelectLayout so it ignores grids?	(Yes, and so it can warn on unknown layouts)
		Context _context;

		PageIndicator _pageIndicator;

		CarouselView Carousel => Element as CarouselView;

		public CarouselViewRenderer(Context context) : base(context)
		{
			_context = context;
			_pageIndicator = new PageIndicator(_context, null);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> elementChangedEvent)
		{
			var oldElement = elementChangedEvent.OldElement;
			var newElement = elementChangedEvent.NewElement;
			//if (newElement != null)
			//{
			//	AddView(_pageIndicator);
			//}


			AddItemDecoration(new IndicatorsDecoration(_pageIndicator));
			UpdateSpacing();
			UpdateIsSwipeEnabled();
			UpdateIndicators();
		}

		private void UpdateSpacing()
		{

		}

		protected override void UpdateItemsSource()
		{
			if (ItemsView == null)
			{
				return;
			}

			// By default the CollectionViewAdapter creates the items at whatever size the template calls for
			// But for the Carousel, we want it to create the items to fit the width/height of the viewport
			// So we give it an alternate delegate for creating the views

			ItemsViewAdapter = new ItemsViewAdapter(ItemsView, 
				(view, context) => new SizedItemContentView(context, () => Width, () => Height));

			SwapAdapter(ItemsViewAdapter, false);
			_pageIndicator.UpdateRecyclerView(this);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == CarouselView.IsSwipeEnabledProperty.PropertyName)
				UpdateIsSwipeEnabled();
			else if (e.PropertyName == CarouselView.IndicatorsTintColorProperty.PropertyName)
				UpdateIndicatorsTintColor();
			else if (e.PropertyName == CarouselView.SelectedIndicatorTintColorProperty.PropertyName)
				UpdateSelectedIndicatorsTintColor();
			else if (e.PropertyName == CarouselView.IndicatorsShapeProperty.PropertyName)
				UpdateIndicatorsShape();
			else if (e.PropertyName == CarouselView.IndicatorsVisibilityProperty.PropertyName)
				UpdateIndicators();
		}

		void UpdateIndicatorsTintColor()
		{

			_pageIndicator?.UpdatePageIndicatorTintColor(Carousel.IndicatorsTintColor.ToAndroid());
			UpdateIndicatorsShape();
		}

		void UpdateIsSwipeEnabled()
		{
			//((IViewPager)_viewPager)?.SetPagingEnabled(Element.IsSwipeEnabled);
		}

		void UpdateSelectedIndicatorsTintColor()
		{
			_pageIndicator?.UpdateCurrentPageIndicatorTintColor(Carousel.SelectedIndicatorTintColor.ToAndroid());
			UpdateIndicatorsShape();
		}

		void UpdateIndicatorsShape()
		{
			_pageIndicator?.UpdateShapeType(Carousel.IndicatorsShape == IndicatorsShape.Circle ? AShapeType.Oval : AShapeType.Rectangle);
		}

		void UpdateIndicators()
		{
			if (Carousel.IndicatorsVisibility != IndicatorVisibility.Hidden)
			{
				_pageIndicator.Visibility = AViews.ViewStates.Visible;
				UpdateIndicatorsTintColor();
				UpdateSelectedIndicatorsTintColor();
				UpdateIndicatorsShape();
			}
			else
			{
				_pageIndicator.Visibility = AViews.ViewStates.Gone;
			}

			_pageIndicator.UpdateIndicatorCount();
			UpdateIndicatorPosition();
		}

		void UpdateIndicatorPosition()
		{

		}
	}
}