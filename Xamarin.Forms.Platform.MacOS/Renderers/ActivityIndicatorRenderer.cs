using System.ComponentModel;
using System.Drawing;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, NSProgressIndicator>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new NSProgressIndicator(RectangleF.Empty) { Style = NSProgressIndicatorStyle.Spinning });
					Control.WantsLayer = true;
				}

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
			//could be a color filter but it didn't worked
			//Control.ControlTint = NSControlTint. Element.Color == Color.Default ? null : Element.Color.ToNSColor();
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

