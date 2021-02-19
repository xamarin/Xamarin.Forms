namespace Xamarin.Platform
{
	public abstract class GradientBrush2 : Brush2
	{
		public GradientBrush2()
		{
			GradientStops = new GradientStopCollection2();
		}

		public GradientStopCollection2 GradientStops { get; set; }
	}
}