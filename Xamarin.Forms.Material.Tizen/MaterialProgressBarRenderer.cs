using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using Xamarin.Forms.Material.Tizen;
using Tizen.NET.MaterialComponents;
using TForms = Xamarin.Forms.Platform.Tizen.Forms;
using EColor = ElmSharp.Color;

[assembly: ExportRenderer(typeof(ProgressBar), typeof(MaterialProgressBarRenderer), new[] { typeof(VisualMarker.MaterialVisual) })]
namespace Xamarin.Forms.Material.Tizen
{
	public class MaterialProgressBarRenderer : ProgressBarRenderer
	{
		EColor _defaultColor = MaterialColors.Light.PrimaryColor;
		EColor _defaultBackgroundColor = EColor.FromRgba(MaterialColors.Light.PrimaryColor.R, MaterialColors.Light.PrimaryColor.G, MaterialColors.Light.PrimaryColor.B, (int)Math.Round(MaterialColors.Light.PrimaryColor.A * MaterialColors.SliderTrackAlpha));

		protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
		{
			if (Control == null)
			{
				SetNativeControl(new MProgressIndicator(TForms.NativeParent));
			}
			base.OnElementChanged(e);
		}

		protected override void UpdateProgressColor(bool initialize)
		{
			Control.Color = Element.ProgressColor == Color.Default ? _defaultColor : Element.ProgressColor.ToNative();
			Control.BackgroundColor = Element.ProgressColor == Color.Default ? _defaultBackgroundColor : EColor.FromRgba(Control.Color.R, Control.Color.G, Control.Color.B, (int)Math.Round(Control.Color.A * MaterialColors.SliderTrackAlpha));
		}
	}
}
