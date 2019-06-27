using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;

namespace Xamarin.Forms.Platform.Android
{
	public class CarouselViewRenderer : ItemsViewRenderer
	{
		//should this move to ItemsViewREnderer and be shared ?
		class ScrollListener : global::Android.Support.V7.Widget.RecyclerView.OnScrollListener
		{
			CarouselViewRenderer _renderer;
			int _oldPosition;
			int _initialPosition;
			bool _scrollingToInitialPosition = true;

			public ScrollListener(CarouselViewRenderer renderer, int initialPosition)
			{
				_renderer = renderer;
				_initialPosition = initialPosition;
			}

			public override void OnScrolled(global::Android.Support.V7.Widget.RecyclerView recyclerView, int dx, int dy)
			{
				base.OnScrolled(recyclerView, dx, dy);
				var layoutManager = (recyclerView.GetLayoutManager() as LinearLayoutManager);
				var adapterPosition = layoutManager.FindFirstVisibleItemPosition();
				if (_scrollingToInitialPosition)
				{
					_scrollingToInitialPosition = !(_initialPosition == adapterPosition);
					return;
				}

				if (_oldPosition != adapterPosition)
				{
					_oldPosition = adapterPosition;
					_renderer.UpdatePosition(adapterPosition);
				}
			}
		}

		// TODO hartez 2018/08/29 17:13:17 Does this need to override SelectLayout so it ignores grids?	(Yes, and so it can warn on unknown layouts)
		Context _context;
		ScrollListener _scrollListener;
		protected CarouselView Carousel;
		bool _isSwipeEnabled;
		bool _isUpdatingPositionFromForms;

		public CarouselViewRenderer(Context context) : base(context)
		{
			_context = context;

		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_scrollListener != null)
				{
					RemoveOnScrollListener(_scrollListener);
					_scrollListener.Dispose();
					_scrollListener = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override void SetUpNewElement(ItemsView newElement)
		{
			base.SetUpNewElement(newElement);
			if (newElement == null)
			{
				Carousel = null;
				return;
			}

			Carousel = newElement as CarouselView;

			UpdateSpacing();
			UpdateIsSwipeEnabled();
			_isUpdatingPositionFromForms = true;
			//Goto to the Correct Position
			Carousel.ScrollTo(Carousel.Position);
			_isUpdatingPositionFromForms = false;
			_scrollListener = new ScrollListener(this, Carousel.Position);
			AddOnScrollListener(_scrollListener);
		}

		protected override void UpdateItemsSource()
		{

			// By default the CollectionViewAdapter creates the items at whatever size the template calls for
			// But for the Carousel, we want it to create the items to fit the width/height of the viewport
			// So we give it an alternate delegate for creating the views

			ItemsViewAdapter = new ItemsViewAdapter(ItemsView, 
				(view, context) => new SizedItemContentView(context, () => Width, () => Height));

			SwapAdapter(ItemsViewAdapter, false);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == CarouselView.IsSwipeEnabledProperty.PropertyName)
				UpdateIsSwipeEnabled();
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			//TODO: this doesn't work because we need to interact with the Views
			if (!_isSwipeEnabled)
			{
				return false;
			}
			return base.OnTouchEvent(e);
		}

		void UpdateIsSwipeEnabled()
		{
			_isSwipeEnabled = Carousel.IsSwipeEnabled;
		}

		void UpdatePosition(int position)
		{
			if (position == -1 || _isUpdatingPositionFromForms)
				return;

			var context = ItemsViewAdapter?.ItemsSource[position];
			if (context == null)
				throw new InvalidOperationException("Visible item not found");

			Carousel.SetCurrentItem(context);
		}

		void UpdateSpacing()
		{

		}
	}
}