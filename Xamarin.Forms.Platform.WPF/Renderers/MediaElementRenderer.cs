using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Xamarin.Forms.Platform.WPF
{
	public sealed class MediaElementRenderer : ViewRenderer<MediaElement, System.Windows.Controls.MediaElement>
	{
		long _bufferingProgressChangedToken;
		long _positionChangedToken;
		
		protected override void OnElementChanged(ElementChangedEventArgs<MediaElement> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				Element.SeekRequested -= Element_SeekRequested;
				Element.StateRequested -= Element_StateRequested;

				if (Control != null)
				{
					Control.BufferingStarted -= Control_BufferingStarted;
					Control.BufferingEnded -= Control_BufferingEnded;
					Control.MediaOpened -= Control_MediaOpened;
					Control.MediaEnded -= Control_MediaEnded;
					Control.MediaFailed -= Control_MediaFailed;
				}
				
			}

			if (e.NewElement != null)
			{
				SetNativeControl(new System.Windows.Controls.MediaElement());
				Control.HorizontalAlignment = HorizontalAlignment.Stretch;
				Control.VerticalAlignment = VerticalAlignment.Stretch;
				
				Control.LoadedBehavior = MediaState.Manual;
				Control.UnloadedBehavior = MediaState.Close;
				Control.Stretch = Element.Aspect.ToStretch();
				
				Control.BufferingStarted += Control_BufferingStarted;
				Control.BufferingEnded += Control_BufferingEnded;
				Control.MediaOpened += Control_MediaOpened;
				Control.MediaEnded += Control_MediaEnded;
				Control.MediaFailed += Control_MediaFailed;

				Element.SeekRequested += Element_SeekRequested;
				Element.StateRequested += Element_StateRequested;
				UpdateSource();
			}
		}

		private void Element_StateRequested(object sender, StateRequested e)
		{
			switch(e.State)
			{
				case MediaElementState.Playing:
					if (Element.KeepScreenOn)
					{
						DisplayRequestActive();
					}

					Control.Play();
					((IMediaElementController)Element).CurrentState = MediaElementState.Playing;
					break;

				case MediaElementState.Paused:
					if (Control.CanPause)
					{
						if (Element.KeepScreenOn)
						{
							DisplayRequestRelease();
						}

						Control.Pause();
						((IMediaElementController)Element).CurrentState = MediaElementState.Paused;
					}
					break;

				case MediaElementState.Stopped:
					if (Element.KeepScreenOn)
					{
						DisplayRequestRelease();
					}

					Control.Stop();
					((IMediaElementController)Element).CurrentState = MediaElementState.Stopped;
					break;
			}

			((IMediaElementController)Element).Position = Control.Position;
		}

		private void Element_SeekRequested(object sender, SeekRequested e)
		{
			Control.Position = e.Position;
			((IMediaElementController)Element).Position = Control.Position;
		}

		void UpdateSource()
		{
			if (Element.Source != null)
			{
				if (Control.Clock != null)
					Control.Clock = null;

				if (Element.Source.Scheme == "ms-appx")
				{
					Control.Source = new Uri(Element.Source.ToString().Replace("ms-appx://", "pack://application:,,,"));
				}
				else if (Element.Source.Scheme == "ms-appdata")
				{
					string filePath = string.Empty;

					if (Element.Source.LocalPath.StartsWith("/local"))
					{
						// WPF doesn't have the concept of an app package local folder so using My Documents as a placeholder
						filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Element.Source.LocalPath.Substring(7));
					}
					else if (Element.Source.LocalPath.StartsWith("/temp"))
					{
						filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Element.Source.LocalPath.Substring(6));
					}
					else
					{
						throw new ArgumentException("Invalid Uri", "Source");
					}

					Control.Source = new Uri(filePath);
				}
				else if (Element.Source.Scheme == "https")
				{
					throw new ArgumentException("HTTPS Not supported", "Source");
				}
				else
				{
					Control.Source = Element.Source;
				}

				((IMediaElementController)Element).CurrentState = MediaElementState.Opening;
			}
		}

		void Control_BufferingEnded(object sender, RoutedEventArgs e)
		{
			Element.SetValueFromRenderer(MediaElement.BufferingProgressProperty, 1.0);
			if (Control.Clock != null && Control.Clock.CurrentState == System.Windows.Media.Animation.ClockState.Active)
			{
				((IMediaElementController)Element).CurrentState = MediaElementState.Playing;
			}
			else
			{
				((IMediaElementController)Element).CurrentState = MediaElementState.Paused;
			}
		}

		void Control_BufferingStarted(object sender, RoutedEventArgs e)
		{
			Element.SetValueFromRenderer(MediaElement.BufferingProgressProperty, 0);
			((IMediaElementController)Element).CurrentState = MediaElementState.Buffering;
		}

		void Control_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			Element.OnMediaFailed();
		}

		void Control_MediaEnded(object sender, RoutedEventArgs e)
		{
			if(Element.IsLooping)
			{
				// restart media
				Control.Position = TimeSpan.Zero;
				Control.Play();
			}
			else
			{
				((IMediaElementController)Element).CurrentState = MediaElementState.Stopped;
			}
		}

		void Control_MediaOpened(object sender, RoutedEventArgs e)
		{
			((IMediaElementController)Element).Duration = Control.NaturalDuration.HasTimeSpan ? Control.NaturalDuration.TimeSpan : (TimeSpan?)null;
			((IMediaElementController)Element).VideoHeight = Control.NaturalVideoHeight;
			((IMediaElementController)Element).VideoWidth = Control.NaturalVideoWidth;

			Element?.RaiseMediaOpened();
			if(Element.AutoPlay)
			{
				Control.Play();
				((IMediaElementController)Element).CurrentState = MediaElementState.Playing;
			}
		}

		void BufferingProgressChanged(DependencyObject sender, DependencyProperty dp)
		{
			((IMediaElementController)Element).BufferingProgress = Control.BufferingProgress;
		}
		
		void Control_SeekCompleted(object sender, RoutedEventArgs e)
		{
			Element?.RaiseSeekCompleted();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				//TODO: AreTransportControlsEnabled not supported for WPF

				case nameof(MediaElement.Aspect):
					Control.Stretch = Element.Aspect.ToStretch();
					break;
					
				case nameof(MediaElement.KeepScreenOn):
					if (Element.KeepScreenOn)
					{
						if (Control.Clock != null && Control.Clock.CurrentState == System.Windows.Media.Animation.ClockState.Active)
						{
							DisplayRequestActive();
						}
					}
					else
					{
						DisplayRequestRelease();
					}
					break;

				case nameof(MediaElement.Source):
					UpdateSource();
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

		void DisplayRequestActive()
		{
			NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.DISPLAY_REQUIRED | NativeMethods.EXECUTION_STATE.CONTINUOUS);
		}

		void DisplayRequestRelease()
		{
			NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.CONTINUOUS);
		}

		static class NativeMethods
		{
			[DllImport("Kernel32", SetLastError = true)]
			internal static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

			internal enum EXECUTION_STATE : uint
			{
				/// <summary>
				/// Informs the system that the state being set should remain in effect until the next call that uses ES_CONTINUOUS and one of the other state flags is cleared.
				/// </summary>
				CONTINUOUS = 0x80000000,

				/// <summary>
				/// Forces the display to be on by resetting the display idle timer.
				/// </summary>
				DISPLAY_REQUIRED = 0x00000002,
			}
		}
	}
}
