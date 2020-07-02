using System.ComponentModel;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Platform.UWP
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, FrameworkElement>
	{
		object _foregroundDefault;

		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var activityIndicatorStyle = Element.OnThisPlatform().GetActivityIndicatorStyle();
					if (activityIndicatorStyle == PlatformConfiguration.WindowsSpecific.ActivityIndicator
						.ActivityIndicatorType.Ring)
					{
						SetNativeControl(new ProgressRing
						{
							IsActive = Element.IsRunning,
							Visibility =
								Element.IsVisible ? Windows.UI.Xaml.Visibility.Visible : Visibility.Collapsed,
							IsEnabled = Element.IsEnabled
						});
					}
					else
					{
						SetNativeControl(new FormsProgressBar
						{
							IsIndeterminate = true,
							Style = Windows.UI.Xaml.Application.Current.Resources["FormsProgressBarStyle"] as
									Windows.UI.Xaml.Style
						});
					}

					Control.Loaded += OnControlLoaded;
				}

				// UpdateColor() called when loaded to ensure we can cache dynamic default colors
				UpdateIsRunning();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ActivityIndicator.IsRunningProperty.PropertyName ||
				e.PropertyName == VisualElement.OpacityProperty.PropertyName)
				UpdateIsRunning();
			else if (e.PropertyName == ActivityIndicator.ColorProperty.PropertyName)
				UpdateColor();
		}

		void OnControlLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			_foregroundDefault = Control.GetForegroundCache();
			UpdateColor();
		}

		void UpdateColor()
		{
			
			Color color = Element.Color;

			if (color.IsDefault)
			{
				Control.RestoreForegroundCache(_foregroundDefault);
			}
			else
			{
				if (Control is ProgressRing progressRing)
				{
					progressRing.Foreground = color.ToBrush();

				}
				else if (Control is FormsProgressBar formsProgressBar)
				{
					formsProgressBar.Foreground = color.ToBrush();
				}
			}
		}

		void UpdateIsRunning()
		{
			if (Control is ProgressRing progressRing)
			{
				progressRing.Opacity = Element.IsRunning ? Element.Opacity : 0;
			}
			else if (Control is FormsProgressBar formsProgressBar)
			{
				formsProgressBar.ElementOpacity = Element.IsRunning ? Element.Opacity : 0;
			}
		}
	}
}