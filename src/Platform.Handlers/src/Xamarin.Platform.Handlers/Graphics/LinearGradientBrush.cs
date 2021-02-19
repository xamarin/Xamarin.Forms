using Xamarin.Forms;

namespace Xamarin.Platform
{
	public class LinearGradientBrush2 : GradientBrush2
	{
		public LinearGradientBrush2()
		{

		}

		public LinearGradientBrush2(GradientStopCollection2 gradientStops)
		{
			GradientStops = gradientStops;
		}

		public LinearGradientBrush2(GradientStopCollection2 gradientStops, Point startPoint, Point endPoint)
		{
			GradientStops = gradientStops;
			StartPoint = startPoint;
			EndPoint = endPoint;
		}

		public override bool IsEmpty
		{
			get
			{
				var linearGradientBrush = this;
				return linearGradientBrush == null || linearGradientBrush.GradientStops.Count == 0;
			}
		}

		public Point StartPoint { get; set; } = new Point(0, 0);

		public Point EndPoint { get; set; } = new Point(1, 1);
	}
}