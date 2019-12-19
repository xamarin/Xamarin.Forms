﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms.Platform;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CarouselViewRenderer))]
	public class CarouselView : ItemsView
	{
		public const string CurrentItemVisualState = "CurrentItem";
		public const string NextItemVisualState = "NextItem";
		public const string PreviousItemVisualState = "PreviousItem";
		public const string VisibleItemVisualState = "VisibleItem";
		public const string DefaultItemVisualState = "DefaultItem";

		public static readonly BindableProperty PeekAreaInsetsProperty = BindableProperty.Create(nameof(PeekAreaInsets), typeof(Thickness), typeof(CarouselView), default(Thickness));

		public Thickness PeekAreaInsets
		{
			get { return (Thickness)GetValue(PeekAreaInsetsProperty); }
			set { SetValue(PeekAreaInsetsProperty, value); }
		}

		static readonly BindablePropertyKey VisibleViewsPropertyKey = BindableProperty.CreateReadOnly(nameof(VisibleViews), typeof(List<View>), typeof(CarouselView), null);

		public static readonly BindableProperty VisibleViewsProperty = VisibleViewsPropertyKey.BindableProperty;

		public List<View> VisibleViews => (List<View>)GetValue(VisibleViewsProperty);

		static readonly BindablePropertyKey IsDraggingPropertyKey = BindableProperty.CreateReadOnly(nameof(IsDragging), typeof(bool), typeof(CarouselView), false);

		public static readonly BindableProperty IsDraggingProperty = IsDraggingPropertyKey.BindableProperty;

		public bool IsDragging => (bool)GetValue(IsDraggingProperty);

		public static readonly BindableProperty IsBounceEnabledProperty =
			BindableProperty.Create(nameof(IsBounceEnabled), typeof(bool), typeof(CarouselView), true);

		public bool IsBounceEnabled
		{
			get { return (bool)GetValue(IsBounceEnabledProperty); }
			set { SetValue(IsBounceEnabledProperty, value); }
		}

		public static readonly BindableProperty IsSwipeEnabledProperty =
			BindableProperty.Create(nameof(IsSwipeEnabled), typeof(bool), typeof(CarouselView), true);

		public bool IsSwipeEnabled
		{
			get { return (bool)GetValue(IsSwipeEnabledProperty); }
			set { SetValue(IsSwipeEnabledProperty, value); }
		}

		public static readonly BindableProperty IsScrollAnimatedProperty =
		BindableProperty.Create(nameof(IsScrollAnimated), typeof(bool), typeof(CarouselView), true);

		public bool IsScrollAnimated
		{
			get { return (bool)GetValue(IsScrollAnimatedProperty); }
			set { SetValue(IsScrollAnimatedProperty, value); }
		}

		public static readonly BindableProperty CurrentItemProperty =
		BindableProperty.Create(nameof(CurrentItem), typeof(object), typeof(CarouselView), default, BindingMode.TwoWay, 
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

		public static readonly BindableProperty ItemsLayoutProperty =
			BindableProperty.Create(nameof(ItemsLayout), typeof(LinearItemsLayout), typeof(ItemsView),
				LinearItemsLayout.CarouselDefault);

		[TypeConverter(typeof(CarouselLayoutTypeConverter))]
		public LinearItemsLayout ItemsLayout
		{
			get => (LinearItemsLayout)GetValue(ItemsLayoutProperty);
			set => SetValue(ItemsLayoutProperty, value);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsScrolling { get; set; }

		public event EventHandler<CurrentItemChangedEventArgs> CurrentItemChanged;
		public event EventHandler<PositionChangedEventArgs> PositionChanged;

		public CarouselView()
		{
			VerifyCarouselViewFlagEnabled(constructorHint: nameof(CarouselView));
			ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Horizontal)
			{
				SnapPointsType = SnapPointsType.MandatorySingle,
				SnapPointsAlignment = SnapPointsAlignment.Center
			};
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void VerifyCarouselViewFlagEnabled(
			string constructorHint = null,
			[CallerMemberName] string memberName = "")
		{
			try
			{
				ExperimentalFlags.VerifyFlagEnabled(nameof(CollectionView), ExperimentalFlags.CarouselViewExperimental,
					constructorHint, memberName);
			}
			catch (InvalidOperationException)
			{
			
			}
		}

		protected virtual void OnPositionChanged(PositionChangedEventArgs args)
		{
		}

		protected virtual void OnCurrentItemChanged(EventArgs args)
		{
		}

		protected override void OnScrolled(ItemsViewScrolledEventArgs e)
		{
			CurrentItem = GetItemForPosition(this, e.CenterItemIndex);

			base.OnScrolled(e);
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

			// If the user is interacting with the Carousel or the Carousel is doing ScrollTo, we don't need to scroll to item.
			if (!carousel.IsDragging && !carousel.IsScrolling)
				carousel.ScrollTo(args.CurrentPosition, position: ScrollToPosition.Center, animate: carousel.IsScrollAnimated);

			carousel.OnPositionChanged(args);
		}

		static object GetItemForPosition(CarouselView carouselView, int index)
		{
			if (!(carouselView?.ItemsSource is IList itemSource))
				return null;

			if (itemSource.Count == 0)
				return null;

			return itemSource[index];
		}

		static int GetPositionForItem(CarouselView carouselView, object item)
		{
			var itemSource = carouselView?.ItemsSource as IList;

			for (int n = 0; n < itemSource?.Count; n++)
			{
				if (itemSource[n] == item)
				{
					return n;
				}
			}
			return 0;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetCurrentItem(object item)
		{
			SetValueFromRenderer(CurrentItemProperty, item);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetIsDragging(bool value)
		{
			SetValue(IsDraggingPropertyKey, value);
		}
	}
}
