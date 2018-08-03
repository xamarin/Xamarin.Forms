using System;
using System.Windows;
using System.Windows.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	public sealed class MediaElementRenderer : ViewRenderer<MediaElement, System.Windows.Controls.MediaElement>, IMediaElementRenderer
	{
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
			get { return Control.NaturalVideoHeight; }
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
					Control.BufferingStarted -= Control_BufferingStarted;
					Control.BufferingEnded -= Control_BufferingEnded;
					Control.MediaOpened -= Control_MediaOpened;
					Control.MediaEnded -= Control_MediaEnded;
					Control.MediaFailed -= Control_MediaFailed;
				}

				e.OldElement.SetRenderer(null);
			}

			if (e.NewElement != null)
			{
				SetNativeControl(new System.Windows.Controls.MediaElement());
				Control.HorizontalAlignment = HorizontalAlignment.Stretch;
				Control.VerticalAlignment = VerticalAlignment.Stretch;

				e.NewElement.SetRenderer(this);
				Control.LoadedBehavior = Element.AutoPlay ? MediaState.Play : MediaState.Manual;
				Control.UnloadedBehavior = MediaState.Close;
				Control.Stretch = Element.Aspect.ToStretch();
				
				Control.BufferingStarted += Control_BufferingStarted;
				Control.BufferingEnded += Control_BufferingEnded;
				Control.MediaOpened += Control_MediaOpened;
				Control.MediaEnded += Control_MediaEnded;
				Control.MediaFailed += Control_MediaFailed;

				UpdateSource();
			}
		}

		void UpdateSource()
		{
			if (Element.Source != null)
			{
				if(Element.Source.Scheme == "ms-appx")
				{
					Control.Source = new Uri(Element.Source.ToString().Replace("ms-appx://", "pack://application:,,,"));
				}
				else
				{
					Control.Source = Element.Source;
				}
			}
		}

		void Control_BufferingEnded(object sender, RoutedEventArgs e)
		{
			Element.SetValueFromRenderer(MediaElement.BufferingProgressProperty, 1.0);
		}

		void Control_BufferingStarted(object sender, RoutedEventArgs e)
		{
			Element.SetValueFromRenderer(MediaElement.BufferingProgressProperty, 0);
		}

		void Control_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(e.ErrorException);
		}

		void Control_MediaEnded(object sender, RoutedEventArgs e)
		{
			if(Element.IsLooping)
			{
				// restart media
				Control.Position = TimeSpan.Zero;
				Element.Play();
			}
		}

		void Control_MediaOpened(object sender, RoutedEventArgs e)
		{
			Element?.RaiseMediaOpened();
		}

		void BufferingProgressChanged(DependencyObject sender, DependencyProperty dp)
		{
			((IElementController)Element).SetValueFromRenderer(MediaElement.BufferingProgressProperty, Control.BufferingProgress);
		}
		
		void Control_SeekCompleted(object sender, RoutedEventArgs e)
		{
			Element?.RaiseSeekCompleted();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				//case nameof(MediaElement.AreTransportControlsEnabled):
				//	Control.AreTransportControlsEnabled = Element.AreTransportControlsEnabled;
				//	break;

				case nameof(MediaElement.Aspect):
					Control.Stretch = Element.Aspect.ToStretch();
					break;

				case nameof(MediaElement.AutoPlay):
					Control.LoadedBehavior = Element.AutoPlay ? MediaState.Play : MediaState.Manual;
					break;

				case nameof(MediaElement.CurrentState):
					switch (Element.CurrentState)
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
					break;
					
				/*case nameof(MediaElement.KeepScreenOn):
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
					break;*/

				case nameof(MediaElement.Source):
					Control.Source = Element.Source;
					break;
			}

			base.OnElementPropertyChanged(sender, e);
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return base.GetDesiredSize(Math.Max(240,widthConstraint), Math.Max(180,heightConstraint));
		}

		protected override void UpdateWidth()
		{
			if (Element.Width > 0)
			{
				Control.Width = Element.Width;
			}
		}

		protected override void UpdateHeight()
		{
			if (Element.Height > 0)
			{
				Control.Height = Element.Height;
			}
		}
	}
}
