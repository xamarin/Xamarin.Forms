using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public sealed class MediaElementRenderer : ViewRenderer<MediaElement, Windows.UI.Xaml.Controls.MediaElement>
	{
		Windows.System.Display.DisplayRequest _request = new Windows.System.Display.DisplayRequest();

		long _bufferingProgressChangedToken;

		long _positionChangedToken;

		void ReleaseControl()
		{
			if (Control != null)
			{
				if (_bufferingProgressChangedToken != 0)
				{
					Control.UnregisterPropertyChangedCallback(Windows.UI.Xaml.Controls.MediaElement.BufferingProgressProperty, _bufferingProgressChangedToken);
					_bufferingProgressChangedToken = 0;
				}

				if (_positionChangedToken != 0)
				{
					Control.UnregisterPropertyChangedCallback(Windows.UI.Xaml.Controls.MediaElement.PositionProperty, _positionChangedToken);
					_positionChangedToken = 0;
				}

				Element.SeekRequested -= Element_SeekRequested;
				Element.StateRequested -= Element_StateRequested;
				Element.PositionRequested -= Element_PositionRequested;

				Control.CurrentStateChanged -= Control_CurrentStateChanged;
				Control.SeekCompleted -= Control_SeekCompleted;
				Control.MediaOpened -= Control_MediaOpened;
				Control.MediaEnded -= Control_MediaEnded;
				Control.MediaFailed -= Control_MediaFailed;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			ReleaseControl();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<MediaElement> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				ReleaseControl();
			}

			if (e.NewElement != null)
			{
				SetNativeControl(new Windows.UI.Xaml.Controls.MediaElement());
				Control.HorizontalAlignment = HorizontalAlignment.Stretch;
				Control.VerticalAlignment = VerticalAlignment.Stretch;

				Control.AreTransportControlsEnabled = Element.ShowsPlaybackControls;
				Control.AutoPlay = Element.AutoPlay;
				Control.IsLooping = Element.IsLooping;
				Control.Stretch = Element.Aspect.ToStretch();

				_bufferingProgressChangedToken = Control.RegisterPropertyChangedCallback(Windows.UI.Xaml.Controls.MediaElement.BufferingProgressProperty, BufferingProgressChanged);
				_positionChangedToken = Control.RegisterPropertyChangedCallback(Windows.UI.Xaml.Controls.MediaElement.PositionProperty, PositionChanged);

				Element.SeekRequested += Element_SeekRequested;
				Element.StateRequested += Element_StateRequested;
				Element.PositionRequested += Element_PositionRequested;
				Control.SeekCompleted += Control_SeekCompleted;
				Control.CurrentStateChanged += Control_CurrentStateChanged;
				Control.MediaOpened += Control_MediaOpened;
				Control.MediaEnded += Control_MediaEnded;
				Control.MediaFailed += Control_MediaFailed;

				if (Element.Source != null)
				{
					Control.Source = Element.Source;
				}
			}
		}

		void Element_PositionRequested(object sender, EventArgs e)
		{
			if (Control != null)
			{
				Controller.Position = Control.Position;
			}
		}

		IMediaElementController Controller => Element as IMediaElementController;

		private void Element_StateRequested(object sender, StateRequested e)
		{
			switch (e.State)
			{
				case MediaElementState.Playing:
					Control.Play();
					break;

				case MediaElementState.Paused:
					if (Control.CanPause)
					{
						Control.Pause();
					}
					break;

				case MediaElementState.Stopped:
					Control.Stop();
					break;
			}

			Controller.Position = Control.Position;
		}

		private void Element_SeekRequested(object sender, SeekRequested e)
		{
			if (Control.CanSeek)
			{
				Control.Position = e.Position;
				Controller.Position = Control.Position;
			}
		}

		void Control_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			Element?.OnMediaFailed();
		}

		void Control_MediaEnded(object sender, RoutedEventArgs e)
		{
			Controller.Position = Control.Position;
			Controller.CurrentState = MediaElementState.Stopped;
			Element?.OnMediaEnded();
		}

		void Control_MediaOpened(object sender, RoutedEventArgs e)
		{
			Controller.Duration = Control.NaturalDuration.HasTimeSpan ? Control.NaturalDuration.TimeSpan : (TimeSpan?)null;
			Controller.VideoHeight = Control.NaturalVideoHeight;
			Controller.VideoWidth = Control.NaturalVideoWidth;

			Element?.RaiseMediaOpened();
		}

	
		void Control_CurrentStateChanged(object sender, RoutedEventArgs e)
		{
			if (Element == null)
				return;

			switch (((Windows.UI.Xaml.Controls.MediaElement)sender).CurrentState)
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

			Controller.CurrentState = FromWindowsMediaElementState(Control.CurrentState);
		}

		static MediaElementState FromWindowsMediaElementState(Windows.UI.Xaml.Media.MediaElementState state)
		{
			switch(state)
			{
				case Windows.UI.Xaml.Media.MediaElementState.Buffering:
					return MediaElementState.Buffering;

				case Windows.UI.Xaml.Media.MediaElementState.Closed:
					return MediaElementState.Closed;

				case Windows.UI.Xaml.Media.MediaElementState.Opening:
					return MediaElementState.Opening;

				case Windows.UI.Xaml.Media.MediaElementState.Paused:
					return MediaElementState.Paused;

				case Windows.UI.Xaml.Media.MediaElementState.Playing:
					return MediaElementState.Playing;

				case Windows.UI.Xaml.Media.MediaElementState.Stopped:
					return MediaElementState.Stopped;
			}

			throw new ArgumentOutOfRangeException();
		}
		
		void BufferingProgressChanged(DependencyObject sender, DependencyProperty dp)
		{
			Controller.BufferingProgress = ((Windows.UI.Xaml.Controls.MediaElement)sender).BufferingProgress;
		}

		void PositionChanged(DependencyObject sender, DependencyProperty dp)
		{
			Controller.Position = ((Windows.UI.Xaml.Controls.MediaElement)sender).Position;
		}

		void Control_SeekCompleted(object sender, RoutedEventArgs e)
		{
			Controller.Position = ((Windows.UI.Xaml.Controls.MediaElement)sender).Position;
			Element?.RaiseSeekCompleted();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{	
				case nameof(MediaElement.Aspect):
					Control.Stretch = Element.Aspect.ToStretch();
					break;

				case nameof(MediaElement.AutoPlay):
					Control.AutoPlay = Element.AutoPlay;
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

				case nameof(MediaElement.ShowsPlaybackControls):
					Control.AreTransportControlsEnabled = Element.ShowsPlaybackControls;
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
