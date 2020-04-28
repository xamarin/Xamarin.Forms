namespace Xamarin.Forms
{
	static class SpacingElement
	{
		public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create(nameof(ISpacingElement.RowSpacing), typeof(double), typeof(ISpacingElement), default(double),
			propertyChanged: OnRowSpacingPropertyChanged,
			defaultValueCreator: RowSpacingDefaultValueCreator);

		static void OnRowSpacingPropertyChanged(BindableObject bindable, object oldValue, object newValue) => ((ISpacingElement)bindable).OnRowSpacingPropertyChanged((double)oldValue, (double)newValue);

		static object RowSpacingDefaultValueCreator(BindableObject bindable) => ((ISpacingElement)bindable).RowSpacingDefaultValueCreator();


		public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create(nameof(ISpacingElement.ColumnSpacing), typeof(double), typeof(ISpacingElement), default(double),
			propertyChanged: OnColumnSpacingPropertyChanged,
			defaultValueCreator: ColumnSpacingDefaultValueCreator);

		static void OnColumnSpacingPropertyChanged(BindableObject bindable, object oldValue, object newValue) => ((ISpacingElement)bindable).OnColumnSpacingPropertyChanged((double)oldValue, (double)newValue);

		static object ColumnSpacingDefaultValueCreator(BindableObject bindable) => ((ISpacingElement)bindable).ColumnSpacingDefaultValueCreator();
	}
}