namespace Xamarin.Forms
{
	[ContentProperty(nameof(GradientStops))]
	public class GradientBrush : Brush
	{
		public GradientBrush()
		{
			GradientStops = new GradientStopCollection();
		}

		public static readonly BindableProperty GradientStopsProperty = BindableProperty.Create(
			nameof(GradientStops), typeof(GradientStopCollection), typeof(GradientBrush), null);

		public GradientStopCollection GradientStops
		{
			get => (GradientStopCollection)GetValue(GradientStopsProperty);
			set => SetValue(GradientStopsProperty, value);
		}
	}
}