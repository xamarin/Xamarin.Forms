﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	public sealed class MediaElementRenderer : ViewRenderer<MediaElement, System.Windows.Controls.MediaElement>
	{		
		protected override void OnElementChanged(ElementChangedEventArgs<MediaElement> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				Element.SeekRequested -= Element_SeekRequested;
				Element.StateRequested -= Element_StateRequested;
				Element.PositionRequested -= Element_PositionRequested;

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
				Element.PositionRequested += Element_PositionRequested;
				UpdateSource();
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
		MediaElementState _requestedState;

		void Element_StateRequested(object sender, StateRequested e)
		{
			_requestedState = e.State;

			switch(e.State)
			{
				case MediaElementState.Playing:
					if (Element.KeepScreenOn)
					{
						DisplayRequestActive();
					}

					Control.Play();
					Controller.CurrentState = _requestedState;
					break;

				case MediaElementState.Paused:
					if (Control.CanPause)
					{
						if (Element.KeepScreenOn)
						{
							DisplayRequestRelease();
						}

						Control.Pause();
						Controller.CurrentState = _requestedState;
					}
					break;

				case MediaElementState.Stopped:
					if (Element.KeepScreenOn)
					{
						DisplayRequestRelease();
					}

					Control.Stop();
					Controller.CurrentState = _requestedState;
					break;
			}

			Controller.Position = Control.Position;
		}

		private void Element_SeekRequested(object sender, SeekRequested e)
		{
			Control.Position = e.Position;
			Controller.Position = Control.Position;
		}

		void UpdateSource()
		{
			if (Element.Source == null)
				return;


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

			Controller.CurrentState = MediaElementState.Opening;
		}

		void Control_BufferingEnded(object sender, RoutedEventArgs e)
		{
			Controller.BufferingProgress = 1.0;
			if (Element.AutoPlay)
			{
				Controller.CurrentState = MediaElementState.Playing;
			}
			else
			{
				Controller.CurrentState = _requestedState; ;
			}
		}

		void Control_BufferingStarted(object sender, RoutedEventArgs e)
		{
			Controller.BufferingProgress = 0.0;
			Controller.CurrentState = MediaElementState.Buffering;
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
				_requestedState = MediaElementState.Stopped;
				Controller.CurrentState = MediaElementState.Stopped;
				Element.OnMediaEnded();
			}

			Controller.Position = Control.Position;
		}

		void Control_MediaOpened(object sender, RoutedEventArgs e)
		{
			Controller.Duration = Control.NaturalDuration.HasTimeSpan ? Control.NaturalDuration.TimeSpan : (TimeSpan?)null;
			Controller.VideoHeight = Control.NaturalVideoHeight;
			Controller.VideoWidth = Control.NaturalVideoWidth;

			Element?.RaiseMediaOpened();

			if(Element.AutoPlay)
			{
				Control.Play();
				_requestedState = MediaElementState.Playing;
				Controller.CurrentState = MediaElementState.Playing;
			}
			else
			{
				Controller.CurrentState = _requestedState;
			}
		}
		
		void Control_SeekCompleted(object sender, RoutedEventArgs e)
		{
			Controller.Position = Control.Position;
			Element?.RaiseSeekCompleted();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(MediaElement.Aspect):
					Control.Stretch = Element.Aspect.ToStretch();
					break;
					
				case nameof(MediaElement.KeepScreenOn):
					if (Element.KeepScreenOn)
					{
						if (Element.CurrentState == MediaElementState.Playing)
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

		protected override void UpdateWidth()
		{
			Control.Width = Math.Max(0, Element.Width);
		}

		protected override void UpdateHeight()
		{
			Control.Height = Math.Max(0, Element.Height);
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
