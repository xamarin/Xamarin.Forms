using AVFoundation;
using AVKit;
using CoreGraphics;
using CoreMedia;
using Foundation;
using System;
using System.IO;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	public sealed class MediaElementRenderer : AVPlayerViewController, IVisualElementRenderer, IEffectControlProvider
	{
		MediaElement MediaElement { get; set; }
		IMediaElementController Controller => MediaElement as IMediaElementController;

#pragma warning disable 0414
		VisualElementTracker _tracker;
#pragma warning restore 0414
		
		NSObject _playToEndObserver;
		NSObject _statusObserver;
		NSObject _rateObserver;

		VisualElement IVisualElementRenderer.Element => MediaElement;

		UIView IVisualElementRenderer.NativeView => View;

		UIViewController IVisualElementRenderer.ViewController => this;
		
		bool _idleTimerDisabled = false;

		public MediaElementRenderer()
		{
			Xamarin.Forms.MediaElement.VerifyMediaElementFlagEnabled(nameof(MediaElementRenderer));

			_playToEndObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, PlayedToEnd);
			View.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
		}

		public override void ViewDidLayoutSubviews()
		{
			MediaElement.Layout(View.Frame.ToRectangle());
		}

		void SetKeepScreenOn(bool value)
		{
			if (value)
			{
				if (!UIApplication.SharedApplication.IdleTimerDisabled)
				{
					_idleTimerDisabled = true;
					UIApplication.SharedApplication.IdleTimerDisabled = true;
				}
			}
			else if (_idleTimerDisabled)
			{
				_idleTimerDisabled = false;
				UIApplication.SharedApplication.IdleTimerDisabled = false;
			}
		}

		void UpdateSource()
		{
			if (MediaElement.Source != null)
			{
				AVAsset asset = null;
				
				var uriSource = MediaElement.Source as UriMediaSource;
				if (uriSource != null)
				{
					if (uriSource.Uri.Scheme == "ms-appx")
					{
						// used for a file embedded in the application package
						asset = AVAsset.FromUrl(NSUrl.FromFilename(uriSource.Uri.LocalPath.Substring(1)));
					}
					else if (uriSource.Uri.Scheme == "ms-appdata")
					{
						string filePath = Platform.ResolveMsAppDataUri(uriSource.Uri);

						if (string.IsNullOrEmpty(filePath))
							throw new ArgumentException("Invalid Uri", "Source");

						asset = AVAsset.FromUrl(NSUrl.FromFilename(filePath));
					}
					else
					{
						asset = AVUrlAsset.Create(NSUrl.FromString(uriSource.Uri.AbsoluteUri));
					}
				}
				else
				{
					var fileSource = MediaElement.Source as FileMediaSource;
					if (fileSource != null)
					{
						asset = AVAsset.FromUrl(NSUrl.FromFilename(fileSource.File));
					}
				}

				
				AVPlayerItem item = new AVPlayerItem(asset);
				RemoveStatusObserver();

				_statusObserver = (NSObject)item.AddObserver("status", NSKeyValueObservingOptions.New, ObserveStatus);
				
				
				if (Player != null)
				{
					Player.ReplaceCurrentItemWithPlayerItem(item);
				}
				else
				{
					Player = new AVPlayer(item);
					_rateObserver = (NSObject)Player.AddObserver("rate", NSKeyValueObservingOptions.New, ObserveRate);
				}
				
				if (MediaElement.AutoPlay)
					Play();
			}
			else
			{
				if (MediaElement.CurrentState == MediaElementState.Playing || MediaElement.CurrentState == MediaElementState.Buffering)
				{
					Player.Pause();
					Controller.CurrentState = MediaElementState.Stopped;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(_playToEndObserver != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(_playToEndObserver);
				_playToEndObserver = null;
			}

			if(_rateObserver != null)
			{
				Player.RemoveObserver(_rateObserver, "rate");
				_rateObserver = null;
			}

			RemoveStatusObserver();

			Player?.Pause();
			Player?.ReplaceCurrentItemWithPlayerItem(null);

			base.Dispose(disposing);
		}

		void RemoveStatusObserver()
		{
			if (_statusObserver != null)
			{
				try
				{
					Player?.CurrentItem?.RemoveObserver(_statusObserver, "status");
				}
				finally
				{

					_statusObserver = null;
				}
			}
		}

		void ObserveRate(NSObservedChange e)
		{
			switch(Player.Rate)
			{
				case 0.0f:
					Controller.CurrentState = MediaElementState.Paused;
					break;

				case 1.0f:
					Controller.CurrentState = MediaElementState.Playing;
					break;
			}

			Controller.Position = Position;
		}

		void ObserveStatus(NSObservedChange e)
		{
			Controller.Volume = Player.Volume;

			switch (Player.Status)
			{
				case AVPlayerStatus.Failed:
					Controller.OnMediaFailed();
					break;

				case AVPlayerStatus.ReadyToPlay:
					Controller.Duration = TimeSpan.FromSeconds(Player.CurrentItem.Duration.Seconds);
					Controller.VideoHeight = (int)Player.CurrentItem.Asset.NaturalSize.Height;
					Controller.VideoWidth = (int)Player.CurrentItem.Asset.NaturalSize.Width;
					Controller.OnMediaOpened();
					Controller.Position = Position;
					break;
			}
		}

		TimeSpan Position
		{
			get
			{
				if (Player.CurrentTime.IsInvalid)
					return TimeSpan.Zero;

				return TimeSpan.FromSeconds(Player.CurrentTime.Seconds);
			}
		}

		void PlayedToEnd(NSNotification notification)
		{
			if (MediaElement.IsLooping)
			{
				Player.Seek(CMTime.Zero);
				Controller.Position = Position;
				Player.Play();
			}
			else
			{
				SetKeepScreenOn(false);
				Controller.Position = Position;

				try
				{
					Device.BeginInvokeOnMainThread(Controller.OnMediaEnded);
				}
				catch { }
			}
		}
		
		void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(MediaElement.Aspect):
					VideoGravity = AspectToGravity(MediaElement.Aspect);
					break;

				case nameof(MediaElement.KeepScreenOn):
					if (!MediaElement.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}
					else if(MediaElement.CurrentState == MediaElementState.Playing)
					{
						// only toggle this on if property is set while video is already running
						SetKeepScreenOn(true);
					}
					break;

				case nameof(MediaElement.ShowsPlaybackControls):
					ShowsPlaybackControls = MediaElement.ShowsPlaybackControls;
					break;

				case nameof(MediaElement.Source):
					UpdateSource();
					break;

				case nameof(MediaElement.Volume):
					Player.Volume = (float)MediaElement.Volume;
					break;
			}
		}

		void MediaElementSeekRequested(object sender, SeekRequested e)
		{
			if (Player.Status != AVPlayerStatus.ReadyToPlay || Player.CurrentItem == null)
				return;

			NSValue[] ranges = Player.CurrentItem.SeekableTimeRanges;
			CMTime seekTo = new CMTime(Convert.ToInt64(e.Position.TotalMilliseconds), 1000);
			foreach (NSValue v in ranges)
			{
				if (seekTo >= v.CMTimeRangeValue.Start && seekTo < (v.CMTimeRangeValue.Start + v.CMTimeRangeValue.Duration))
				{
					Player.Seek(seekTo, SeekComplete);
					break;
				}
			}
		}

		void Play()
		{
			var audioSession = AVAudioSession.SharedInstance();
			NSError err = audioSession.SetCategory(AVAudioSession.CategoryPlayback);
			if (!(err is null))
				Log.Warning("MediaElement", "Failed to set AVAudioSession Category {0}", err.Code);

			audioSession.SetMode(AVAudioSession.ModeMoviePlayback, out err);
			if (!(err is null))
				Log.Warning("MediaElement", "Failed to set AVAudioSession Mode {0}", err.Code);
			err = audioSession.SetActive(true);
			if (!(err is null))
				Log.Warning("MediaElement", "Failed to set AVAudioSession Active {0}", err.Code);

			Player.Play();
			Controller.CurrentState = MediaElementState.Playing;
			if (MediaElement.KeepScreenOn)
			{
				SetKeepScreenOn(true);
			}
		}

		void MediaElementStateRequested(object sender, StateRequested e)
		{
			MediaElementVolumeRequested(this, EventArgs.Empty);

			switch (e.State)
			{
				case MediaElementState.Playing:
					Play();
					break;

				case MediaElementState.Paused:
					if (MediaElement.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}
					Player.Pause();
					Controller.CurrentState = MediaElementState.Paused;
					break;

				case MediaElementState.Stopped:
					if (MediaElement.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}
					//ios has no stop...
					Player.Pause();
					Player.Seek(CMTime.Zero);
					Controller.CurrentState = MediaElementState.Stopped;

					NSError err = AVAudioSession.SharedInstance().SetActive(false);
					if (!(err is null))
						Log.Warning("MediaElement", "Failed to set AVAudioSession Inactive {0}", err.Code);
					break;
			}

			Controller.Position = Position;
		}

		static AVLayerVideoGravity AspectToGravity(Aspect aspect)
		{
			switch (aspect)
			{
				case Aspect.Fill:
					return AVLayerVideoGravity.Resize;

				case Aspect.AspectFill:
					return AVLayerVideoGravity.ResizeAspectFill;

				default:
					return AVLayerVideoGravity.ResizeAspect;
			}
		}

		void SeekComplete(bool finished)
		{
			if (finished)
			{
				Controller.OnSeekCompleted();
			}
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return View.GetSizeRequest(widthConstraint, heightConstraint, 44, 44);
		}
		
		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (!(element is MediaElement))
			{
				throw new ArgumentException($"{nameof(element)} must be of type {nameof(MediaElement)}");
			}

			MediaElement oldElement = MediaElement;
			MediaElement = (MediaElement)element;

			Performance.Start(out string reference);

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
				oldElement.SeekRequested -= MediaElementSeekRequested;
				oldElement.StateRequested -= MediaElementStateRequested;
				oldElement.PositionRequested -= MediaElementPositionRequested;
				oldElement.VolumeRequested -= MediaElementVolumeRequested;
			}

			Color currentColor = oldElement?.BackgroundColor ?? Color.Default;
			if (element.BackgroundColor != currentColor)
			{
				UpdateBackgroundColor();
			}

			MediaElement.PropertyChanged += OnElementPropertyChanged;
			MediaElement.SeekRequested += MediaElementSeekRequested;
			MediaElement.StateRequested += MediaElementStateRequested;
			MediaElement.PositionRequested += MediaElementPositionRequested;
			MediaElement.VolumeRequested += MediaElementVolumeRequested;
			
			_tracker = new VisualElementTracker(this);

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, MediaElement));

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);

			Performance.Stop(reference);
		}

		private void MediaElementVolumeRequested(object sender, EventArgs e)
		{
			Controller.Volume = Player.Volume;
		}

		void MediaElementPositionRequested(object sender, EventArgs e)
		{
			Controller.Position = Position;
		}

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		void OnElementChanged(VisualElementChangedEventArgs e)
		{
			ElementChanged?.Invoke(this, e);
		}

		void IVisualElementRenderer.SetElementSize(Size size)
		{
			Layout.LayoutChildIntoBoundingRegion(MediaElement, new Rectangle(MediaElement.X, MediaElement.Y, size.Width, size.Height));
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			VisualElementRenderer<VisualElement>.RegisterEffect(effect, View);
		}

		void UpdateBackgroundColor()
		{
			View.BackgroundColor = MediaElement.BackgroundColor.ToUIColor();
		}
	}
}