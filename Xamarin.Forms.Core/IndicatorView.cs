using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Flex;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	public enum IndicatorShape
	{
		Circle,
		Square
	}

	[ContentProperty(nameof(IndicatorLayout))]
	[RenderWith(typeof(_IndicatorViewRenderer))]
	public class IndicatorView : TemplatedView
	{
		const int DefaultPadding = 4;

		public static readonly BindableProperty IndicatorsShapeProperty = BindableProperty.Create(nameof(IndicatorsShape), typeof(IndicatorShape), typeof(IndicatorView), IndicatorShape.Circle);

		public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(int), typeof(IndicatorView), default(int), BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty CountProperty = BindableProperty.Create(nameof(Count), typeof(int), typeof(IndicatorView), default(int), propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorCount((int)oldValue));

		public static readonly BindableProperty MaximumVisibleProperty = BindableProperty.Create(nameof(MaximumVisible), typeof(int), typeof(IndicatorView), int.MaxValue, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty IndicatorTemplateProperty = BindableProperty.Create(nameof(IndicatorTemplate), typeof(DataTemplate), typeof(IndicatorView), default(DataTemplate), propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty HideSingleProperty = BindableProperty.Create(nameof(HideSingle), typeof(bool), typeof(IndicatorView), true, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty IndicatorColorProperty = BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(IndicatorView), Color.Default, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty SelectedIndicatorColorProperty = BindableProperty.Create(nameof(SelectedIndicatorColor), typeof(Color), typeof(IndicatorView), Color.Default, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty IndicatorSizeProperty = BindableProperty.Create(nameof(IndicatorSize), typeof(double), typeof(IndicatorView), 6.0, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(IndicatorView), null, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetItemsSource((IEnumerable)oldValue));

		static readonly BindableProperty IndicatorLayoutProperty = BindableProperty.Create(nameof(IndicatorLayout), typeof(Layout<View>), typeof(IndicatorView), null, propertyChanged: TemplateUtilities.OnContentChanged);

		public IndicatorView()
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(IndicatorView), ExperimentalFlags.IndicatorViewExperimental);
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (propertyName == VisualProperty.PropertyName && IsFormsVisual())
			{
				IndicatorLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
			}
			base.OnPropertyChanged(propertyName);
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			var baseRequest = base.OnMeasure(widthConstraint, heightConstraint);

			if (IsFormsVisual())
				return baseRequest;

			var defaultSize = IndicatorSize + DefaultPadding + DefaultPadding;
			var items = Count;
			var sizeRequest = new SizeRequest(new Size(items * defaultSize, IndicatorSize), new Size(10, 10));
			return sizeRequest;
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

		bool IsFormsVisual() => (Visual is VisualMarker.FormsVisual);

		IList<View> Items
		{
			get
			{
				if (!IsFormsVisual())
					return null;

				if (IndicatorLayout == null)
					IndicatorLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

				return IndicatorLayout.Children;
			}
		}

		void ResetIndicatorStyles()
		{
			if (IndicatorLayout == null)
				return;

			try
			{
				BatchBegin();
				ResetIndicatorStylesNonBatch();
			}
			finally
			{
				BatchCommit();
			}
		}

		void ResetIndicatorCount(int oldCount)
		{
			if (IndicatorLayout == null)
				return;

			try
			{
				BatchBegin();
				if (oldCount < 0)
				{
					oldCount = 0;
				}

				if (oldCount > Count)
				{
					RemoveRedundantIndicatorItems();
					return;
				}

				AddExtraIndicatorItems();
			}
			finally
			{
				ResetIndicatorStylesNonBatch();
				BatchCommit();
			}
		}

		void ResetIndicatorStylesNonBatch()
		{
			for (int index = 0; index < Items.Count; index++)
			{
				Items[index].BackgroundColor = index == Position
					? GetColorOrDefault(SelectedIndicatorColor, Color.Gray)
					: GetColorOrDefault(IndicatorColor, Color.Silver);
			}

			IndicatorLayout.IsVisible = Count > 1 || !HideSingle;
		}

		Color GetColorOrDefault(Color color, Color defaultColor)
			=> color.IsDefault ? defaultColor : color;

		void ResetItemsSource(IEnumerable oldItemsSource)
		{
			if (oldItemsSource is INotifyCollectionChanged oldCollection)
				oldCollection.CollectionChanged -= OnCollectionChanged;

			if (ItemsSource is INotifyCollectionChanged collection)
				collection.CollectionChanged += OnCollectionChanged;

			OnCollectionChanged(ItemsSource, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		void AddExtraIndicatorItems()
		{
			var oldCount = Items.Count;
			for (var i = 0; i < Count - oldCount && i < MaximumVisible - oldCount; i++)
			{
				var size = IndicatorSize > 0 ? IndicatorSize : 10;
				var indicator = IndicatorTemplate?.CreateContent() as View ?? new Frame
				{
					Padding = 0,
					HasShadow = false,
					BorderColor = Color.Transparent,
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center,
					WidthRequest = size,
					HeightRequest = size,
					CornerRadius = (float)size / 2
				};
				var tapGestureRecognizer = new TapGestureRecognizer();
				tapGestureRecognizer.Tapped += (sender, args) => Position = Items.IndexOf(sender as View);
				indicator.GestureRecognizers.Add(tapGestureRecognizer);
				Items.Add(indicator);
			}
		}

		void RemoveRedundantIndicatorItems()
		{
			while (Items.Count > Count)
			{
				Items.RemoveAt(0);
			}
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (sender is ICollection collection)
			{
				Count = collection.Count;
				return;
			}
			var count = 0;
			var enumerator = (sender as IEnumerable)?.GetEnumerator();
			while (enumerator?.MoveNext() ?? false)
			{
				count++;
			}
			Count = count;
		}
	}
}