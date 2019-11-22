using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[ContentProperty(nameof(IndicatorLayout))]
	[RenderWith(typeof(_IndicatorViewRenderer))]
	public class IndicatorView : TemplatedView
	{
		const int DefaultPadding = 4;

		public static readonly BindableProperty IndicatorsShapeProperty = BindableProperty.Create(nameof(IndicatorsShape), typeof(IndicatorShape), typeof(IndicatorView), IndicatorShape.Circle);

		public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(int), typeof(IndicatorView), default(int), BindingMode.TwoWay);

		public static readonly BindableProperty CountProperty = BindableProperty.Create(nameof(Count), typeof(int), typeof(IndicatorView), default(int), propertyChanged: (bindable, oldValue, newValue)
			=> (((IndicatorView)bindable).IndicatorLayout as IndicatorStackLayout)?.ResetIndicatorCount((int)oldValue));

		public static readonly BindableProperty MaximumVisibleProperty = BindableProperty.Create(nameof(MaximumVisible), typeof(int), typeof(IndicatorView), int.MaxValue);

		public static readonly BindableProperty IndicatorTemplateProperty = BindableProperty.Create(nameof(IndicatorTemplate), typeof(DataTemplate), typeof(IndicatorView), default(DataTemplate));

		public static readonly BindableProperty HideSingleProperty = BindableProperty.Create(nameof(HideSingle), typeof(bool), typeof(IndicatorView), true);

		public static readonly BindableProperty IndicatorColorProperty = BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(IndicatorView), Color.Default);

		public static readonly BindableProperty SelectedIndicatorColorProperty = BindableProperty.Create(nameof(SelectedIndicatorColor), typeof(Color), typeof(IndicatorView), Color.Default);

		public static readonly BindableProperty IndicatorSizeProperty = BindableProperty.Create(nameof(IndicatorSize), typeof(double), typeof(IndicatorView), 6.0);

		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(IndicatorView), null, propertyChanged: (bindable, oldValue, newValue)
			=> (((IndicatorView)bindable).IndicatorLayout as IndicatorStackLayout)?.ResetItemsSource((IEnumerable)oldValue));

		static readonly BindableProperty IndicatorLayoutProperty = BindableProperty.Create(nameof(IndicatorLayout), typeof(Layout<View>), typeof(IndicatorView), null, propertyChanged: TemplateUtilities.OnContentChanged);

		public IndicatorView()
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(IndicatorView), ExperimentalFlags.IndicatorViewExperimental);
			IndicatorLayout = new IndicatorStackLayout(this);
		}

		public IndicatorShape IndicatorsShape
		{
			get { return (IndicatorShape)GetValue(IndicatorsShapeProperty); }
			set { SetValue(IndicatorsShapeProperty, value); }
		}

		public Layout<View> IndicatorLayout
		{
			get => (Layout<View>)GetValue(IndicatorLayoutProperty);
			set => SetValue(IndicatorLayoutProperty, value);
		}

		public int Position
		{
			get => (int)GetValue(PositionProperty);
			set => SetValue(PositionProperty, value);
		}

		public int Count
		{
			get => (int)GetValue(CountProperty);
			set => SetValue(CountProperty, value);
		}

		public int MaximumVisible
		{
			get => (int)GetValue(MaximumVisibleProperty);
			set => SetValue(MaximumVisibleProperty, value);
		}

		public DataTemplate IndicatorTemplate
		{
			get => (DataTemplate)GetValue(IndicatorTemplateProperty);
			set => SetValue(IndicatorTemplateProperty, value);
		}

		public bool HideSingle
		{
			get => (bool)GetValue(HideSingleProperty);
			set => SetValue(HideSingleProperty, value);
		}

		public Color IndicatorColor
		{
			get => (Color)GetValue(IndicatorColorProperty);
			set => SetValue(IndicatorColorProperty, value);
		}

		public Color SelectedIndicatorColor
		{
			get => (Color)GetValue(SelectedIndicatorColorProperty);
			set => SetValue(SelectedIndicatorColorProperty, value);
		}

		public double IndicatorSize
		{
			get => (double)GetValue(IndicatorSizeProperty);
			set => SetValue(IndicatorSizeProperty, value);
		}

		public IEnumerable ItemsSource
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		public static readonly BindableProperty ItemsSourceByProperty = BindableProperty.Create("ItemsSourceBy", typeof(VisualElement), typeof(IndicatorView), default(VisualElement), propertyChanged: (bindable, oldValue, newValue)
		 => LinkToCarouselView(bindable as IndicatorView, newValue as CarouselView));

		[TypeConverter(typeof(ReferenceTypeConverter))]
		public static VisualElement GetItemsSourceBy(BindableObject bindable)
		{
			return (VisualElement)bindable.GetValue(ItemsSourceByProperty);
		}

		public static void SetItemsSourceBy(BindableObject bindable, VisualElement value)
		{
			bindable.SetValue(ItemsSourceByProperty, value);
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			var baseRequest = base.OnMeasure(widthConstraint, heightConstraint);

			if (IndicatorTemplate != null)
				return baseRequest;

			var defaultSize = IndicatorSize + DefaultPadding + DefaultPadding;
			var items = Count;
			var sizeRequest = new SizeRequest(new Size(items * defaultSize, IndicatorSize), new Size(10, 10));
			return sizeRequest;
		}

		static void LinkToCarouselView(IndicatorView indicatorView, CarouselView carouselView)
		{
			if (carouselView == null || indicatorView == null)
				return;

			indicatorView.SetBinding(PositionProperty, new Binding
			{
				Path = nameof(CarouselView.Position),
				Source = carouselView
			});

			indicatorView.SetBinding(ItemsSourceProperty, new Binding
			{
				Path = nameof(ItemsView.ItemsSource),
				Source = carouselView
			});
		}
	}
}