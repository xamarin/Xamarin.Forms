using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public sealed class MediaElementRenderer : ViewRenderer<MediaElement, Windows.UI.Xaml.Controls.MediaElement>, IMediaElementRenderer
	{
		Windows.System.Display.DisplayRequest _request = new Windows.System.Display.DisplayRequest();

		long _bufferingProgressChangedToken;

		long _positionChangedToken;

		double IMediaElementRenderer.BufferingProgress
		{
			get { return Control.BufferingProgress; }
		}

		TimeSpan IMediaElementRenderer.NaturalDuration
		{
			get
			{
				if (Control.NaturalDuration.HasTimeSpan)
				{
					return Control.NaturalDuration.TimeSpan;
				}

				return TimeSpan.Zero;
			}
		}

		int IMediaElementRenderer.NaturalVideoHeight
		{
			get	{ return Control.NaturalVideoHeight; }
		}

		int IMediaElementRenderer.NaturalVideoWidth
		{
			get { return Control.NaturalVideoWidth; }
		}

		TimeSpan IMediaElementRenderer.Position
		{
			get { return Control.Position; }
		}

		void IMediaElementRenderer.Seek(TimeSpan time)
		{
			Control.Position = time;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<MediaElement> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				if (Control != null)
				{
					if (_positionChangedToken != 0)
					{
						Control.UnregisterPropertyChangedCallback(Windows.UI.Xaml.Controls.MediaElement.PositionProperty, _positionChangedToken);
						_positionChangedToken = 0;
					}

					Control.CurrentStateChanged -= Control_CurrentStateChanged;
					Control.SeekCompleted -= Control_SeekCompleted;
					Control.MediaOpened -= Control_MediaOpened;
					Control.MediaEnded -= Control_MediaEnded;
				}

				e.OldElement.SetRenderer(null);
			}

			if (e.NewElement != null)
			{
				SetNativeControl(new Windows.UI.Xaml.Controls.MediaElement());
				Control.HorizontalAlignment = HorizontalAlignment.Stretch;
				Control.VerticalAlignment = VerticalAlignment.Stretch;

				e.NewElement.SetRenderer(this);
				Control.AreTransportControlsEnabled = Element.AreTransportControlsEnabled;
				Control.AutoPlay = Element.AutoPlay;
				Control.IsLooping = Element.IsLooping;
				Control.Stretch = Element.Aspect.ToStretch();

				_bufferingProgressChangedToken = Control.RegisterPropertyChangedCallback(Windows.UI.Xaml.Controls.MediaElement.BufferingProgressProperty, BufferingProgressChanged);
				_positionChangedToken = Control.RegisterPropertyChangedCallback(Windows.UI.Xaml.Controls.MediaElement.PositionProperty, PositionChanged);

				Control.SeekCompleted += Control_SeekCompleted;
				Control.CurrentStateChanged += Control_CurrentStateChanged;
				Control.MediaOpened += Control_MediaOpened;
				Control.MediaEnded += Control_MediaEnded;

				if (Element.Source != null)
				{
					Control.Source = Element.Source;
				}
			}
		}

		void Control_MediaEnded(object sender, RoutedEventArgs e)
		{
			Element?.OnMediaEnded();
		}

		void Control_MediaOpened(object sender, RoutedEventArgs e)
		{
			Element?.RaiseMediaOpened();
		}

	
		void Control_CurrentStateChanged(object sender, RoutedEventArgs e)
		{
			switch (Control.CurrentState)
			{
				case Windows.UI.Xaml.Media.MediaElementState.Playing:
					if (Element.KeepScreenOn)
					{
						_request.RequestActive();
					}
					break;

				case Windows.UI.Xaml.Media.MediaElementState.Paused:
				case Windows.UI.Xaml.Media.MediaElementState.Stopped:
				case Windows.UI.Xaml.Media.MediaElementState.Closed:
					if (Element.KeepScreenOn)
					{
						_request.RequestRelease();
					}
					break;
			}

			if (Element != null)
			{
				Element.SendCurrentState((MediaElementState)((int)Control.CurrentState));
			}
		}
		
		void BufferingProgressChanged(DependencyObject sender, DependencyProperty dp)
		{
			((IElementController)Element).SetValueFromRenderer(MediaElement.BufferingProgressProperty, Control.BufferingProgress);
		}

		void PositionChanged(DependencyObject sender, DependencyProperty dp)
		{
		}

		void Control_SeekCompleted(object sender, RoutedEventArgs e)
		{
			Element?.RaiseSeekCompleted();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(MediaElement.AreTransportControlsEnabled):
					Control.AreTransportControlsEnabled = Element.AreTransportControlsEnabled;
					break;
					
				case nameof(MediaElement.Aspect):
					Control.Stretch = Element.Aspect.ToStretch();
					break;

				case nameof(MediaElement.AutoPlay):
					Control.AutoPlay = Element.AutoPlay;
					break;

				case nameof(MediaElement.CurrentState):
					switch (Element.CurrentState)
					{
						case MediaElementState.Playing:
							Control.Play();
							break;

						case MediaElementState.Paused:
							Control.Pause();
							break;

						case MediaElementState.Stopped:
							Control.Stop();
							break;
					}
					break;

				case nameof(MediaElement.IsLooping):
					Control.IsLooping = Element.IsLooping;
					break;

				case nameof(MediaElement.KeepScreenOn):
					if (Element.KeepScreenOn)
					{
						if (Control.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
						{
							_request.RequestActive();
						}
					}
					else
					{
						_request.RequestRelease();
					}
					break;

				case nameof(MediaElement.Source):
					Control.Source = Element.Source;
					break;

				case nameof(MediaElement.Width):
					if (Element.Width > 0)
					{
						Width = Element.Width;
					}
					break;

				case nameof(MediaElement.Height):
					if (Element.Height > 0)
					{
						Height = Element.Height;
					}
					break;
			}

			base.OnElementPropertyChanged(sender, e);
		}
	}
}
