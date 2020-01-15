using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
    [RenderWith(typeof(_EllipseRenderer))]
    public sealed class Ellipse : Shape
    {
		public Ellipse()
		{
			Aspect = Stretch.Fill;
		}
	}
}