using Xamarin.Forms;

namespace Xamarin.Platform
{
	public static class BoxViewExtensions
	{
		public static void UpdateColor(this NativeBoxView nativeView, IBox boxView)
		{
			Color color;

			if (!boxView.Color.IsDefault)
				color = boxView.Color;
			else
				color = boxView.BackgroundColor;

			nativeView.Color = Color.Purple;
		}

		public static void UpdateCornerRadius(this NativeBoxView nativeView, IBox boxView)
		{
			nativeView.CornerRadius = boxView.CornerRadius;
		}
	}
}