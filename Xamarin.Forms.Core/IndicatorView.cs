using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Xamarin.Forms
{
	public class IndicatorView : StackLayout
	{
		public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(int), typeof(IndicatorView), default(int), BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorsStyles());

		public static readonly BindableProperty CountProperty = BindableProperty.Create(nameof(Count), typeof(int), typeof(IndicatorView), default(int), propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorsCount((int)oldValue, (int)newValue));

		public static readonly BindableProperty MaximumVisibleCountProperty = BindableProperty.Create(nameof(MaximumVisibleCount), typeof(int), typeof(IndicatorView), int.MaxValue, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorsStyles());

		public static readonly BindableProperty IndicatorTemplateProperty = BindableProperty.Create(nameof(IndicatorTemplate), typeof(DataTemplate), typeof(IndicatorView), default(DataTemplate), propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorsStyles());

		public static readonly BindableProperty IndicatorColorProperty = BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(IndicatorView), Color.Silver, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorsStyles());

		public static readonly BindableProperty SelectedIndicatorColorProperty = BindableProperty.Create(nameof(SelectedIndicatorColor), typeof(Color), typeof(IndicatorView), Color.Gray, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorsStyles());

		public static readonly BindableProperty IndicatorSizeProperty = BindableProperty.Create(nameof(IndicatorSize), typeof(double), typeof(IndicatorView), 10.0, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorsStyles());

		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(IndicatorView), null, propertyChanged: (bindable, oldValue, newValue)
			=> ((IndicatorView)bindable).ResetIndicatorsStyles());

		public IndicatorView()
		{
			Orientation = StackOrientation.Horizontal;
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
			get => GetValue(IndicatorTemplateProperty) as DataTemplate;
			set => SetValue(IndicatorTemplateProperty, value);
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
			get => GetValue(ItemsSourceProperty) as IEnumerable;
			set => SetValue(ItemsSourceProperty, value);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new IList<View> Children => base.Children;

		public static void SetItemsSourceBy(IndicatorView indicatorsView, CarouselView carouselView)
			=> indicatorsView.SetItemsSourceBy(carouselView);

		public void SetItemsSourceBy(CarouselView carouselView)
		{
			SetBinding(PositionProperty, new Binding
			{
				Path = nameof(CarouselView.Position),
				Source = carouselView
			});
			SetBinding(ItemsSourceProperty, new Binding
			{
				Path = nameof(ItemsView.ItemsSource),
				Source = carouselView
			});
		}

		protected virtual void ApplySelectedStyle(View view, int index)
			=> view.BackgroundColor = SelectedIndicatorColor;

		protected virtual void ApplyUnselectedStyle(View view, int index)
			=> view.BackgroundColor = IndicatorColor;

		void AddExtraIndicatorsItems()
		{
			var oldCount = Children.Count;
			for (var i = 0; i < Count - oldCount && i < MaximumVisibleCount - oldCount; ++i)
			{
				var indicator = IndicatorTemplate?.CreateContent() as View ?? new Frame
				{
					Padding = 0,
					HasShadow = false,
					BorderColor = Color.Transparent,
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center,
					WidthRequest = IndicatorSize,
					HeightRequest = IndicatorSize,
					CornerRadius = (float)IndicatorSize / 2
				};
				var itemTapGesture = new TapGestureRecognizer();
				itemTapGesture.Tapped += (tapSender, tapArgs) => Position = IndexOf(tapSender as View);
				indicator.GestureRecognizers.Add(itemTapGesture);
				Children.Add(indicator);
			}
		}

		void RemoveRedundantIndicatorsItems()
		{
			foreach (var item in Children.Where((v, i) => i >= Count).ToArray())
			{
				Children.Remove(item);
			}
		}

		int IndexOf(View view) => Children.IndexOf(view);

		void ApplyStyle(View view)
		{
			try
			{
				view.BatchBegin();
				var index = IndexOf(view);
				if (index == Position)
				{
					ApplySelectedStyle(view, index);
					return;
				}
				ApplyUnselectedStyle(view, index);
			}
			finally
			{
				view.BatchCommit();
			}
		}

		void ResetIndicatorsStylesNonBatch()
		{
			foreach (var child in Children)
			{
				ApplyStyle(child);
			}
		}

		void ResetIndicatorsStyles()
		{
			try
			{
				BatchBegin();
				ResetIndicatorsStylesNonBatch();
			}
			finally
			{
				BatchCommit();
			}
		}

		void ResetIndicatorsCount(int oldValue, int newValue)
		{
			try
			{
				BatchBegin();
				if (oldValue < 0)
				{
					oldValue = 0;
				}

				if (oldValue > newValue)
				{
					RemoveRedundantIndicatorsItems();
					return;
				}

				AddExtraIndicatorsItems();
			}
			finally
			{
				ResetIndicatorsStylesNonBatch();
				BatchCommit();
			}
		}

		void ResetItemsSource(IEnumerable oldCollection)
		{
			if (oldCollection is INotifyCollectionChanged oldObservableCollection)
			{
				oldObservableCollection.CollectionChanged -= OnCollectionChanged;
			}

			if (ItemsSource is INotifyCollectionChanged observableCollection)
			{
				observableCollection.CollectionChanged += OnCollectionChanged;
			}

			OnCollectionChanged(ItemsSource, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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