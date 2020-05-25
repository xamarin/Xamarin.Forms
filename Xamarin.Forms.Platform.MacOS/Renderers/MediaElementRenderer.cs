using System;
using AppKit;
using AVFoundation;
using AVKit;
using CoreMedia;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	public class MediaElementRenderer : ViewRenderer<MediaElement, NSView>
	{
		IMediaElementController Controller => Element;

		readonly AVPlayerView _avPlayerView = new AVPlayerView();
		NSObject _playedToEndObserver;
		NSObject _statusObserver;
		NSObject _rateObserver;
		AVPlayerLayer _playerLayer;

		[Internals.Preserve(Conditional = true)]
		public MediaElementRenderer()
		{
			MediaElement.VerifyMediaElementFlagEnabled(nameof(MediaElementRenderer));

			_playedToEndObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, PlayedToEnd);
		}

		void SetKeepScreenOn(bool value)
		{
			_avPlayerView.Player.PreventsDisplaySleepDuringVideoPlayback=value;
		}

		void UpdateSource()
		{
			if (Element.Source != null)
			{
				AVAsset asset = null;

				var uriSource = Element.Source as UriMediaSource;
				if (uriSource != null)
				{
					if (uriSource.Uri.Scheme == "ms-appx")
					{
						if (uriSource.Uri.LocalPath.Length <= 1)
							return;

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
					var fileSource = Element.Source as FileMediaSource;
					if (fileSource != null)
					{
						asset = AVAsset.FromUrl(NSUrl.FromFilename(fileSource.File));
					}
				}

				var item = new AVPlayerItem(asset);
				RemoveStatusObserver();

				_statusObserver = (NSObject)item.AddObserver("status", NSKeyValueObservingOptions.New, ObserveStatus);


				if (_avPlayerView.Player != null)
				{
					_avPlayerView.Player.ReplaceCurrentItemWithPlayerItem(item);
				}
				else
				{
					_avPlayerView.Player = new AVPlayer(item);
					_rateObserver = (NSObject)_avPlayerView.Player.AddObserver("rate", NSKeyValueObservingOptions.New, ObserveRate);
				}

				if (Element.AutoPlay)
					Play();
			}
			else
			{
				if (Element.CurrentState == MediaElementState.Playing || Element.CurrentState == MediaElementState.Buffering)
				{
					_avPlayerView.Player.Pause();
					Controller.CurrentState = MediaElementState.Stopped;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			Element.PropertyChanged -= OnElementPropertyChanged;
			Element.SeekRequested -= MediaElementSeekRequested;
			Element.StateRequested -= MediaElementStateRequested;
			Element.PositionRequested -= MediaElementPositionRequested;

			if (_playedToEndObserver != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(_playedToEndObserver);
				_playedToEndObserver = null;
			}

			if (_rateObserver != null)
			{
				_rateObserver.Dispose();
				_rateObserver = null;
			}

			RemoveStatusObserver();

			_avPlayerView?.Player?.Pause();
			_avPlayerView?.Player?.ReplaceCurrentItemWithPlayerItem(null);
			_playerLayer?.Dispose();
			_avPlayerView?.Dispose();
			

			base.Dispose(disposing);
		}

		void RemoveStatusObserver()
		{
			if (_statusObserver != null)
			{
				try
				{
					_avPlayerView?.Player?.CurrentItem?.RemoveObserver(_statusObserver, "status");
				}
				finally
				{

					_statusObserver = null;
				}
			}
		}

		void ObserveRate(NSObservedChange e)
		{
			if (Controller is object)
			{
				switch (_avPlayerView.Player.Rate)
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
		}

		void ObserveStatus(NSObservedChange e)
		{
			Controller.Volume = _avPlayerView.Player.Volume;

			switch (_avPlayerView.Player.Status)
			{
				case AVPlayerStatus.Failed:
					Controller.OnMediaFailed();
					break;

				case AVPlayerStatus.ReadyToPlay:
					var duration = _avPlayerView.Player.CurrentItem.Duration;

					if (duration.IsIndefinite)
						Controller.Duration = TimeSpan.Zero;
					else
						Controller.Duration = TimeSpan.FromSeconds(duration.Seconds);

					Controller.VideoHeight = (int)_avPlayerView.Player.CurrentItem.Asset.NaturalSize.Height;
					Controller.VideoWidth = (int)_avPlayerView.Player.CurrentItem.Asset.NaturalSize.Width;
					Controller.OnMediaOpened();
					Controller.Position = Position;
					break;
			}
		}

		TimeSpan Position
		{
			get
			{
				if (_avPlayerView.Player.CurrentTime.IsInvalid)
					return TimeSpan.Zero;

				return TimeSpan.FromSeconds(_avPlayerView.Player.CurrentTime.Seconds);
			}
		}

		void PlayedToEnd(NSNotification notification)
		{
			if (Element.IsLooping)
			{
				_avPlayerView.Player.Seek(CMTime.Zero);
				Controller.Position = Position;
				_avPlayerView.Player.Play();
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

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(MediaElement.Aspect):
					if (_playerLayer==null)
					{
						_playerLayer = AVPlayerLayer.FromPlayer(_avPlayerView.Player);
						_avPlayerView.Layer = _playerLayer;						
					}
					_playerLayer.VideoGravity = AspectToGravity(Element.Aspect);
					break;

				case nameof(MediaElement.KeepScreenOn):
					if (!Element.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}
					else if (Element.CurrentState == MediaElementState.Playing)
					{
						// only toggle this on if property is set while video is already running
						SetKeepScreenOn(true);
					}
					break;

				case nameof(MediaElement.ShowsPlaybackControls):
					_avPlayerView.ShowsFullScreenToggleButton = Element.ShowsPlaybackControls;
					break;

				case nameof(MediaElement.Source):
					UpdateSource();
					break;

				case nameof(MediaElement.Volume):
					if (Element.Volume > 0 && Element.Volume <1)
					{
						_avPlayerView.Player.Volume = Convert.ToSingle(Element.Volume);
					}					
					break;
			}
		}

		void MediaElementSeekRequested(object sender, SeekRequested e)
		{
			if (_avPlayerView.Player.Status != AVPlayerStatus.ReadyToPlay || _avPlayerView.Player.CurrentItem == null)
				return;

			NSValue[] ranges = _avPlayerView.Player.CurrentItem.SeekableTimeRanges;
			CMTime seekTo = new CMTime(Convert.ToInt64(e.Position.TotalMilliseconds), 1000);
			foreach (NSValue v in ranges)
			{
				if (seekTo >= v.CMTimeRangeValue.Start && seekTo < (v.CMTimeRangeValue.Start + v.CMTimeRangeValue.Duration))
				{
					_avPlayerView.Player.Seek(seekTo, SeekComplete);
					break;
				}
			}
		}

		void Play()
		{			

			if (_avPlayerView.Player != null)
			{
				_avPlayerView.Player.Play();
				Controller.CurrentState = MediaElementState.Playing;
			}

			if (Element.KeepScreenOn)
			{
				SetKeepScreenOn(true);
			}
		}

		void MediaElementStateRequested(object sender, StateRequested e)
		{			

			switch (e.State)
			{
				case MediaElementState.Playing:
					Play();
					break;

				case MediaElementState.Paused:
					if (Element.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}

					if (_avPlayerView.Player != null)
					{
						_avPlayerView.Player.Pause();
						Controller.CurrentState = MediaElementState.Paused;
					}
					break;

				case MediaElementState.Stopped:
					if (Element.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}					
					_avPlayerView?.Player.Pause();
					_avPlayerView?.Player.Seek(CMTime.Zero);
					Controller.CurrentState = MediaElementState.Stopped;

					
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

		void MediaElementPositionRequested(object sender, EventArgs e)
		{
			Controller.Position = Position;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<MediaElement> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
				e.OldElement.SeekRequested -= MediaElementSeekRequested;
				e.OldElement.StateRequested -= MediaElementStateRequested;
				e.OldElement.PositionRequested -= MediaElementPositionRequested;
				

				if (_playedToEndObserver != null)
				{
					NSNotificationCenter.DefaultCenter.RemoveObserver(_playedToEndObserver);
					_playedToEndObserver = null;
				}

				// stop video if playing
				if (_avPlayerView?.Player?.CurrentItem != null)
				{
					RemoveStatusObserver();

					_avPlayerView.Player.Pause();
					_avPlayerView.Player.Seek(CMTime.Zero);
					_avPlayerView.Player.ReplaceCurrentItemWithPlayerItem(null);					
				}
			}

			if (e.NewElement != null)
			{				
				SetNativeControl(_avPlayerView);

				Element.PropertyChanged += OnElementPropertyChanged;
				Element.SeekRequested += MediaElementSeekRequested;
				Element.StateRequested += MediaElementStateRequested;
				Element.PositionRequested += MediaElementPositionRequested;
				

				_avPlayerView.ShowsFullScreenToggleButton = Element.ShowsPlaybackControls;				
				
				if (Element.KeepScreenOn)
				{
					SetKeepScreenOn(true);
				}

				_playedToEndObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, PlayedToEnd);

				
				UpdateSource();
			}
		}		
	}
}
