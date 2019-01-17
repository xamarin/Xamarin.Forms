using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{

	public enum IndicatorVisibility
	{
		Bottom,
		Top,
		Left,
		Right,
		Hidden,
	}

	public enum IndicatorsShape
	{
		Circle,
		Square
	}

	public class ScrolledDirectionEventArgs : EventArgs
	{
		public double NewValue { get; set; }
		public ScrollDirection Direction { get; set; }
	}

	public enum ScrollDirection
	{
		Left,
		Right,
		Up,
		Down
	}

	public interface ICarouselViewController : IViewController
	{
		void NotifyPositionChanged(int newPosition);
		void SendScrolled(double value, ScrollDirection direction);
	}

	[RenderWith(typeof(_CarouselViewRenderer))]
	public class CarouselView : IndexableSelectableItemsView
	{
		public static readonly BindableProperty IsSwipeEnabledProperty = BindableProperty.Create(nameof(IsSwipeEnabled), typeof(bool), typeof(CarouselView), true);

		public bool IsSwipeEnabled
		{
			get { return (bool)GetValue(IsSwipeEnabledProperty); }
			set { SetValue(IsSwipeEnabledProperty, value); }
		}

		public static readonly BindableProperty IndicatorsVisibilityProperty =
				BindableProperty.Create(nameof(IndicatorsVisibility), typeof(IndicatorVisibility), typeof(CarouselView),
					IndicatorVisibility.Bottom);

		public IndicatorVisibility IndicatorsVisibility
		{
			get { return (IndicatorVisibility)GetValue(IndicatorsVisibilityProperty); }
			set { SetValue(IndicatorsVisibilityProperty, value); }
		}

		public static readonly BindableProperty ItemSpacingProperty =
				BindableProperty.Create(nameof(ItemSpacing), typeof(double), typeof(CarouselView), 0.0);

		public double ItemSpacing
		{
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, value); }
		}

		public static readonly BindableProperty IndicatorsTintColorProperty = BindableProperty.Create(nameof(IndicatorsTintColor), typeof(Color), typeof(CarouselView), Color.Silver);

		public Color IndicatorsTintColor
		{
			get { return (Color)GetValue(IndicatorsTintColorProperty); }
			set { SetValue(IndicatorsTintColorProperty, value); }
		}

		public static readonly BindableProperty SelectedIndicatorTintColorProperty = BindableProperty.Create(nameof(SelectedIndicatorTintColor), typeof(Color), typeof(CarouselView), Color.Gray);

		public Color SelectedIndicatorTintColor
		{
			get { return (Color)GetValue(SelectedIndicatorTintColorProperty); }
			set { SetValue(SelectedIndicatorTintColorProperty, value); }
		}

		public static readonly BindableProperty IndicatorsShapeProperty = BindableProperty.Create(nameof(IndicatorsShape), typeof(IndicatorsShape), typeof(CarouselView), IndicatorsShape.Circle);

		public IndicatorsShape IndicatorsShape
		{
			get { return (IndicatorsShape)GetValue(IndicatorsShapeProperty); }
			set { SetValue(IndicatorsShapeProperty, value); }
		}

		public static readonly BindableProperty AnimateTransitionProperty = BindableProperty.Create(nameof(AnimateTransition), typeof(bool), typeof(CarouselView), true);

		public bool AnimateTransition
		{
			get { return (bool)GetValue(AnimateTransitionProperty); }
			set { SetValue(AnimateTransitionProperty, value); }
		}

		public static readonly BindableProperty ShowArrowsProperty = BindableProperty.Create(nameof(ShowArrows), typeof(bool), typeof(CarouselView), false);

		public bool ShowArrows
		{
			get { return (bool)GetValue(ShowArrowsProperty); }
			set { SetValue(ShowArrowsProperty, value); }
		}

		public static readonly BindableProperty ArrowsBackgroundColorProperty = BindableProperty.Create(nameof(ArrowsBackgroundColor), typeof(Color), typeof(CarouselView), Color.Black);

		public Color ArrowsBackgroundColor
		{
			get { return (Color)GetValue(ArrowsBackgroundColorProperty); }
			set { SetValue(ArrowsBackgroundColorProperty, value); }
		}

		public static readonly BindableProperty ArrowsTintColorProperty = BindableProperty.Create(nameof(ArrowsTintColor), typeof(Color), typeof(CarouselView), Color.White);

		public Color ArrowsTintColor
		{
			get { return (Color)GetValue(ArrowsTintColorProperty); }
			set { SetValue(ArrowsTintColorProperty, value); }
		}

		// Not working on UWP
		public static readonly BindableProperty ArrowsTransparencyProperty = BindableProperty.Create(nameof(ArrowsTransparency), typeof(float), typeof(CarouselView), 0.5f);

		public float ArrowsTransparency
		{
			get { return (float)GetValue(ArrowsTransparencyProperty); }
			set { SetValue(ArrowsTransparencyProperty, value); }
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
		}
	}
}