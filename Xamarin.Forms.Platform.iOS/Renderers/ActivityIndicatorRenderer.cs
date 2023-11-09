using CoreGraphics;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Forms.Platform.iOS
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, UIActivityIndicatorView>
	{
		[Internals.Preserve(Conditional = true)]
		public ActivityIndicatorRenderer()
		{

		}

		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					if (Forms.IsiOS13OrNewer)
						SetNativeControl(new UIActivityIndicatorView(RectangleF.Empty) { ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Medium });
					else
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
			// You can't call StartAnimating until it has been added to the top UIView (its Superview), otherwise it doesn't do
			// anything.It seems to affect any cell based view, where the cell might not be visible, when the Activity starts its animation.
			// See https://github.com/xamarin/Xamarin.Forms/pull/11339
			if (Superview == null || Control?.Superview == null)
				return;

			if (Element.IsRunning)
				Control.StartAnimating();
			else
				Control.StopAnimating();
		}

		internal void PreserveState()
		{
			// Re-apply is running state in case animation was stopped by external means/events and/or Superview changes
			// (e.g. in UITableView, see ListViewRenderer).
			// NOTE: not sure if this is still needed after PR11339
			UpdateIsRunning();
		}

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			// Ensure running state is applied when Superview has changed
			// See https://github.com/xamarin/Xamarin.Forms/pull/11339
			UpdateIsRunning();
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			// Ensure running state is applied when Superview has changed
			// See https://github.com/xamarin/Xamarin.Forms/pull/11339
			UpdateIsRunning();
		}
	}
}
