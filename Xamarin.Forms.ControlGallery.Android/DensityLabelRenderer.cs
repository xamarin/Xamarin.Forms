using System;
using Android.Content;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DensityLabel), typeof(DensityLabelRenderer))]
namespace Xamarin.Forms.ControlGallery.Android
{
	public class DensityLabelRenderer : Xamarin.Forms.Platform.Android.FastRenderers.LabelRenderer
	{
		public DensityLabelRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			((DensityLabel)Element).GetCurrentDensityValue = new Command(GetCurrentDensityValueCommandExecute);

			GetCurrentDensityValueCommandExecute();
			base.OnElementChanged(e);
		}

		void GetCurrentDensityValueCommandExecute()
		{
			using (var metrics = Context.Resources.DisplayMetrics)
				Element.Text = metrics.Density.ToString();
		}
	}
}