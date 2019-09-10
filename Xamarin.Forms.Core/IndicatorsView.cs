using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public enum IndicatorShape
	{
		Circle,
		Square
	}

	public class IndicatorsView : StackLayout
	{
		public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(int), typeof(IndicatorsView), default(int), BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as IndicatorsView)?.ResetIndicatorsStyles();
		});

		public static readonly BindableProperty CountProperty = BindableProperty.Create(nameof(Count), typeof(int), typeof(IndicatorsView), default(int), propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as IndicatorsView)?.ResetIndicatorsCount((int)oldValue, (int)newValue);
		});

		public static readonly BindableProperty MaxVisibleCountProperty = BindableProperty.Create(nameof(MaxVisibleCount), typeof(int), typeof(IndicatorsView), int.MaxValue, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as IndicatorsView)?.ResetIndicatorsStyles();
		});

		public static readonly BindableProperty IndicatorTemplateProperty = BindableProperty.Create(nameof(IndicatorTemplate), typeof(DataTemplate), typeof(IndicatorsView), default(DataTemplate), propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as IndicatorsView)?.ResetIndicatorsStyles();
		});

		public static readonly BindableProperty IndicatorShapeProperty = BindableProperty.Create(nameof(IndicatorShape), typeof(IndicatorShape), typeof(IndicatorsView), default(IndicatorShape), propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as IndicatorsView)?.ResetIndicatorsStyles();
		});

		public static readonly BindableProperty IndicatorsColorProperty = BindableProperty.Create(nameof(IndicatorsColor), typeof(Color), typeof(IndicatorsView), Color.Silver, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as IndicatorsView)?.ResetIndicatorsStyles();
		});

		public static readonly BindableProperty SelectedIndicatorColorProperty = BindableProperty.Create(nameof(SelectedIndicatorColor), typeof(Color), typeof(IndicatorsView), Color.Gray, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as IndicatorsView)?.ResetIndicatorsStyles();
		});

		public static readonly BindableProperty IndicatorsSizeProperty = BindableProperty.Create(nameof(IndicatorsSize), typeof(double), typeof(IndicatorsView), 10.0, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as IndicatorsView)?.ResetIndicatorsStyles();
		});

		internal static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(IndicatorsView), null, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as IndicatorsView)?.ResetItemsSource(oldValue as IEnumerable);
		});

		static IndicatorsView()
		{
		}

		public IndicatorsView()
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

		public int MaxVisibleCount
		{
			get => (int)GetValue(MaxVisibleCountProperty);
			set => SetValue(MaxVisibleCountProperty, value);
		}

		public DataTemplate IndicatorTemplate
		{
			get => GetValue(IndicatorTemplateProperty) as DataTemplate;
			set => SetValue(IndicatorTemplateProperty, value);
		}

		public IndicatorShape IndicatorShape
		{
			get => (IndicatorShape)GetValue(IndicatorShapeProperty);
			set => SetValue(IndicatorShapeProperty, value);
		}

		public Color IndicatorsColor
		{
			get => (Color)GetValue(IndicatorsColorProperty);
			set => SetValue(IndicatorsColorProperty, value);
		}

		public Color SelectedIndicatorColor
		{
			get => (Color)GetValue(SelectedIndicatorColorProperty);
			set => SetValue(SelectedIndicatorColorProperty, value);
		}

		public double IndicatorsSize
		{
			get => (double)GetValue(IndicatorsSizeProperty);
			set => SetValue(IndicatorsSizeProperty, value);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable ItemsSource
		{
			get => GetValue(ItemsSourceProperty) as IEnumerable;
			set => SetValue(ItemsSourceProperty, value);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new IList<View> Children => base.Children;

		public static void SetItemsSourceBy(IndicatorsView indicatorsView, CarouselView carouselView)
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
			=> view.BackgroundColor = IndicatorsColor;

		private void AddExtraIndicatorsItems()
		{
			var oldCount = Children.Count;
			for (var i = 0; i < Count - oldCount && i < MaxVisibleCount - oldCount; ++i)
			{
				var indicator = IndicatorTemplate?.CreateContent() as View ?? new Frame
				{
					Padding = 0,
					HasShadow = false,
					BorderColor = Color.Transparent,
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center,
					WidthRequest = IndicatorsSize,
					HeightRequest = IndicatorsSize,
					CornerRadius = IndicatorShape == IndicatorShape.Circle
						? (float)IndicatorsSize / 2
						: 0
				};
				var itemTapGesture = new TapGestureRecognizer();
				itemTapGesture.Tapped += (tapSender, tapArgs) => Position = IndexOf(tapSender as View);
				indicator.GestureRecognizers.Add(itemTapGesture);
				Children.Add(indicator);
			}
		}

		private void RemoveRedundantIndicatorsItems()
		{
			foreach (var item in Children.Where((v, i) => i >= Count).ToArray())
			{
				Children.Remove(item);
			}
		}

		private int IndexOf(View view) => Children.IndexOf(view);

		private void ApplyStyle(View view)
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

		private void ResetIndicatorsStylesNonBatch()
		{
			foreach (var child in Children)
			{
				ApplyStyle(child);
			}
		}

		private void ResetIndicatorsStyles()
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

		private void ResetIndicatorsCount(int oldValue, int newValue)
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

		private void ResetItemsSource(IEnumerable oldCollection)
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

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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