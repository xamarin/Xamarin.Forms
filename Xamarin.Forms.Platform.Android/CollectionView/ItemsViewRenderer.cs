using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android.FastRenderers;

namespace Xamarin.Forms.Platform.Android
{
	public class ItemsViewRenderer : RecyclerView, IVisualElementRenderer, IEffectControlProvider
	{
		readonly AutomationPropertiesProvider _automationPropertiesProvider;
		readonly EffectControlProvider _effectControlProvider;

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

			// TODO hartez 2018/08/29 17:33:59 Should this use SwapAdapter if one exists? Probably	

			SetAdapter(new ItemsViewAdapter(ItemsView, Context));
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

		// TODO hartez 2018/08/09 09:30:17 Package up backround color and flow direction providers so we don't have to reimplement them here	
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

		void ScrollToRequested(object sender, ScrollToRequestEventArgs args)
		{
			if (args.Mode == ScrollToMode.Position)
			{
				ScrollToPosition(args);
				return;
			}

			// TODO hartez 2018/08/28 15:40:26 handle scrolling to to item	
		}

		void ScrollToPosition(ScrollToRequestEventArgs args)
		{
			// TODO hartez 2018/08/28 15:40:03 Need to handle group indices here as well	

			// TODO hartez 2018/08/28 16:38:49 Handle args.ScrollToPosition; right now we're just doing MakeVisible by default	

			if (args.Animate)
			{
				SmoothScrollToPosition(args.Index);
			}
			else
			{
				ScrollToPosition(args.Index);	
			}
		}
	}
}