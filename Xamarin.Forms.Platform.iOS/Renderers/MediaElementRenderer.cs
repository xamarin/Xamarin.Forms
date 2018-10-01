using System;
using System.Collections.Generic;
using System.IO;
using AVFoundation;
using AVKit;
using CoreGraphics;
using CoreMedia;
using Foundation;
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

		public MediaElementRenderer()
		{
			_playToEndObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, PlayedToEnd);
			_avPlayerViewController.View.Frame = Bounds;
			AddSubview(_avPlayerViewController.View);
		}

		public override CGRect Frame
		{
			get => base.Frame;
			set
			{
				base.Frame = value;
				_avPlayerViewController.View.Frame = Bounds;				
			}
		}

		VisualElement IVisualElementRenderer.Element => MediaElement;

		UIView IVisualElementRenderer.NativeView => this;

		UIViewController IVisualElementRenderer.ViewController => _avPlayerViewController;
		
		bool _idleTimerDisabled = false;
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
			else
			{
				if (_idleTimerDisabled)
				{
					_idleTimerDisabled = false;
					UIApplication.SharedApplication.IdleTimerDisabled = false;
				}
			}
		}

		AVUrlAssetOptions GetOptionsWithHeaders(IDictionary<string, string> headers)
		{
			if (headers == null || headers.Count == 0)
				return null;

			var nativeHeaders = new NSMutableDictionary();

			foreach (var header in headers)
			{
				nativeHeaders.Add((NSString)header.Key, (NSString)header.Value);
			}

			var nativeHeadersKey = (NSString)"AVURLAssetHTTPHeaderFieldsKey";

			var options = new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
				nativeHeaders,
				nativeHeadersKey
			));

			return options;
		}

		void UpdateSource()
		{
			if (MediaElement.Source != null)
			{
				AVAsset asset = null;
				if (MediaElement.Source.Scheme == null)
				{
					// file path
					asset = AVAsset.FromUrl(NSUrl.FromFilename(MediaElement.Source.OriginalString));
				}
				else if (MediaElement.Source.Scheme == "ms-appx")
				{
					// used for a file embedded in the application package
					asset = AVAsset.FromUrl(NSUrl.FromFilename(MediaElement.Source.LocalPath.Substring(1)));
				}
				else if (MediaElement.Source.Scheme == "ms-appdata")
				{
					string filePath = string.Empty;

					if (MediaElement.Source.LocalPath.StartsWith("/local"))
					{
						filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), MediaElement.Source.LocalPath.Substring(7));
					}
					else if (MediaElement.Source.LocalPath.StartsWith("/temp"))
					{
						filePath = Path.Combine(Path.GetTempPath(), MediaElement.Source.LocalPath.Substring(6));
					}
					else
					{
						throw new ArgumentException("Invalid Uri", "Source");
					}

					asset = AVAsset.FromUrl(NSUrl.FromFilename(filePath));
				}
				else
				{
					asset = AVUrlAsset.Create(NSUrl.FromString(MediaElement.Source.AbsoluteUri), GetOptionsWithHeaders(MediaElement.HttpHeaders));
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
				{
					var audioSession = AVAudioSession.SharedInstance();
					NSError err = audioSession.SetCategory(AVAudioSession.CategoryPlayback);
					audioSession.SetMode(AVAudioSession.ModeMoviePlayback, out err);
					err = audioSession.SetActive(true);

					_avPlayerViewController.Player.Play();
					Controller.CurrentState = MediaElementState.Playing;
				}
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
				catch { }
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
			if (e.NewValue != null)
			{
				switch (_avPlayerViewController.Player.Status)
				{
					case AVPlayerStatus.Failed:
						MediaElement.OnMediaFailed();
						break;

					case AVPlayerStatus.ReadyToPlay:
						Controller.Duration = TimeSpan.FromSeconds(_avPlayerViewController.Player.CurrentItem.Duration.Seconds);
						Controller.VideoHeight = (int)_avPlayerViewController.Player.CurrentItem.Asset.NaturalSize.Height;
						Controller.VideoWidth = (int)_avPlayerViewController.Player.CurrentItem.Asset.NaturalSize.Width;
						MediaElement?.RaiseMediaOpened();
						Controller.Position = Position;
						break;
				}
			}
		}

		TimeSpan Position
		{
			get
			{
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
					Device.BeginInvokeOnMainThread(MediaElement.OnMediaEnded);
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

		

		void MediaElement_SeekRequested(object sender, SeekRequested e)
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

		void MediaElement_StateRequested(object sender, StateRequested e)
		{
			switch (e.State)
			{
				case MediaElementState.Playing:
					var audioSession = AVAudioSession.SharedInstance();
					NSError err = audioSession.SetCategory(AVAudioSession.CategoryPlayback);
					audioSession.SetMode(AVAudioSession.ModeMoviePlayback, out err);
					err = audioSession.SetActive(true);

					_avPlayerViewController.Player.Play();
					Controller.CurrentState = MediaElementState.Playing;
					if (MediaElement.KeepScreenOn)
					{
						SetKeepScreenOn(true);
					}
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

					err = AVAudioSession.SharedInstance().SetActive(false);
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
				MediaElement.RaiseSeekCompleted();
			}
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return ((IVisualElementRenderer)this).NativeView.GetSizeRequest(widthConstraint, heightConstraint, 44, 44);
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
			}

			Color currentColor = oldElement?.BackgroundColor ?? Color.Default;
			if (element.BackgroundColor != currentColor)
			{
				UpdateBackgroundColor();
			}

			MediaElement.PropertyChanged += OnElementPropertyChanged;
			MediaElement.SeekRequested += MediaElement_SeekRequested;
			MediaElement.StateRequested += MediaElement_StateRequested;

			_tracker = new VisualElementTracker(this);

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, MediaElement));

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);

			Performance.Stop(reference);
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
			VisualElementRenderer<VisualElement>.RegisterEffect(effect, this);
		}

		void UpdateBackgroundColor()
		{
			BackgroundColor = MediaElement.BackgroundColor.ToUIColor();
		}
	}
}