using Android.Content;
using Android.Util;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ChangeDensityButton), typeof(ChangeDensityButtonRenderer))]
namespace Xamarin.Forms.ControlGallery.Android
{
	public class ChangeDensityButtonRenderer : Xamarin.Forms.Platform.Android.FastRenderers.ButtonRenderer
	{
		private DisplayMetricsDensity _previousDensityDpi;

		private int? _previousHeightPixels;
		private int? _previousWidthPixels;

		private float? _previousScaledDensity;
		private float? _previousDensity;
		private float? _previousXdpi;
		private float? _previousYDpi;

		private bool restoreDensity;

		public ChangeDensityButtonRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if(Element.Command == null)
				Element.Command = new Command(ChangeDensityClicked);
		}

		void ChangeDensityClicked()
		{
			using (var metrics = Context.Resources.DisplayMetrics)
			{
				BackupMetricsParameters(metrics);

				metrics.Density = restoreDensity ? _previousDensity.Value : 1.5f;
				metrics.DensityDpi = restoreDensity ? _previousDensityDpi : DisplayMetricsDensity.D260;
				metrics.HeightPixels = restoreDensity ? _previousHeightPixels.Value : 1104;
				metrics.WidthPixels = restoreDensity ? _previousWidthPixels.Value : 1920;
				metrics.ScaledDensity = restoreDensity ? _previousScaledDensity.Value : 1.5f;
				metrics.Xdpi =  restoreDensity ? _previousXdpi.Value : 254.0f;
				metrics.Ydpi = restoreDensity ? _previousYDpi.Value : 254.0f;

				Context.Resources.DisplayMetrics.SetTo(metrics);
				restoreDensity = !restoreDensity;
			}
		}

		void BackupMetricsParameters(DisplayMetrics metrics)
		{
			if (_previousDensity.HasValue)
				return;

			_previousDensity = metrics.Density;
			_previousDensityDpi = metrics.DensityDpi;
			_previousHeightPixels = metrics.HeightPixels;
			_previousWidthPixels = metrics.WidthPixels;
			_previousScaledDensity = metrics.ScaledDensity;
			_previousXdpi = metrics.Xdpi;
			_previousYDpi = metrics.Ydpi;
		}
	}
}