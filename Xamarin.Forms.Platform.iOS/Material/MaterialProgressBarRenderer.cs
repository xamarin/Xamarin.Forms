using System.ComponentModel;
using CoreGraphics;
using MaterialComponents;
using UIKit;
using Xamarin.Forms;
using MProgressView = MaterialComponents.ProgressView;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ProgressBar), typeof(Xamarin.Forms.Platform.iOS.Material.MaterialProgressBarRenderer), new[] { typeof(VisualRendererMarker.Material) })]

namespace Xamarin.Forms.Platform.iOS.Material
{
	public class MaterialProgressBarRenderer : ViewRenderer<ProgressBar, MProgressView>
	{
		UIColor _defaultTrackColor;
		UIColor _defaultProgressColor;

		public MaterialProgressBarRenderer()
		{
			VisualElement.VerifyVisualFlagEnabled();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(CreateNativeControl());
				}

				UpdateProgressColor();
				UpdateProgress();
			}
		}

		protected virtual IColorScheming CreateColorScheme()
		{
			return MaterialColors.Light.CreateColorScheme();
		}

		protected override MProgressView CreateNativeControl()
		{
			var progressBar = new MProgressView();

			// TODO: fix this once Google implements the new way
#pragma warning disable CS0618 // Type or member is obsolete
			var cs = CreateColorScheme();
			var temp = new BasicColorScheme(cs.PrimaryColor);
			ProgressViewColorThemer.ApplyColorScheme(temp, progressBar);
#pragma warning restore CS0618 // Type or member is obsolete

			return progressBar;
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			var result = base.SizeThatFits(size);
			var height = result.Height;

			if (height == 0)
			{
				if (System.nfloat.IsInfinity(size.Height))
				{
					height = 5;
				}
				else
				{
					height = size.Height;
				}

			}
			return new CGSize(10, height);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ProgressBar.ProgressColorProperty.PropertyName)
				UpdateProgressColor();
			else if (e.PropertyName == ProgressBar.ProgressProperty.PropertyName)
				UpdateProgress();
		}

		protected override void SetBackgroundColor(Color color)
		{
			base.SetBackgroundColor(color);

			if (Control == null)
				return;

			if (color.IsDefault && _defaultTrackColor == null)
				return;

			if (_defaultTrackColor == null)
				_defaultTrackColor = Control.TrackTintColor;

			if (color.IsDefault)
				Control.TrackTintColor = _defaultTrackColor;
			else
				Control.TrackTintColor = color.ToUIColor();
		}

		void UpdateProgressColor()
		{
			if (Control == null)
				return;

			Color color = Element.ProgressColor;
			if (color.IsDefault && _defaultProgressColor == null)
				return;

			if (_defaultProgressColor == null)
				_defaultProgressColor = Control.ProgressTintColor;

			if (color.IsDefault)
				Control.ProgressTintColor = _defaultProgressColor;
			else
				Control.ProgressTintColor = color.ToUIColor();
		}

		void UpdateProgress()
		{
			if (Control == null)
				return;

			Control.Progress = (float)Element.Progress;
		}
	}
}