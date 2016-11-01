using System.ComponentModel;
using System.Drawing;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, UIActivityIndicatorView>
	{
		bool _disposed;

		public ActivityIndicatorRenderer()
		{
			MessagingCenter.Subscribe<ListViewRenderer.ListViewDataSource>(this, "PreserveActivityIndicatorState", sender =>
			{
				if (Control != null && !Control.IsAnimating && Element != null && Element.IsRunning)
				{
					Control.StartAnimating();
				}
			});
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new UIActivityIndicatorView(RectangleF.Empty) { ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray });
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
			Control.Color = Element.Color == Color.Default ? null : Element.Color.ToUIColor();
		}

		void UpdateIsRunning()
		{
			if (Element.IsRunning)
				Control.StartAnimating();
			else
				Control.StopAnimating();
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if(disposing)
				MessagingCenter.Unsubscribe<ListViewRenderer.ListViewDataSource>(this, "PreserveActivityIndicatorState");

			_disposed = true;

			base.Dispose(disposing);
		}
	}
}