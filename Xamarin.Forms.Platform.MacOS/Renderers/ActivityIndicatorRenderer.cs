using System.ComponentModel;
using System.Drawing;
using AppKit;
using CoreImage;

namespace Xamarin.Forms.Platform.MacOS
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, NSProgressIndicator>
	{
		static CIColorPolynomial _currentColorFilter;
		static NSColor _currentColor;

		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
					SetNativeControl(new NSProgressIndicator(RectangleF.Empty) { Style = NSProgressIndicatorStyle.Spinning });

				UpdateColor();
				UpdateIsRunning();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ActivityIndicator.ColorProperty.PropertyName)
				UpdateColor();
			else if (e.PropertyName == ActivityIndicator.IsRunningProperty.PropertyName)
				UpdateIsRunning();
		}

		void UpdateColor()
		{
			var color = Element.Color;
			if (_currentColorFilter == null && color.IsDefault)
				return;

			if (color.IsDefault)
				Control.ContentFilters = new CIFilter[0];

			var newColor = Element.Color.ToNSColor();
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

		void UpdateIsRunning()
		{
			if (Element.IsRunning)
				Control.StartAnimation(this);
			else
				Control.StopAnimation(this);
		}
	}
}

