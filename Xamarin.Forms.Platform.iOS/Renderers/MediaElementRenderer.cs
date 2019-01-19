using AVFoundation;
using AVKit;
using CoreMedia;
using Foundation;
using System;
using System.IO;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	public sealed class MediaElementRenderer : UIView, IVisualElementRenderer, IEffectControlProvider
	{
		MediaElement MediaElement { get; set; }
		IMediaElementController Controller => MediaElement as IMediaElementController;

#pragma warning disable 0414
		VisualElementTracker _tracker;
#pragma warning restore 0414

		AVPlayerViewController _avPlayerViewController = new AVPlayerViewController();

		NSObject _playToEndObserver;
		NSObject _statusObserver;
		NSObject _rateObserver;

		VisualElement IVisualElementRenderer.Element => MediaElement;

		UIView IVisualElementRenderer.NativeView => this;

		UIViewController IVisualElementRenderer.ViewController => _avPlayerViewController;
		
		bool _idleTimerDisabled = false;

		public MediaElementRenderer()
		{
			_playToEndObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, PlayedToEnd);
			_avPlayerViewController.View.Frame = Bounds;
			_avPlayerViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			AddSubview(_avPlayerViewController.View);
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
						string filePath = string.Empty;

						if (uriSource.Uri.LocalPath.StartsWith("/local"))
						{
							var libraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
							filePath = Path.Combine(libraryPath, uriSource.Uri.LocalPath.Substring(7));
						}
						else if (uriSource.Uri.LocalPath.StartsWith("/temp"))
						{
							filePath = Path.Combine(Path.GetTempPath(), uriSource.Uri.LocalPath.Substring(6));
						}
						else
						{
							throw new ArgumentException("Invalid Uri", "Source");
						}

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
				
				
				if (_avPlayerViewController.Player != null)
				{
					_avPlayerViewController.Player.ReplaceCurrentItemWithPlayerItem(item);
				}
				else
				{
					_avPlayerViewController.Player = new AVPlayer(item);
					_rateObserver = (NSObject)_avPlayerViewController.Player.AddObserver("rate", NSKeyValueObservingOptions.New, ObserveRate);
				}
				
				if (MediaElement.AutoPlay)
					Play();
			}
			else
			{
				if (MediaElement.CurrentState == MediaElementState.Playing || MediaElement.CurrentState == MediaElementState.Buffering)
				{
					_avPlayerViewController.Player.Pause();
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
				_avPlayerViewController?.Player.RemoveObserver(_rateObserver, "rate");
				_rateObserver = null;
			}

			RemoveStatusObserver();

			_avPlayerViewController?.Player?.Pause();
			_avPlayerViewController?.Player?.ReplaceCurrentItemWithPlayerItem(null);

			base.Dispose(disposing);
		}

		void RemoveStatusObserver()
		{
			if (_statusObserver != null)
			{
				try
				{
					_avPlayerViewController?.Player?.CurrentItem?.RemoveObserver(_statusObserver, "status");
				}
				finally
				{

					_statusObserver = null;
				}
			}
		}

		void ObserveRate(NSObservedChange e)
		{
			switch(_avPlayerViewController.Player.Rate)
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
			switch (_avPlayerViewController.Player.Status)
			{
				case AVPlayerStatus.Failed:
					Controller.OnMediaFailed();
					break;

				case AVPlayerStatus.ReadyToPlay:
					Controller.Duration = TimeSpan.FromSeconds(_avPlayerViewController.Player.CurrentItem.Duration.Seconds);
					Controller.VideoHeight = (int)_avPlayerViewController.Player.CurrentItem.Asset.NaturalSize.Height;
					Controller.VideoWidth = (int)_avPlayerViewController.Player.CurrentItem.Asset.NaturalSize.Width;
					Controller.OnMediaOpened();
					Controller.Position = Position;
					break;
			}
		}

		TimeSpan Position
		{
			get
			{
				if (_avPlayerViewController.Player.CurrentTime.IsInvalid)
					return TimeSpan.Zero;

				return TimeSpan.FromSeconds(_avPlayerViewController.Player.CurrentTime.Seconds);
			}
		}

		void PlayedToEnd(NSNotification notification)
		{
			if (MediaElement.IsLooping)
			{
				_avPlayerViewController.Player.Seek(CMTime.Zero);
				Controller.Position = Position;
				_avPlayerViewController.Player.Play();
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
					_avPlayerViewController.VideoGravity = AspectToGravity(MediaElement.Aspect);
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
					_avPlayerViewController.ShowsPlaybackControls = MediaElement.ShowsPlaybackControls;
					break;

				case nameof(MediaElement.Source):
					UpdateSource();
					break;
			}
		}

		void MediaElementSeekRequested(object sender, SeekRequested e)
		{
			if (_avPlayerViewController.Player.Status != AVPlayerStatus.ReadyToPlay || _avPlayerViewController.Player.CurrentItem == null)
				return;

			NSValue[] ranges = _avPlayerViewController.Player.CurrentItem.SeekableTimeRanges;
			CMTime seekTo = new CMTime(Convert.ToInt64(e.Position.TotalMilliseconds), 1000);
			foreach (NSValue v in ranges)
			{
				if (seekTo >= v.CMTimeRangeValue.Start && seekTo < (v.CMTimeRangeValue.Start + v.CMTimeRangeValue.Duration))
				{
					_avPlayerViewController.Player.Seek(seekTo, SeekComplete);
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

			_avPlayerViewController.Player.Play();
			Controller.CurrentState = MediaElementState.Playing;
			if (MediaElement.KeepScreenOn)
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
					if (MediaElement.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}
					_avPlayerViewController.Player.Pause();
					Controller.CurrentState = MediaElementState.Paused;
					break;

				case MediaElementState.Stopped:
					if (MediaElement.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}
					//ios has no stop...
					_avPlayerViewController.Player.Pause();
					_avPlayerViewController.Player.Seek(CMTime.Zero);
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
			return ((UIView)this).GetSizeRequest(widthConstraint, heightConstraint, 44, 44);
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
				oldElement.PositionRequested -= MediaElement_PositionRequested;
			}

			Color currentColor = oldElement?.BackgroundColor ?? Color.Default;
			if (element.BackgroundColor != currentColor)
			{
				UpdateBackgroundColor();
			}

			MediaElement.PropertyChanged += OnElementPropertyChanged;
			MediaElement.SeekRequested += MediaElementSeekRequested;
			MediaElement.StateRequested += MediaElementStateRequested;
			MediaElement.PositionRequested += MediaElement_PositionRequested;

			AutosizesSubviews = true;

			_tracker = new VisualElementTracker(this);

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, MediaElement));

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);

			Performance.Stop(reference);
		}

		void MediaElement_PositionRequested(object sender, EventArgs e)
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

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			_avPlayerViewController.View.Frame = Bounds;
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			VisualElementRenderer<VisualElement>.RegisterEffect(effect, this);
		}

		void UpdateBackgroundColor()
		{
			BackgroundColor = MediaElement.BackgroundColor.ToUIColor();
			_avPlayerViewController.View.BackgroundColor = MediaElement.BackgroundColor.ToUIColor();
		}
	}
}