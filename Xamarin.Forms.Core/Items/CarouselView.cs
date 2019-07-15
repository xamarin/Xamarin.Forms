using System;
using System.Collections;
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
	}

	[RenderWith(typeof(_CarouselViewRenderer))]
	public class CarouselView : SelectableItemsView, ICarouselViewController
	{
		public static readonly BindableProperty IsSwipeEnabledProperty = BindableProperty.Create(nameof(IsSwipeEnabled), typeof(bool), typeof(CarouselView), true);

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

		public event EventHandler<PositionChangedEventArgs> PositionChanged;

		protected virtual void OnPositionChanged(PositionChangedEventArgs args)
		{
			ScrollTo(args.CurrentPosition, animate: AnimateTransition);
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

			carousel.OnPositionChanged(args);
		}

		public event EventHandler<ScrolledDirectionEventArgs> Scrolled;

		public CarouselView()
		{
			CollectionView.VerifyCollectionViewFlagEnabled(constructorHint: nameof(CarouselView));
			ItemsLayout = new ListItemsLayout(ItemsLayoutOrientation.Horizontal)
			{
				SnapPointsType = SnapPointsType.MandatorySingle,
				SnapPointsAlignment = SnapPointsAlignment.Center
			};
			SelectionMode = SelectionMode.Single;
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (propertyName.Equals(SelectableItemsView.SelectedItemProperty.PropertyName))
			{
				SetValueCore(PositionProperty, GetPositionForItem(SelectedItem));
			}
			base.OnPropertyChanged(propertyName);
		}

		int GetPositionForItem(object item)
		{
			var itemSource = ItemsSource as IList;
			for (int n = 0; n < itemSource.Count; n++)
			{
				if (itemSource[n] == item)
				{
					return n;
				}
			}
			return 0;
		}

		public void SendScrolled(double value, ScrollDirection direction)
		{
			Scrolled?.Invoke(this, new ScrolledDirectionEventArgs(direction, value));
		}
	}
}