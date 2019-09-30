using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[ContentProperty(nameof(IndicatorLayout))]
	[RenderWith(typeof(_IndicatorViewRenderer))]
	public class IndicatorView : TemplatedView
	{
		public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(int), typeof(IndicatorView), default(int), BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty CountProperty = BindableProperty.Create(nameof(Count), typeof(int), typeof(IndicatorView), default(int), propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorCount((int)oldValue));

		public static readonly BindableProperty MaximumVisibleCountProperty = BindableProperty.Create(nameof(MaximumVisibleCount), typeof(int), typeof(IndicatorView), int.MaxValue, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty IndicatorTemplateProperty = BindableProperty.Create(nameof(IndicatorTemplate), typeof(DataTemplate), typeof(IndicatorView), default(DataTemplate), propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty HidesForSingleIndicatorProperty = BindableProperty.Create(nameof(HidesForSingleIndicator), typeof(bool), typeof(IndicatorView), true, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty IndicatorColorProperty = BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(IndicatorView), Color.Default, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty SelectedIndicatorColorProperty = BindableProperty.Create(nameof(SelectedIndicatorColor), typeof(Color), typeof(IndicatorView), Color.Default, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty IndicatorSizeProperty = BindableProperty.Create(nameof(IndicatorSize), typeof(double), typeof(IndicatorView), -1.0, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorStyles());

		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(IndicatorView), null, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetItemsSource((IEnumerable)oldValue));

		public static readonly BindableProperty IndicatorLayoutProperty = BindableProperty.Create(nameof(IndicatorLayout), typeof(Layout<View>), typeof(IndicatorView), null, propertyChanged: TemplateUtilities.OnContentChanged);

		public IndicatorView()
			=> IndicatorLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

		IList<View> Items => IndicatorLayout.Children;

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

		public int MaximumVisibleCount
		{
			get => (int)GetValue(MaximumVisibleCountProperty);
			set => SetValue(MaximumVisibleCountProperty, value);
		}

		public DataTemplate IndicatorTemplate
		{
			get => (DataTemplate)GetValue(IndicatorTemplateProperty);
			set => SetValue(IndicatorTemplateProperty, value);
		}

		public bool HidesForSingleIndicator
		{
			get => (bool)GetValue(HidesForSingleIndicatorProperty);
			set => SetValue(HidesForSingleIndicatorProperty, value);
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

		void ResetIndicatorStyles()
		{
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
			foreach (var child in Items)
			{
				ApplyColor(child as View);
			}

			IndicatorLayout.IsVisible = Count > 1 || !HidesForSingleIndicator;
		}

		void ResetItemsSource(IEnumerable oldItemsSource)
		{
			if (oldItemsSource is INotifyCollectionChanged oldObservableCollection)
			{
				oldObservableCollection.CollectionChanged -= OnCollectionChanged;
			}

			if (ItemsSource is INotifyCollectionChanged observableCollection)
			{
				observableCollection.CollectionChanged += OnCollectionChanged;
			}

			OnCollectionChanged(ItemsSource, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		void AddExtraIndicatorItems()
		{
			var oldCount = Items.Count;
			for (var i = 0; i < Count - oldCount && i < MaximumVisibleCount - oldCount; ++i)
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
				var itemTapGesture = new TapGestureRecognizer();
				itemTapGesture.Tapped += (tapSender, tapArgs) => Position = Items.IndexOf(tapSender as View);
				indicator.GestureRecognizers.Add(itemTapGesture);
				Items.Add(indicator);
			}
		}

		void RemoveRedundantIndicatorItems()
		{
			foreach (var item in Items.Where((v, i) => i >= Count).ToArray())
			{
				Items.Remove(item);
			}
		}

		void ApplyColor(View view)
		{
			var index = Items.IndexOf(view);
			if (index == Position)
			{
				ApplySelectedColor(view);
				return;
			}
			ApplyRegularColor(view);
		}

		void ApplySelectedColor(View indicatorItemView)
			=> indicatorItemView.BackgroundColor = GetColorOrDefault(SelectedIndicatorColor, Color.Gray);

		void ApplyRegularColor(View indicatorItemView)
			=> indicatorItemView.BackgroundColor = GetColorOrDefault(IndicatorColor, Color.Silver);

		Color GetColorOrDefault(Color color, Color defaultColor)
			=> color.IsDefault ? defaultColor : color;

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