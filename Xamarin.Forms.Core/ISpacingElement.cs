namespace Xamarin.Forms
{
	interface ISpacingElement
	{
		//note to implementor: implement this property publicly
		double RowSpacing { get; }
		double ColumnSpacing { get; }

		//note to implementor: but implement this method explicitly
		void OnRowSpacingPropertyChanged(double oldValue, double newValue);
		void OnColumnSpacingPropertyChanged(double oldValue, double newValue);
		double RowSpacingDefaultValueCreator();
		double ColumnSpacingDefaultValueCreator();
	}
}