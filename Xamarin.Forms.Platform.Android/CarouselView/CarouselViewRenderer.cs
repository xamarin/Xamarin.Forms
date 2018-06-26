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

namespace Xamarin.Forms.Platform.Android
{
	public class CarouselViewRenderer : ViewRenderer<CarouselView, AViews.View>
	{
		Context _context;

		AViews.View _indicators;
		AViews.View _nativeView;
		ViewPager _viewPager;
		PageIndicator _pageIndicator;
		LinearLayout _prevBtn;
		LinearLayout _nextBtn;

		bool _disposed;
		bool _isChangingPosition;

		// KeyboardService code
		bool _isKeyboardVisible;
		SoftKeyboardService _keyboardService;
		bool _setCurrentPageCalled;
		int _pageScrolledCount;
		ScrollDirection _direction;
		int _mViewPagerState;

		public CarouselViewRenderer(Context context) : base(context)
		{
			_context = context;

			// KeyboardService code
			var activity = _context as Activity;
			if (activity != null)
				_keyboardService = new SoftKeyboardService(activity);
		}

		protected ITemplatedItemsView<View> TemplatedItemsView => Element;

		protected override void OnElementChanged(ElementChangedEventArgs<CarouselView> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				e.OldElement.TemplatedItems.CollectionChanged -= ItemsSource_CollectionChanged;

				// KeyboardService code
				_keyboardService.VisibilityChanged -= KeyboardService_VisibilityChanged;
			}

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var inflater = AViews.LayoutInflater.From(_context);

					// Orientation BP
					if (Element.Orientation == CarouselOrientation.Horizontal)
						_nativeView = inflater.Inflate(Resource.Layout.horizontal_viewpager, null);
					else
						_nativeView = inflater.Inflate(Resource.Layout.vertical_viewpager, null);

					_viewPager = _nativeView.FindViewById<ViewPager>(Resource.Id.pager);
					_pageIndicator = _nativeView.FindViewById<PageIndicator>(Resource.Id.pageIndicator);


					_prevBtn = _nativeView.FindViewById<LinearLayout>(Resource.Id.prev);
					_nextBtn = _nativeView.FindViewById<LinearLayout>(Resource.Id.next);


					ImageView prev = (ImageView)_prevBtn.GetChildAt(0);
					ImageView next = (ImageView)_nextBtn.GetChildAt(0);

					if (Element.Orientation == CarouselOrientation.Horizontal)
					{
						_pageIndicator.Orientation = Orientation.Horizontal;
						prev.SetImageDrawable(AndroidAppCompat.GetDrawable(Context, Resource.Drawable.Prev));
						next.SetImageDrawable(AndroidAppCompat.GetDrawable(Context, Resource.Drawable.Next));
					}
					else
					{
						_pageIndicator.Orientation = Orientation.Vertical;
						prev.SetImageDrawable(AndroidAppCompat.GetDrawable(Context, Resource.Drawable.Up));
						next.SetImageDrawable(AndroidAppCompat.GetDrawable(Context, Resource.Drawable.Down));
					}

					_prevBtn.Click += PrevBtn_Click;
					_nextBtn.Click += NextBtn_Click;

					_pageIndicator.UpdateViewPager(_viewPager);

					_viewPager.PageSelected += ViewPager_PageSelected;
					_viewPager.PageScrollStateChanged += ViewPager_PageScrollStateChanged;
					_viewPager.PageScrolled += ViewPager_PageScrolled;

					// TapGestureRecognizer doesn't work when added to CarouselViewControl (Android) #66, #191, #200
					((IViewPager)_viewPager)?.SetElement(e.NewElement);

					SetNativeControl(_nativeView);
				}

				// Configure the control and subscribe to event handlers
				e.NewElement.TemplatedItems.CollectionChanged += ItemsSource_CollectionChanged;

				// KeyboardService code
				_keyboardService.VisibilityChanged += KeyboardService_VisibilityChanged;
			}

			UpdateBackgroundColor();
			UpdateItemsSource();
			UpdateInterSpacing();
			UpdateIsSwipeEnabled();
			UpdateArrowsVisibility();
			UpdateIndicators();
			UpdateArrowsTransparency();
			UpdateArrowsBackgroundColor();
			UpdateArrowsTintColor();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Element == null || _viewPager == null) return;

			if (e.PropertyName == CarouselView.OrientationProperty.PropertyName)
				UpdateOrientation();
			else if (e.PropertyName == CarouselView.BackgroundColorProperty.PropertyName)
				_viewPager.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
			else if (e.PropertyName == CarouselView.IsSwipeEnabledProperty.PropertyName)
				UpdateIsSwipeEnabled();
			else if (e.PropertyName == CarouselView.IndicatorsTintColorProperty.PropertyName)
				UpdateIndicatorsTintColor();
			else if (e.PropertyName == CarouselView.CurrentPageIndicatorTintColorProperty.PropertyName)
				UpdateIndicatorsCurrentPageTintColor();
			else if (e.PropertyName == CarouselView.IndicatorsShapeProperty.PropertyName)
				UpdateIndicatorsShape();
			else if (e.PropertyName == CarouselView.ShowIndicatorsProperty.PropertyName)
				UpdateIndicators();
			else if (e.PropertyName == CarouselView.ItemsSourceProperty.PropertyName)
				UpdateItemsSource();
			else if (e.PropertyName == CarouselView.PositionProperty.PropertyName)
				UpdatePosition();
			else if (e.PropertyName == CarouselView.ShowArrowsProperty.PropertyName)
				UpdateArrowsVisibility();
			else if (e.PropertyName == CarouselView.ArrowsTintColorProperty.PropertyName)
				UpdateArrowsTintColor();
			else if (e.PropertyName == CarouselView.ArrowsTransparencyProperty.PropertyName)
				UpdateArrowsTransparency();
			else if (e.PropertyName == CarouselView.ArrowsBackgroundColorProperty.PropertyName)
				UpdateArrowsBackgroundColor();
		}

		async void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (Element == null || _viewPager == null) return;

			// NewItems contains the item that was added.
			// If NewStartingIndex is not -1, then it contains the index where the new item was added.
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				var item = GetItem(e.NewStartingIndex);
				InsertPage(item, e.NewStartingIndex);
			}

			// OldItems contains the item that was removed.
			// If OldStartingIndex is not -1, then it contains the index where the old item was removed.
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				await RemovePage(e.OldStartingIndex);
			}

			// OldItems contains the moved item.
			// OldStartingIndex contains the index where the item was moved from.
			// NewStartingIndex contains the index where the item was moved to.
			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				// Fix for #168 Android NullReferenceException
				var Source = ((CarouselViewPagerAdapter)_viewPager?.Adapter)?.Source;

				if (_viewPager?.Adapter == null || Source == null) return;

				Source.RemoveAt(e.OldStartingIndex);
				Source.Insert(e.NewStartingIndex, e.OldItems[e.OldStartingIndex]);
				_viewPager.Adapter?.NotifyDataSetChanged();

				UpdateArrowsVisibility();
			}

			// NewItems contains the replacement item.
			// NewStartingIndex and OldStartingIndex are equal, and if they are not -1,
			// then they contain the index where the item was replaced.
			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				// Fix for #168 Android NullReferenceException
				var Source = ((CarouselViewPagerAdapter)_viewPager?.Adapter)?.Source;

				if (_viewPager?.Adapter == null || Source == null) return;

				Source[e.OldStartingIndex] = e.NewItems[e.NewStartingIndex];
				_viewPager.Adapter?.NotifyDataSetChanged();
			}

			// No other properties are valid.
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				UpdateItemsSource();
			}

			if (_pageIndicator != null)
				_pageIndicator.UpdateIndicatorCount();
		}

		// KeyboardService code
		void KeyboardService_VisibilityChanged(object sender, SoftKeyboardEventArgs e)
		{
			// The OnGlobalLayout method is calledd multiple times, so we have to store the previous state
			// and only do anything if the keyboard visibility is changed
			if (_isKeyboardVisible != e.IsVisible)
			{
				_isKeyboardVisible = e.IsVisible;
			}
		}

		void UpdatePositionFromRenderer(int position)
		{
			Element.NotifyPositionChanged(position);
		}

		void UpdatePosition()
		{
			if (!_isChangingPosition)
			{
				UpdateCurrentPage(Element.Position);
			}
		}

		void UpdateIndicatorPosition()
		{

		}

		void UpdateItemsSource()
		{
			_viewPager.Adapter = new CarouselViewPagerAdapter(Element, _context);
			var elementCount = GetItemCount();
			if (elementCount > 0)
			{
				for (int j = 0; j < elementCount; j++)
				{
					var item = GetItem(j);
					InsertPage(item, j);
				}
			}
			UpdatePosition();
			UpdateArrowsVisibility();
		}

		View GetItem(int index) => TemplatedItemsView.TemplatedItems[index];

		void UpdateOrientation()
		{

		}

		void ViewPager_PageScrolled(object sender, ViewPager.PageScrolledEventArgs e)
		{
			double percentCompleted;

			if (_setCurrentPageCalled)
			{
				percentCompleted = _pageScrolledCount * 100;
				_pageScrolledCount++;
			}
			else
			{
				// e.PositionOffset is the %
				// if e.Position < currentPosition, it is scrolling to the left
				if (e.Position < Element.Position)
				{
					percentCompleted = Math.Floor((1 - e.PositionOffset) * 100);
					_direction = Element.Orientation == CarouselOrientation.Horizontal ? ScrollDirection.Left : ScrollDirection.Up;
				}
				else
				{
					percentCompleted = Math.Floor(e.PositionOffset * 100);
					_direction = Element.Orientation == CarouselOrientation.Horizontal ? ScrollDirection.Right : ScrollDirection.Down;
				}
			}

			// report % while the user is dragging or when UpdateCurrentPage has been called
			if (_mViewPagerState == ViewPager.ScrollStateDragging || _setCurrentPageCalled)
				Element.SendScrolled(percentCompleted, _direction);

			// PageScrolled is called 2 times when UpdateCurrentPage is executed
			if (_pageScrolledCount == 2)
			{
				_setCurrentPageCalled = false;
				_pageScrolledCount = 0;
			}
		}

		// To assign position when page selected
		void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
		{
			// To avoid calling UpdateCurrentPage
			_isChangingPosition = true;
			Element.NotifyPositionChanged(e.Position);
			_isChangingPosition = false;
		}

		// To invoke PositionSelected
		void ViewPager_PageScrollStateChanged(object sender, ViewPager.PageScrollStateChangedEventArgs e)
		{
			// ScrollStateIdle = 0 : the pager is in Idle, Updatetled state
			// ScrollStateDragging = 1 : the pager is currently being dragged by the user
			// ScrollStateUpdatetling = 2 : the pager is in the process of Updatetling to a final position

			_mViewPagerState = e.State;

			// Call PositionSelected when scroll finish, after swiping finished and position > 0
			if (e.State == ViewPager.ScrollStateIdle)
				UpdateArrowsVisibility();
		}

		void UpdateIsSwipeEnabled()
		{
			((IViewPager)_viewPager)?.SetPagingEnabled(Element.IsSwipeEnabled);
		}

		int GetItemCount()
		{
			var count = TemplatedItemsView.TemplatedItems.Count();
			return count;
		}
		void UpdateArrowsVisibility()
		{
			if (_prevBtn == null || _nextBtn == null) return;
			var count = GetItemCount();

			_prevBtn.Visibility = !Element.ShowArrows || Element.Position == 0 || count == 0 ? AViews.ViewStates.Gone : AViews.ViewStates.Visible;
			_nextBtn.Visibility = !Element.ShowArrows || Element.Position == count - 1 || count == 0 ? AViews.ViewStates.Gone : AViews.ViewStates.Visible;
		}

		void PrevBtn_Click(object sender, EventArgs e)
		{
			if (Element.Position > 0)
			{
				UpdatePositionFromRenderer(Element.Position - 1);
				_direction = Element.Orientation == CarouselOrientation.Horizontal ? ScrollDirection.Left : ScrollDirection.Up;
			}
		}

		void NextBtn_Click(object sender, EventArgs e)
		{
			if (Element.Position < GetItemCount() - 1)
			{
				UpdatePositionFromRenderer(Element.Position + 1);
				_direction = Element.Orientation == CarouselOrientation.Horizontal ? ScrollDirection.Right : ScrollDirection.Down;
			}
		}

		void UpdateArrowsTintColor()
		{
			if (_prevBtn == null || _nextBtn == null) return;
			ImageView prev = (ImageView)_prevBtn.GetChildAt(0);
			ImageView next = (ImageView)_nextBtn.GetChildAt(0);

			AImageViewCompat.SetImageTintList(prev, AColorStateList.ValueOf(Element.ArrowsTintColor.ToAndroid()));
			AImageViewCompat.SetImageTintList(next, AColorStateList.ValueOf(Element.ArrowsTintColor.ToAndroid()));
		}

		void UpdateArrowsTransparency()
		{
			if (_prevBtn == null || _nextBtn == null) return;

			_prevBtn.Alpha = Element.ArrowsTransparency;
			_nextBtn.Alpha = Element.ArrowsTransparency;
		}

		void UpdateArrowsBackgroundColor()
		{
			if (_prevBtn == null || _nextBtn == null) return;

			_prevBtn.SetBackgroundColor(Element.ArrowsBackgroundColor.ToAndroid());
			_nextBtn.SetBackgroundColor(Element.ArrowsBackgroundColor.ToAndroid());
		}

		void UpdateIndicators()
		{
			if (Element.ShowIndicators)
			{
				_pageIndicator.Visibility = AViews.ViewStates.Visible;
				UpdateIndicatorsTintColor();
				UpdateIndicatorsCurrentPageTintColor();
				UpdateIndicatorsShape();
			}
			else
			{
				_pageIndicator.Visibility = AViews.ViewStates.Gone;
			}

			UpdateIndicatorPosition();
			UpdateIndicatorVisibility();
		}

		void UpdateIndicatorVisibility()
		{
			if (_indicators == null || Element == null)
				return;

			_indicators.Visibility = Element.ShowIndicators ? AViews.ViewStates.Visible : AViews.ViewStates.Gone;
		}

		void UpdateIndicatorsShape()
		{
			if (_pageIndicator == null) return;
			_pageIndicator.UpdateShapeType(Element.IndicatorsShape == IndicatorsShape.Circle ? AShapeType.Oval : AShapeType.Rectangle);
		}

		void UpdateIndicatorsCurrentPageTintColor()
		{
			if (_pageIndicator == null) return;
			_pageIndicator.UpdateCurrentPageIndicatorTintColor(Element.CurrentPageIndicatorTintColor.ToAndroid());
			UpdateIndicatorsShape();
		}

		void UpdateIndicatorsTintColor()
		{
			if (_pageIndicator == null) return;
			_pageIndicator.UpdatePageIndicatorTintColor(Element.IndicatorsTintColor.ToAndroid());
			UpdateIndicatorsShape();
		}

		void UpdateInterSpacing()
		{
			if (_viewPager == null || Element == null)
				return;

			var metrics = Resources.DisplayMetrics;
			var interPageSpacing = Element.InterPageSpacing * metrics.Density;
			_viewPager.PageMargin = (int)interPageSpacing;
		}

		void InsertPage(object item, int position)
		{
			// Fix for #168 Android NullReferenceException
			var Source = ((CarouselViewPagerAdapter)_viewPager?.Adapter)?.Source;

			if (Element == null || _viewPager == null || _viewPager?.Adapter == null || Source == null) return;

			Source.Insert(position, item);

			_viewPager.Adapter.NotifyDataSetChanged();

			UpdateArrowsVisibility();
		}

		// Android ViewPager is the most complicated piece of code ever :)
		async Task RemovePage(int position)
		{
			// Fix for #168 Android NullReferenceException
			var Source = ((CarouselViewPagerAdapter)_viewPager?.Adapter)?.Source;

			if (Element == null || _viewPager == null || _viewPager?.Adapter == null || Source == null) return;

			if (Source?.Count > 0)
			{
				_isChangingPosition = true;

				// To remove current page
				if (position == Element.Position && Source.Count > 1)
				{
					var newPos = position - 1;
					if (newPos == -1)
						newPos = 0;

					if (position == 0 && _viewPager.Adapter.Count > 2)
						// Move to next page
						_viewPager.SetCurrentItem(1, Element.AnimateTransition);
					else
						// Move to previous page
						_viewPager.SetCurrentItem(newPos, Element.AnimateTransition);

					// With a swipe transition
					if (Element.AnimateTransition)
						await Task.Delay(100);

					UpdatePositionFromRenderer(newPos);
				}

				Source.RemoveAt(position);
				_viewPager.Adapter.NotifyDataSetChanged();
				UpdateArrowsVisibility();
				_isChangingPosition = false;
			}
		}

		void UpdateCurrentPage(int position)
		{
			var itemCount = GetItemCount();
			if (position < 0 || position > itemCount - 1)
				return;

			_setCurrentPageCalled = true;

			if (Element == null || _viewPager == null || Element.ItemsSource == null) return;

			if (itemCount > 0)
			{
				_viewPager.SetCurrentItem(position, Element.AnimateTransition);

				UpdateArrowsVisibility();
			}
		}

		class CarouselViewPagerAdapter : PagerAdapter
		{
			CarouselView Element;
			Context _context;

			// A local copy of ItemsSource so we can use CollectionChanged events
			public List<object> Source;

			public CarouselViewPagerAdapter(CarouselView element, Context context)
			{
				Element = element;
				_context = context;
				Source = Element.ItemsSource != null ? new List<object>(Element.TemplatedItems.Count) : null;
			}

			public override int Count
			{
				get
				{
					return Source?.Count ?? 0;
				}
			}

			public override bool IsViewFromObject(AViews.View view, Java.Lang.Object @object)
			{
				return view == @object;
			}

			public override Java.Lang.Object InstantiateItem(AViews.ViewGroup container, int position)
			{
				View formsView = null;

				object bindingContext = null;

				if (Source != null && Source?.Count > 0)
					bindingContext = Source.Cast<object>().ElementAt(position);

				var dt = bindingContext as DataTemplate;
				var view = bindingContext as View;

				// Support for List<DataTemplate> as ItemsSource
				if (dt != null)
				{
					formsView = (View)dt.CreateContent();
				}
				else
				{
					if (view != null)
					{
						formsView = view;
					}
					else
					{
						var selector = Element.ItemTemplate as DataTemplateSelector;
						if (selector != null)
							formsView = (View)selector.SelectTemplate(bindingContext, Element).CreateContent();
						else
							formsView = (View)Element.ItemTemplate.CreateContent();

						formsView.BindingContext = bindingContext;
					}
				}

				var nativeConverted = formsView.ToAndroid(new Rectangle(0, 0, Element.Width, Element.Height), _context);
				nativeConverted.Tag = new Tag() { BindingContext = bindingContext }; //position;

				//nativeConverted.SaveEnabled = true;
				//nativeConverted.RestoreHierarchyState(mViewStates);

				var pager = (ViewPager)container;
				pager.AddView(nativeConverted);

				return nativeConverted;
			}

			public override void DestroyItem(AViews.ViewGroup container, int position, Java.Lang.Object @object)
			{
				var pager = (ViewPager)container;
				var view = (AViews.View)@object;
				//view.SaveEnabled = true;
				//view.SaveHierarchyState(mViewStates);
				pager.RemoveView(view);
				//[Android] Out of memories(FFImageLoading + CarouselView) #279
				view.Dispose();
			}

			public override int GetItemPosition(Java.Lang.Object @object)
			{
				var tag = (Tag)((AViews.View)@object).Tag;
				var position = Source.IndexOf(tag.BindingContext);
				return position != -1 ? position : PositionNone;
			}

		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				if (_prevBtn != null)
				{
					_prevBtn.Click -= PrevBtn_Click;
					_prevBtn.Dispose();
					_prevBtn = null;
				}

				if (_nextBtn != null)
				{
					_nextBtn.Click -= NextBtn_Click;
					_nextBtn.Dispose();
					_nextBtn = null;
				}

				if (_indicators != null)
				{
					_indicators.Dispose();
					_indicators = null;
				}

				if (_viewPager != null)
				{
					_viewPager.PageSelected -= ViewPager_PageSelected;
					_viewPager.PageScrollStateChanged -= ViewPager_PageScrollStateChanged;

					if (_viewPager.Adapter != null)
						_viewPager.Adapter.Dispose();

					_viewPager.Dispose();
					_viewPager = null;
				}

				if (_keyboardService != null)
				{
					_keyboardService.Dispose();
					_keyboardService = null;
				}

				if (_pageIndicator != null)
				{
					_pageIndicator.Dispose();
					_pageIndicator = null;
				}

				_disposed = true;
			}

			try
			{
				base.Dispose(disposing);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return;
			}
		}
	}
}