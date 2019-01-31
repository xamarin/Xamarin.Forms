using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	public class ScrolledDirectionEventArgs : EventArgs
	{
		public ScrolledDirectionEventArgs(ScrollDirection direction, double newValue)
		{
			Direction = direction;
			NewValue = newValue;

		}
		public double NewValue { get; private set; }
		public ScrollDirection Direction { get; private set; }
	}

	public enum ScrollDirection
	{
		Left,
		Right,
		Up,
		Down
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICarouselViewController : IViewController
	{
		void SendScrolled(double value, ScrollDirection direction);
		void SetCurrentItem(object item);
		void SetIsScrolling(bool value);
		void SetIsDragging(bool value);
	}

	[RenderWith(typeof(_CarouselViewRenderer))]
	public class CarouselView : ItemsView, ICarouselViewController
	{
		public const string CurrentItemVisualState = "CurrentItem";
		public const string NextItemVisualState = "NextItem";
		public const string PreviousItemVisualState = "PreviousItem";
		public const string DefaultItemVisualState = "DefaultItem";

		public static readonly BindablePropertyKey VisibleViewsPropertyKey = BindableProperty.CreateReadOnly(nameof(VisibleViews), typeof(List<View>), typeof(CarouselView), null);

		public static readonly BindableProperty VisibleViewsProperty = VisibleViewsPropertyKey.BindableProperty;

		public List<View> VisibleViews => (List<View>)GetValue(VisibleViewsProperty);

		public static readonly BindablePropertyKey IsScrollingPropertyKey = BindableProperty.CreateReadOnly(nameof(IsScrolling), typeof(bool), typeof(CarouselView), false);

		public static readonly BindableProperty IsScrollingProperty = IsScrollingPropertyKey.BindableProperty;

		public bool IsScrolling => (bool)GetValue(IsScrollingProperty);

		public static readonly BindablePropertyKey IsDraggingPropertyKey = BindableProperty.CreateReadOnly(nameof(IsDragging), typeof(bool), typeof(CarouselView), false);

		public static readonly BindableProperty IsDraggingProperty = IsDraggingPropertyKey.BindableProperty;

		public bool IsDragging => (bool)GetValue(IsDraggingProperty);

		public static readonly BindableProperty IsBounceEnabledProperty =
			BindableProperty.Create(nameof(IsBounceEnabled), typeof(bool), typeof(CarouselView), true);

		public bool IsBounceEnabled
		{
			get { return (bool)GetValue(IsBounceEnabledProperty); }
			set { SetValue(IsBounceEnabledProperty, value); }
		}

		public static readonly BindableProperty NumberOfVisibleItemsProperty =
			BindableProperty.Create(nameof(NumberOfVisibleItems), typeof(int), typeof(CarouselView), 1);

		public int NumberOfVisibleItems
		{
			get { return (int)GetValue(NumberOfVisibleItemsProperty); }
			set { SetValue(NumberOfVisibleItemsProperty, value); }
		}

		public static readonly BindableProperty IsSwipeEnabledProperty =
			BindableProperty.Create(nameof(IsSwipeEnabled), typeof(bool), typeof(CarouselView), true);

		public bool IsSwipeEnabled
		{
			get { return (bool)GetValue(IsSwipeEnabledProperty); }
			set { SetValue(IsSwipeEnabledProperty, value); }
		}

		public static readonly BindableProperty ItemSpacingProperty =
				BindableProperty.Create(nameof(ItemSpacing), typeof(double), typeof(CarouselView), 0.0);

		public double ItemSpacing
		{
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, value); }
		}

		public static readonly BindableProperty AnimateTransitionProperty =
		BindableProperty.Create(nameof(AnimateTransition), typeof(bool), typeof(CarouselView), true);

		public bool AnimateTransition
		{
			get { return (bool)GetValue(AnimateTransitionProperty); }
			set { SetValue(AnimateTransitionProperty, value); }
		}

		public static readonly BindableProperty CurrentItemProperty =
		BindableProperty.Create(nameof(CurrentItem), typeof(object), typeof(CarouselView), default(object),
			propertyChanged: CurrentItemPropertyChanged);

		public static readonly BindableProperty CurrentItemChangedCommandProperty =
			BindableProperty.Create(nameof(CurrentItemChangedCommand), typeof(ICommand), typeof(CarouselView));

		public static readonly BindableProperty CurrentItemChangedCommandParameterProperty =
			BindableProperty.Create(nameof(CurrentItemChangedCommandParameter), typeof(object), typeof(CarouselView));

		public object CurrentItem
		{
			get => GetValue(CurrentItemProperty);
			set => SetValue(CurrentItemProperty, value);
		}

		public ICommand CurrentItemChangedCommand
		{
			get => (ICommand)GetValue(CurrentItemChangedCommandProperty);
			set => SetValue(CurrentItemChangedCommandProperty, value);
		}

		public object CurrentItemChangedCommandParameter
		{
			get => GetValue(CurrentItemChangedCommandParameterProperty);
			set => SetValue(CurrentItemChangedCommandParameterProperty, value);
		}

		static void CurrentItemPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var carouselView = (CarouselView)bindable;

			var args = new CurrentItemChangedEventArgs(oldValue, newValue);

			var command = carouselView.CurrentItemChangedCommand;

			if (command != null)
			{
				var commandParameter = carouselView.CurrentItemChangedCommandParameter;

				if (command.CanExecute(commandParameter))
				{
					command.Execute(commandParameter);
				}
			}

			carouselView.SetValueCore(PositionProperty, GetPositionForItem(carouselView, newValue));

			carouselView.CurrentItemChanged?.Invoke(carouselView, args);

			carouselView.OnCurrentItemChanged(args);
		}


		public static readonly BindableProperty PositionProperty =
		BindableProperty.Create(nameof(Position), typeof(int), typeof(CarouselView), default(int), BindingMode.TwoWay,
			propertyChanged: PositionPropertyChanged);

		public static readonly BindableProperty PositionChangedCommandProperty =
			BindableProperty.Create(nameof(PositionChangedCommand), typeof(ICommand), typeof(CarouselView));

		public static readonly BindableProperty PositionChangedCommandParameterProperty =
			BindableProperty.Create(nameof(PositionChangedCommandParameter), typeof(object),
				typeof(CarouselView));

		public int Position
		{
			get => (int)GetValue(PositionProperty);
			set => SetValue(PositionProperty, value);
		}

		public ICommand PositionChangedCommand
		{
			get => (ICommand)GetValue(PositionChangedCommandProperty);
			set => SetValue(PositionChangedCommandProperty, value);
		}

		public object PositionChangedCommandParameter
		{
			get => GetValue(PositionChangedCommandParameterProperty);
			set => SetValue(PositionChangedCommandParameterProperty, value);
		}

		public event EventHandler<CurrentItemChangedEventArgs> CurrentItemChanged;
		public event EventHandler<PositionChangedEventArgs> PositionChanged;
		public event EventHandler<ScrolledDirectionEventArgs> Scrolled;


		public CarouselView()
		{
			CollectionView.VerifyCollectionViewFlagEnabled(constructorHint: nameof(CarouselView));
			ItemsLayout = new ListItemsLayout(ItemsLayoutOrientation.Horizontal)
			{

				SnapPointsType = SnapPointsType.MandatorySingle,
				SnapPointsAlignment = SnapPointsAlignment.Center
			};

		}

		protected virtual void OnPositionChanged(PositionChangedEventArgs args)
		{
		}

		protected virtual void OnCurrentItemChanged(EventArgs args)
		{
		}

		static void PositionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var carousel = (CarouselView)bindable;

			var args = new PositionChangedEventArgs((int)oldValue, (int)newValue);

			var command = carousel.PositionChangedCommand;

			if (command != null)
			{
				var commandParameter = carousel.PositionChangedCommandParameter;

				if (command.CanExecute(commandParameter))
				{
					command.Execute(commandParameter);
				}
			}

			carousel.PositionChanged?.Invoke(carousel, args);

			//user is interacting with the carousel we don't need to scroll to item 
			if (!carousel.IsDragging)
				carousel.ScrollTo(args.CurrentPosition, -1, ScrollToPosition.Center, animate: carousel.AnimateTransition);

			carousel.OnPositionChanged(args);
		}


		static int GetPositionForItem(CarouselView carouselView, object item)
		{
			var itemSource = carouselView.ItemsSource as IList;

			for (int n = 0; n < itemSource.Count; n++)
			{
				if (itemSource[n] == item)
				{
					return n;
				}
			}
			return 0;
		}

		void ICarouselViewController.SendScrolled(double value, ScrollDirection direction)
		{
			Scrolled?.Invoke(this, new ScrolledDirectionEventArgs(direction, value));
		}

		void ICarouselViewController.SetCurrentItem(object item)
		{
			SetValueFromRenderer(CurrentItemProperty,item);
		}

		void ICarouselViewController.SetIsScrolling(bool value)
		{
			SetValue(IsScrollingPropertyKey, value);
		}

		void ICarouselViewController.SetIsDragging(bool value)
		{
			SetValue(IsDraggingPropertyKey, value);
		}
	}
}