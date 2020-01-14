namespace Xamarin.Forms
{
	public class RadialGradientBrush : GradientBrush
	{
		public static readonly BindableProperty CenterProperty = BindableProperty.Create(
			nameof(Center), typeof(Point), typeof(RadialGradientBrush), default(Point));

		public Point Center
		{
			get => (Point)GetValue(CenterProperty);
			set => SetValue(CenterProperty, value);
		}

		public static readonly BindableProperty GradientOriginProperty = BindableProperty.Create(
			nameof(GradientOrigin), typeof(Point), typeof(RadialGradientBrush), default(Point));

		public Point GradientOrigin
		{
			get => (Point)GetValue(GradientOriginProperty);
			set => SetValue(GradientOriginProperty, value);
		}

		public static readonly BindableProperty RadiusProperty = BindableProperty.Create(
			nameof(Radius), typeof(double), typeof(RadialGradientBrush), default(double));

		public double Radius
		{
			get => (double)GetValue(RadiusProperty);
			set => SetValue(RadiusProperty, value);
		}
	}
}