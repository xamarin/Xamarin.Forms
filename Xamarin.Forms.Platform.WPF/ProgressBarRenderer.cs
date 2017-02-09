using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.WPF
{
	public class ProgressBarRenderer : ViewRenderer<ProgressBar, System.Windows.Controls.ProgressBar>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
		{
			base.OnElementChanged(e);

			var progressBar = new System.Windows.Controls.ProgressBar { Minimum = 0, Maximum = 1, Value = Element.Progress };
			progressBar.ValueChanged += ProgressBarOnValueChanged;
		    progressBar.Foreground = Brushes.DeepSkyBlue;
			SetNativeControl(progressBar);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			switch (e.PropertyName)
			{
				case "Progress":
					Control.Value = Element.Progress;
					break;
			}
		}

		void ProgressBarOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs)
		{
			((IVisualElementController)Element)?.InvalidateMeasure(InvalidationTrigger.MeasureChanged);
		}
	}
}