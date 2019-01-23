using System;
using System.ComponentModel;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android.FastRenderers;
using Android.Graphics.Drawables;
using AView = Android.Views.View;
using AColor = Android.Graphics.Color;
using AShapes = Android.Graphics.Drawables.Shapes;
using Xamarin.Forms.Internals;
using AShapeType = Android.Graphics.Drawables.ShapeType;

namespace Xamarin.Forms.Platform.Android
{
	public class IndicatorsViewRenderer : LinearLayout, IVisualElementRenderer, IEffectControlProvider
	{
		readonly AutomationPropertiesProvider _automationPropertiesProvider;
		readonly EffectControlProvider _effectControlProvider;

		IItemsViewSource ItemsSource;

		protected IndicatorsView IndicatorsView;
		protected SelectableItemsView SelectableItemsView => IndicatorsView.GetItemsSourceBy(IndicatorsView);

		int? _defaultLabelFor;
		bool _disposed;
		int _selectedIndex = 0;
		AColor _currentPageIndicatorTintColor;
		ShapeType _shapeType = ShapeType.Oval;
		Drawable _currentPageShape = null;
		Drawable _pageShape = null;
		AColor _pageIndicatorTintColor;
		bool IsVisible => Visibility != ViewStates.Gone;

		public VisualElement Element => IndicatorsView;

		public VisualElementTracker Tracker { get; private set; }

		public ViewGroup ViewGroup => null;

		public AView View => this;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;


		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;


		public IndicatorsViewRenderer(Context context) : base(context)
		{
			CollectionView.VerifyCollectionViewFlagEnabled(nameof(IndicatorsViewRenderer));

			_automationPropertiesProvider = new AutomationPropertiesProvider(this);
			_effectControlProvider = new EffectControlProvider(this);
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), new Size());
		}

		void IVisualElementRenderer.UpdateLayout()
		{
			Tracker?.UpdateLayout();
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (!(element is IndicatorsView))
			{
				throw new ArgumentException($"{nameof(element)} must be of type {typeof(IndicatorsView).Name}");
			}

			var oldElement = IndicatorsView;
			var newElement = (IndicatorsView)element;

			TearDownOldElement(oldElement);
			SetUpNewElement(newElement);

			OnElementChanged(oldElement, newElement);
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
			{
				_defaultLabelFor = LabelFor;
			}

			LabelFor = (int)(id ?? _defaultLabelFor);
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			_effectControlProvider.RegisterEffect(effect);
		}

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
					TearDownOldElement(Element as IndicatorsView);

					if (Platform.GetRenderer(Element) == this)
					{
						Element.ClearValue(Platform.RendererProperty);
					}
				}
			}
		}

		void OnElementChanged(IndicatorsView oldElement, IndicatorsView newElement)
		{
			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, newElement));
			EffectUtilities.RegisterEffectControlProvider(this, oldElement, newElement);
			OnElementChanged(new ElementChangedEventArgs<IndicatorsView>(oldElement, newElement));
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<IndicatorsView> elementChangedEvent)
		{
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			ElementPropertyChanged?.Invoke(this, changedProperty);

			if (changedProperty.Is(IndicatorsView.IndicatorsShapeProperty))
			{
				UpdateShapes();
			}
			else if (changedProperty.Is(IndicatorsView.IndicatorsTintColorProperty) ||
					 changedProperty.Is(IndicatorsView.SelectedIndicatorTintColorProperty))
			{
				ResetIndicators();
			}
			else if (changedProperty.Is(VisualElement.BackgroundColorProperty))
			{
				UpdateBackgroundColor();
			}
			else if (changedProperty.Is(IndicatorsView.ItemsSourceByProperty))
			{
				UpdateItemsSource();
			}
		}

		protected virtual void UpdateBackgroundColor(Color? color = null)
		{
			if (Element == null)
			{
				return;
			}

			SetBackgroundColor((color ?? Element.BackgroundColor).ToAndroid());
		}

		void SetUpNewElement(IndicatorsView newElement)
		{
			if (newElement == null)
			{
				IndicatorsView = null;
				return;
			}

			IndicatorsView = newElement;

			IndicatorsView.PropertyChanged += OnElementPropertyChanged;
			IndicatorsView.ItemSourcePropertyChanged += IndicatorsViewItemSourcePropertyChanged;

			if (Tracker == null)
			{
				Tracker = new VisualElementTracker(this);
			}

			this.EnsureId();

			UpdateBackgroundColor();
			UpdateItemsSource();
		}

		void IndicatorsViewItemSourcePropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			if (changedProperty.Is(ItemsView.ItemsSourceProperty))
			{
				UpdateItemsSource();
			}
			else if (changedProperty.Is(SelectableItemsView.SelectedItemProperty))
			{
				UpdateSelectedIndicator();
			} 
		}

		void UpdateSelectedIndicator()
		{
			_selectedIndex = GetPositionForItem(SelectableItemsView?.SelectedItem);
			UpdateIndicators();
		}

		void UpdateItemsSource()
		{
			ItemsSource = ItemsSourceFactory.Create(SelectableItemsView?.ItemsSource, null);
			ResetIndicators();
			UpdateIndicatorCount();
		}

		int GetPositionForItem(object item)
		{
			var itemsCount = ItemsSource.Count;
		
			for (int n = 0; n < itemsCount; n++)
			{
				if (ItemsSource[n] == item)
				{
					return n;
				}
			}

			return -1;
		}

		void TearDownOldElement(IndicatorsView oldElement)
		{
			if (oldElement == null)
			{
				return;
			}

			oldElement.PropertyChanged -= OnElementPropertyChanged;
			IndicatorsView.ItemSourcePropertyChanged -= IndicatorsViewItemSourcePropertyChanged;
		}

		void UpdateIndicatorCount()
		{
			if (!IsVisible)
				return;

			var count = ItemsSource.Count;
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

		void ResetIndicators()
		{
			if (!IsVisible)
				return;

			_pageIndicatorTintColor = IndicatorsView.IndicatorsTintColor.ToAndroid();

			_currentPageIndicatorTintColor = IndicatorsView.SelectedIndicatorTintColor.ToAndroid();
			_shapeType = IndicatorsView.IndicatorsShape == IndicatorsShape.Circle ? AShapeType.Oval : AShapeType.Rectangle;
			_pageShape = null;
			_currentPageShape = null;
			UpdateShapes();
			UpdateIndicators();
		}

		void UpdateIndicators()
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
			shape.Paint.Color = color;

			return shape;
		}
	}
}