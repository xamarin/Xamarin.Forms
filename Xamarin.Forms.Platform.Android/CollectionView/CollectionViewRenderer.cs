using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android.FastRenderers;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class CollectionViewRenderer : RecyclerView, IVisualElementRenderer, IEffectControlProvider
	{
		readonly AutomationPropertiesProvider _automationPropertiesProvider;
		readonly EffectControlProvider _effectControlProvider;

		Adapter _adapter;
		int? _defaultLabelFor;
		bool _isDisposed;
		ItemsView _itemsView;
		IItemsLayout _layout;

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
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;

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
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			ElementPropertyChanged?.Invoke(this, e);
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

			// TODO hartez 2018/05/29 20:28:14 Review whether we really need to keep a ref to adapter	
			_adapter = new CollectionViewAdapter(newElement, Context);
			SetAdapter(_adapter);
			SetLayoutManager(SelectLayoutManager(newElement.ItemsLayout));

			// Keep track of the ItemsLayout's property changes
			_layout = newElement.ItemsLayout;
			_layout.PropertyChanged += LayoutOnPropertyChanged;
		}

		void LayoutOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == GridItemsLayout.SpanProperty.PropertyName)
			{
				if (GetLayoutManager() is GridLayoutManager gridLayoutManager)
				{
					gridLayoutManager.SpanCount = ((GridItemsLayout)_layout).Span;
				}
			}
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
		}
	}
}