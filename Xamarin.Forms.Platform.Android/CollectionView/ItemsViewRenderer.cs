using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android.FastRenderers;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class ItemsViewRenderer : RecyclerView, IVisualElementRenderer, IEffectControlProvider
	{
		readonly AutomationPropertiesProvider _automationPropertiesProvider;
		readonly EffectControlProvider _effectControlProvider;

		protected ItemsViewAdapter ItemsViewAdapter;

		int? _defaultLabelFor;
		bool _disposed;
		protected ItemsView ItemsView;
		IItemsLayout _layout;
		SnapManager _snapManager;

		public ItemsViewRenderer(Context context) : base(context)
		{
			_automationPropertiesProvider = new AutomationPropertiesProvider(this);
			_effectControlProvider = new EffectControlProvider(this);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);
			ClipBounds = new Rect(0,0, Width, Height);

			_scrollAdjustment?.Invoke();
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			_effectControlProvider.RegisterEffect(effect);
		}

		public VisualElement Element => ItemsView;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), new Size());
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (!(element is ItemsView))
			{
				throw new ArgumentException($"{nameof(element)} must be of type {nameof(ItemsView)}");
			}

			Performance.Start(out string perfRef);

			VisualElement oldElement = ItemsView;
			ItemsView = (ItemsView)element;

			OnElementChanged(oldElement as ItemsView, ItemsView);

			// TODO hartez 2018/06/06 20:57:12 Find out what this does, and whether we really need it	
			element.SendViewInitialized(this);

			Performance.Stop(perfRef);
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			// TODO hartez 2018/06/06 20:58:54 Rethink whether we need to have _defaultLabelFor as a class member	
			if (_defaultLabelFor == null)
			{
				_defaultLabelFor = LabelFor;
			}

			LabelFor = (int)(id ?? _defaultLabelFor);
		}

		public VisualElementTracker Tracker { get; private set; }

		void IVisualElementRenderer.UpdateLayout()
		{
			Tracker?.UpdateLayout();
		}

		public global::Android.Views.View View => this;

		public ViewGroup ViewGroup => null;

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				_automationPropertiesProvider?.Dispose();
				Tracker?.Dispose();

				if (Element != null)
				{
					TearDownOldElement(Element as ItemsView);

					if (Platform.GetRenderer(Element) == this)
					{
						Element.ClearValue(Platform.RendererProperty);
					}
				}
			}

			base.Dispose(disposing);
		}

		protected virtual LayoutManager SelectLayoutManager(IItemsLayout layoutSpecification)
		{
			switch (layoutSpecification)
			{
				case GridItemsLayout gridItemsLayout:
					return CreateGridLayout(gridItemsLayout);
				case ListItemsLayout listItemsLayout:
					var orientation = listItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal
						? LinearLayoutManager.Horizontal
						: LinearLayoutManager.Vertical;

					return new LinearLayoutManager(Context, orientation, false);
			}

			// Fall back to plain old vertical list
			// TODO hartez 2018/08/30 19:34:36 Log a warning when we have to fall back because of an unknown layout	
			return new LinearLayoutManager(Context);
		}

		GridLayoutManager CreateGridLayout(GridItemsLayout gridItemsLayout)
		{
			return new GridLayoutManager(Context, gridItemsLayout.Span,
				gridItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal
					? LinearLayoutManager.Horizontal
					: LinearLayoutManager.Vertical,
				false);
		}

		void OnElementChanged(ItemsView oldElement, ItemsView newElement)
		{
			TearDownOldElement(oldElement);
			SetUpNewElement(newElement);

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, newElement));

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, newElement);
			
			UpdateBackgroundColor();
			UpdateFlowDirection();
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			ElementPropertyChanged?.Invoke(this, changedProperty);

			if (changedProperty.Is(ItemsView.ItemsSourceProperty))
			{
				UpdateItemsSource();
			}
			else if (changedProperty.Is(VisualElement.BackgroundColorProperty))
			{
				UpdateBackgroundColor();
			}
			else if (changedProperty.Is(VisualElement.FlowDirectionProperty))
			{
				UpdateFlowDirection();
			}
		}

		protected virtual void UpdateItemsSource()
		{
			if (ItemsView == null)
			{
				return;
			}

			ItemsViewAdapter = new ItemsViewAdapter(ItemsView, Context);
			SwapAdapter(ItemsViewAdapter, false);
		}

		void SetUpNewElement(ItemsView newElement)
		{
			if (newElement == null)
			{
				return;
			}

			newElement.PropertyChanged += OnElementPropertyChanged;

			// TODO hartez 2018/06/06 20:49:14 Review whether we can just do this in the constructor	
			if (Tracker == null)
			{
				Tracker = new VisualElementTracker(this);
			}

			this.EnsureId();

			UpdateItemsSource();

			SetLayoutManager(SelectLayoutManager(newElement.ItemsLayout));
			UpdateSnapBehavior();

			// Keep track of the ItemsLayout's property changes
			_layout = newElement.ItemsLayout;
			_layout.PropertyChanged += LayoutOnPropertyChanged;

			// TODO hartez 2018/09/17 13:16:12 This propertychanged handler needs to be torn down in Dispose and TearDownElement	

			// Listen for ScrollTo requests
			newElement.ScrollToRequested += ScrollToRequested;
		}
		
		void TearDownOldElement(ItemsView oldElement)
		{
			if (oldElement == null)
			{
				return;
			}

			// Stop listening for property changes
			oldElement.PropertyChanged -= OnElementPropertyChanged;

			// Stop listening for ScrollTo requests
			oldElement.ScrollToRequested -= ScrollToRequested;

			var adapter = GetAdapter();

			if (adapter != null)
			{
				adapter.Dispose();
				SetAdapter(null);
			}

			if (_snapManager != null)
			{
				_snapManager.Dispose();
				_snapManager = null;
			}
		}

		void LayoutOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChanged)
		{
			if(propertyChanged.Is(GridItemsLayout.SpanProperty))
			{
				if (GetLayoutManager() is GridLayoutManager gridLayoutManager)
				{
					gridLayoutManager.SpanCount = ((GridItemsLayout)_layout).Span;
				}
			} 
			else if (propertyChanged.IsOneOf(ItemsLayout.SnapPointsTypeProperty, ItemsLayout.SnapPointsAlignmentProperty))
			{
				UpdateSnapBehavior();
			}
		}

		protected virtual void UpdateSnapBehavior()
		{
			if (_snapManager == null)
			{
				_snapManager = new SnapManager(ItemsView, this);
			}

			_snapManager.UpdateSnapBehavior();
		}

		// TODO hartez 2018/08/09 09:30:17 Package up background color and flow direction providers so we don't have to re-implement them here	
		protected virtual void UpdateBackgroundColor(Color? color = null)
		{
			if (Element == null)
			{
				return;
			}

			SetBackgroundColor((color ?? Element.BackgroundColor).ToAndroid());
		}

		protected virtual void UpdateFlowDirection()
		{
			if (Element == null)
			{
				return;
			}

			this.UpdateFlowDirection(Element);

			ReconcileFlowDirectionAndLayout();
		}

		protected virtual void ReconcileFlowDirectionAndLayout()
		{
			if (!(GetLayoutManager() is LinearLayoutManager linearLayoutManager))
			{
				return;
			}

			if (linearLayoutManager.CanScrollVertically())
			{
				return;
			}

			var effectiveFlowDirection = ((IVisualElementController)Element).EffectiveFlowDirection;

			if (effectiveFlowDirection.IsRightToLeft() && !linearLayoutManager.ReverseLayout)
			{
				linearLayoutManager.ReverseLayout = true;
				return;
			}

			if (effectiveFlowDirection.IsLeftToRight() && linearLayoutManager.ReverseLayout)
			{
				linearLayoutManager.ReverseLayout = false;
			}
		}

		protected virtual int DetermineIndex(ScrollToRequestEventArgs args)
		{
			if (args.Mode == ScrollToMode.Position)
			{
				// TODO hartez 2018/08/28 15:40:03 Need to handle group indices here as well	
				return args.Index;
			}

			return ItemsViewAdapter.GetIndexForItem(args.Item);
		}

		void ScrollToRequested(object sender, ScrollToRequestEventArgs args)
		{
			ScrollToPosition(args);
		}

		protected virtual void ScrollToPosition(ScrollToRequestEventArgs args)
		{
			var index = DetermineIndex(args);
			
			if (args.Animate)
			{
				if (args.ScrollToPosition == Xamarin.Forms.ScrollToPosition.MakeVisible)
				{
					// MakeVisible matches the Android default of SnapAny, so we can just use the default
					SmoothScrollToPosition(index);
				}
				else
				{
					// If we want a different ScrollToPosition, we need to create a SmoothScroller which can handle it
					var smoothScroller = new PositionalSmoothScroller(Context, args.ScrollToPosition)
					{
						TargetPosition = index
					};

					// And kick off the scroll operation
					GetLayoutManager().StartSmoothScroll(smoothScroller);
				}
			}
			else
			{
				if (!(GetLayoutManager() is LinearLayoutManager linearLayoutManager))
				{
					// We don't have the ScrollToPositionWithOffset method available, so we don't have a way to 
					// handle ScrollToPosition; just default back to the MakeVisible behavior
					ScrollToPosition(index);
					return;
				}

				if (args.ScrollToPosition == Xamarin.Forms.ScrollToPosition.MakeVisible)
				{
					// MakeVisible is the default behavior, so we don't need to do anything special
					ScrollToPosition(index);
					return;
				}

				// If ScrollToPosition is Start, then we can just use an offset of 0 and we're fine
				// (Though that may change in RTL situations or if we're stacking from the end)
				if (args.ScrollToPosition == Xamarin.Forms.ScrollToPosition.Start)
				{
					linearLayoutManager.ScrollToPositionWithOffset(index, 0);
					return;
				}

				// For handling End or Center, things get more complicated because we need to know the size of
				// the View we're targeting. 

				// If we're using ItemSizingStrategy MeasureFirstItem, we can use any item size as a guide to figure
				// out the offset

				// TODO hartez 2018/10/03 14:00:32 Handle the MeasureFirstItem case to determine the offset and call	
				// `linearLayoutManager.ScrollToPositionWithOffset(index, offset);` here


				// If we don't already know the size of the item, things get more complicated

				// The item may not actually exist; it may have never been realized, or it may have been recycled
				// So we need to get it on screen using ScrollToPosition, then once it's on screen we can use the 
				// width/height to make adjustments for Center/End.

				// ScrollToPosition queues up the scroll operation, it doesn't do it immediately; it requests a layout
				
				_scrollAdjustment = () =>
				{
					var holder = FindViewHolderForAdapterPosition(index);
					var view = holder?.ItemView;

					if (view == null)
					{
						return;
					}

					var offset = 0;

					var rvRect = new Rect();
					GetGlobalVisibleRect(rvRect);

					var viewRect = new Rect();
					view.GetGlobalVisibleRect(viewRect);

					if (args.ScrollToPosition == Xamarin.Forms.ScrollToPosition.Center)
					{
						if (linearLayoutManager.CanScrollHorizontally())
						{
							offset = viewRect.CenterX() - rvRect.CenterX();
						}
						else
						{
							offset = viewRect.CenterY() - rvRect.CenterY();
						}
					}
					else if (args.ScrollToPosition == Xamarin.Forms.ScrollToPosition.End)
					{
						if (linearLayoutManager.CanScrollHorizontally())
						{
							offset = viewRect.Right - rvRect.Right;
						}
						else
						{
							offset = viewRect.Bottom - rvRect.Bottom;
						}
					}

					_scrollAdjustment = null;

					if (linearLayoutManager.CanScrollHorizontally())
					{
						ScrollBy(offset, 0);
					}
					else
					{
						ScrollBy(0, offset);
					}
				};

				ScrollToPosition(index);
			}

			
		}

		Action _scrollAdjustment;

	}

	
}