using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, ProgressRing>
	{
		object _foregroundDefault;

		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(
						new ProgressRing
						{
							IsActive = Element.IsRunning,
							Visibility = Element.IsVisible ? Windows.UI.Xaml.Visibility.Visible : Visibility.Collapsed,
							IsEnabled = Element.IsEnabled
						});

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
				Control.Foreground = color.ToBrush();
			}
		}

		void UpdateIsRunning()
		{
			Control.Opacity = Element.IsRunning ? Element.Opacity : 0;
		}
	}
}