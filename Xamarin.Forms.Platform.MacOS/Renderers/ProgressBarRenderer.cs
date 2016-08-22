using System.ComponentModel;
using AppKit;
using CoreImage;

namespace Xamarin.Forms.Platform.MacOS
{
	public class ProgressBarRenderer : ViewRenderer<ProgressBar, NSProgressIndicator>
	{
		static CIColorPolynomial _currentColorFilter;
		static NSColor _currentColor;

		protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
		{
			if (e.NewElement != null)
			{
				if (e.NewElement != null)
				{
					if (Control == null)
						SetNativeControl(new NSProgressIndicator { IsDisplayedWhenStopped = true, Style = NSProgressIndicatorStyle.Bar, MinValue = 0, MaxValue = 1 });
					UpdateProgress();
				}

			}
		}
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ProgressBar.ProgressProperty.PropertyName)
				UpdateProgress();
		}

		protected override void SetBackgroundColor(Color color)
		{
			//base.SetBackgroundColor(color);
			if (Control == null)
				return;

			if (_currentColorFilter == null && color.IsDefault)
				return;

			if (color.IsDefault)
				Control.ContentFilters = new CIFilter[0];

			var newColor = Element.BackgroundColor.ToNSColor();
			if (_currentColor == newColor)
				return;

			_currentColor = newColor;

			_currentColorFilter = new CIColorPolynomial
			{
				RedCoefficients = new CIVector(_currentColor.RedComponent),
				BlueCoefficients = new CIVector(_currentColor.BlueComponent),
				GreenCoefficients = new CIVector(_currentColor.GreenComponent)
			};

			Control.ContentFilters = new CIFilter[1] { _currentColorFilter };
		}

		void UpdateProgress()
		{
			Control.DoubleValue = Element.Progress;
		}
	}
}

