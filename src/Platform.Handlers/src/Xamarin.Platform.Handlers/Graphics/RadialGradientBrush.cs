using Xamarin.Forms;

namespace Xamarin.Platform
{
	public class RadialGradientBrush2 : GradientBrush2
	{
		public RadialGradientBrush2()
		{

		}

		public RadialGradientBrush2(GradientStopCollection2 gradientStops)
		{
			GradientStops = gradientStops;
		}

		public RadialGradientBrush2(GradientStopCollection2 gradientStops, double radius)
		{
			GradientStops = gradientStops;
			Radius = radius;
		}

		public RadialGradientBrush2(GradientStopCollection2 gradientStops, Point center, double radius)
		{
			GradientStops = gradientStops;
			Center = center;
			Radius = radius;
		}

		public override bool IsEmpty
		{
			get
			{
				var radialGradientBrush = this;
				return radialGradientBrush == null || radialGradientBrush.GradientStops.Count == 0;
			}
		}

		public Point Center { get; set; } = new Point(0.5, 0.5);

		public double Radius { get; set; } = 0.5d;
	}
}