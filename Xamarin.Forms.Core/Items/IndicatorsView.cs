using System;
using System.ComponentModel;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	public enum IndicatorsShape
	{
		Circle,
		Square
	}

	[RenderWith(typeof(_IndicatorsViewRenderer))]
	public class IndicatorsView : View
	{
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

		public static event EventHandler<PropertyChangedEventArgs> ItemSourcePropertyChanged;

		public static readonly BindableProperty ItemsSourceByProperty = BindableProperty.Create("ItemsSourceBy", typeof(SelectableItemsView), typeof(IndicatorsView), default(SelectableItemsView), 
		propertyChanged: (b, o, n) => {
			var oldItemsView = (o as SelectableItemsView);
			//if(oldItemsView != null)
			//{
			//	oldItemsView.PropertyChanged -= ItemsViewPropertyChanged;
			//}
			(n as SelectableItemsView).PropertyChanged += (s,e) => {

				ItemSourcePropertyChanged?.Invoke(b, e);
			};
		} );

		[TypeConverter(typeof(ReferenceTypeConverter))]
		public static SelectableItemsView GetItemsSourceBy(BindableObject bindable)
		{
			return (SelectableItemsView)bindable.GetValue(ItemsSourceByProperty);
		}

		public static void SetItemsSourceBy(BindableObject bindable, ItemsView value)
		{
			bindable.SetValue(ItemsSourceByProperty, value);
		}

		public IndicatorsView()
		{
		}

	}
}
