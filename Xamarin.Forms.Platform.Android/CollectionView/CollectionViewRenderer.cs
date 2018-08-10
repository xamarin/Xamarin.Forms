using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android.FastRenderers;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	// TODO hartez 2018/08/09 16:28:34 Current theory:	

	// Somehow, the container (layout?) renderer has clipChildren and clipToPadding both set to false
	// see your Recycler project for a repro of this problem in Xamarin.Android

	// clipChildren _might_ be getting set by VisualElementTracker; can't find anywhere that sets
	// clipToPadding (which is supposed to default to true)
	// https://developer.android.com/reference/android/view/ViewGroup#attr_android:clipChildren



	public class CollectionViewRenderer : RecyclerView, IVisualElementRenderer, IEffectControlProvider
	{
		readonly AutomationPropertiesProvider _automationPropertiesProvider;
		readonly EffectControlProvider _effectControlProvider;

		Adapter _adapter;
		int? _defaultLabelFor;
		bool _disposed;
		ItemsView _itemsView;
		IItemsLayout _layout;
		SnapManager _snapManager;

		public CollectionViewRenderer(Context context) : base(context)
		{
			_automationPropertiesProvider = new AutomationPropertiesProvider(this);
			_effectControlProvider = new EffectControlProvider(this);
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			_effectControlProvider.RegisterEffect(effect);
		}

		public VisualElement Element => _itemsView;

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
				throw new ArgumentException($"{nameof(element)} must be of type {nameof(_itemsView)}");
			}

			Performance.Start(out string perfRef);

			VisualElement oldElement = _itemsView;
			_itemsView = (ItemsView)element;

			OnElementChanged(oldElement as ItemsView, _itemsView);

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

		public AView View => this;

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

				if (_adapter != null)
				{
					_adapter.Dispose();
					_adapter = null;
				}

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;

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

		protected void UpdateItemsSource()
		{
			if (_itemsView == null)
			{
				return;
			}

			// TODO hartez 2018/05/29 20:28:14 Review whether we really need to keep a ref to adapter	
			_adapter = new CollectionViewAdapter(_itemsView, Context);
			SetAdapter(_adapter);
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
				_snapManager = new SnapManager(_itemsView, this);
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
		}

		void TearDownOldElement(ItemsView oldElement)
		{
			if (oldElement == null)
			{
				return;
			}

			oldElement.PropertyChanged -= OnElementPropertyChanged;

			if (_adapter != null)
			{
				_adapter.Dispose();
				_adapter = null;
			}

			if (_snapManager != null)
			{
				_snapManager.Dispose();
				_snapManager = null;
			}
		}

		public override void OnDraw(Canvas c)
		{
			System.Diagnostics.Debug.WriteLine($">>>>> CollectionViewRenderer OnDraw Elevation: {Elevation}");
			base.OnDraw(c);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			System.Diagnostics.Debug.WriteLine($">>>>> CollectionViewRenderer OnMeasure 297: MESSAGE");

			var width = MeasureSpec.GetSize(widthMeasureSpec);
			var height = MeasureSpec.GetSize(heightMeasureSpec);

			System.Diagnostics.Debug.WriteLine($">>>>> CollectionViewRenderer OnMeasure: width {width}, height {height}");

			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);
		}
	}
}