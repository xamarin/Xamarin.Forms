using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ChangeDensityButton), typeof(ChangeDensityButtonRenderer))]
namespace Xamarin.Forms.ControlGallery.Android
{
	public class ChangeDensityButtonRenderer : Xamarin.Forms.Platform.Android.FastRenderers.ButtonRenderer
	{
		public ChangeDensityButtonRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			Element.Clicked -= ChangeDensityClicked;
			Element.Clicked += ChangeDensityClicked;
			base.OnElementChanged(e);
		}

		private void ChangeDensityClicked(object sender, EventArgs e)
		{
			using (var metrics = Context.Resources.DisplayMetrics)
			{
				metrics.Density = 1.5f;
				metrics.DensityDpi = DisplayMetricsDensity.D260;
				metrics.HeightPixels = 1104;
				metrics.WidthPixels = 1920;
				metrics.ScaledDensity = 1.5f;
				metrics.Xdpi = 254.0f;
				metrics.Ydpi = 254.0f;

				Context.Resources.DisplayMetrics.SetTo(metrics);
			}
		}
	}
}