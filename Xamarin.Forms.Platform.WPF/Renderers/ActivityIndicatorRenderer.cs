using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.WPF.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, FormsProgressRing>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null) // construct and SetNativeControl and suscribe control event
				{
					SetNativeControl(new FormsProgressRing());
				}

				UpdateIsActive();
				UpdateColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ActivityIndicator.IsRunningProperty.PropertyName)
				UpdateIsActive();
			else if (e.PropertyName == ActivityIndicator.ColorProperty.PropertyName)
				UpdateColor();
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			// Restrict control to a square
			return base.GetDesiredSize(Math.Min(widthConstraint, heightConstraint), Math.Min(widthConstraint, heightConstraint));
		}

		protected override void Dispose(bool disposing)
		{
			if (Element is object)
			{
				Element.IsRunning = false;
			}

			base.Dispose(disposing);
		}

		void UpdateColor()
		{
			Control.UpdateDependencyColor(FormsProgressRing.ForegroundProperty, Element.Color);
		}

		void UpdateIsActive()
		{
			Control.IsActive = Element.IsRunning;
		}
	}
}
