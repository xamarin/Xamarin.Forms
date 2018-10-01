using Android.Content;
using Android.Media;
using Android.Views;
using Android.Widget;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Runtime;



namespace Xamarin.Forms.Platform.Android
{
	public sealed class MediaElementRenderer : ViewRenderer<MediaElement, FrameLayout>, MediaPlayer.IOnCompletionListener, MediaPlayer.IOnPreparedListener
	{
		MediaController _controller;
		FormsVideoView _view;
		MediaPlayer _mediaPlayer;

		IMediaElementController Controller => Element as IMediaElementController;


		public MediaElementRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (Control != null)
			{
				Control.RemoveAllViews();
			}

			ReleaseControl();
			base.Dispose(disposing);
		}

		void ReleaseControl()
		{
			if (_view != null)
			{
				_view.MetadataRetrieved -= MetadataRetrieved;
				_view.SetOnPreparedListener(null);
				_view.SetOnCompletionListener(null);
				_view.Dispose();
				_view = null;
			}

			if (_controller != null)
			{
				_controller.Dispose();
				_controller = null;
			}

			if (_mediaPlayer != null)
			{
				_mediaPlayer.Dispose();
				_mediaPlayer = null;
			}
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
				_view = new FormsVideoView(Context);
				_view.MetadataRetrieved += MetadataRetrieved;
				SetNativeControl(new FrameLayout(Context));

				_view.SetZOrderMediaOverlay(true);
				_view.SetOnCompletionListener(this);
				_view.SetOnPreparedListener(this);
				_view.KeepScreenOn = Element.KeepScreenOn;

				Control.AddView(_view);

				_controller = new MediaController(Context);
				_controller.SetAnchorView(this);
				_controller.Visibility = Element.AreTransportControlsEnabled ? ViewStates.Visible : ViewStates.Gone;
				_view.SetMediaController(_controller);

				Element.SeekRequested += SeekRequested;
				Element.StateRequested += StateRequested;

				UpdateSource();
			}
		}

		void MetadataRetrieved(object sender, EventArgs e)
		{
			Controller.Duration = _view.DurationTimeSpan;
			Controller.VideoHeight = _view.VideoHeight;
			Controller.VideoWidth = _view.VideoWidth;
		}

		void StateRequested(object sender, StateRequested e)
		{
			switch (e.State)
			{
				case MediaElementState.Playing:
					_view.Start();
					Controller.CurrentState = _view.IsPlaying ? MediaElementState.Playing : MediaElementState.Stopped;
					break;

				case MediaElementState.Paused:
					if (_view.CanPause())
					{
						_view.Pause();
						Controller.CurrentState = MediaElementState.Paused;
					}
					break;

				case MediaElementState.Stopped:
					_view.Pause();
					_view.SeekTo(0);

					Controller.CurrentState = _view.IsPlaying ? MediaElementState.Playing : MediaElementState.Stopped;
					break;
			}

			UpdateLayoutParameters();
			Controller.Position = _view.Position;
		}

		void SeekRequested(object sender, SeekRequested e)
		{
			Controller.Position = _view.Position;
		}


		void UpdateSource()
		{
			if (Element.Source != null)
			{
				if (Element.Source.Scheme == null)
				{
					_view.SetVideoPath(Element.Source.AbsolutePath);
				}
				else if (Element.Source.Scheme == "ms-appx")
				{
					// video resources should be in the raw folder with Build Action set to AndroidResource
					string uri = "android.resource://" + Context.PackageName + "/raw/" + Element.Source.LocalPath.Substring(1, Element.Source.LocalPath.LastIndexOf('.') - 1).ToLower();
					_view.SetVideoURI(global::Android.Net.Uri.Parse(uri));
				}
				else if (Element.Source.Scheme == "ms-appdata")
				{
					string filePath = string.Empty;

					if (Element.Source.LocalPath.StartsWith("/local"))
					{
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

					_view.SetVideoPath(filePath);

				}
				else
				{
					if (Element.Source.IsFile)
					{
						_view.SetVideoPath(Element.Source.AbsolutePath);
					}
					else
					{
						_view.SetVideoURI(global::Android.Net.Uri.Parse(Element.Source.ToString()), Element.HttpHeaders);
					}
				}

				if (Element.AutoPlay)
				{
					_view.Start();
					Controller.CurrentState = _view.IsPlaying ? MediaElementState.Playing : MediaElementState.Stopped;
				}

			}
			else
			{
				if (Element.CurrentState == MediaElementState.Playing || Element.CurrentState == MediaElementState.Buffering)
				{
					Element.Stop();
				}
			}
		}



		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(MediaElement.Aspect):
					UpdateLayoutParameters();
					break;

				case nameof(MediaElement.IsLooping):
					if (_mediaPlayer != null)
					{
						_mediaPlayer.Looping = Element.IsLooping;
					}
					break;

				case nameof(MediaElement.KeepScreenOn):
					_view.KeepScreenOn = Element.KeepScreenOn;
					break;

				case nameof(MediaElement.ShowsPlaybackControls):
					_controller.Visibility = Element.ShowsPlaybackControls ? ViewStates.Visible : ViewStates.Gone;
					break;

				case nameof(MediaElement.Source):
					UpdateSource();
					break;
			}

			base.OnElementPropertyChanged(sender, e);
		}

		protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged(w, h, oldw, oldh);
			UpdateLayoutParameters();
		}

		void UpdateLayoutParameters()
		{
			if (_view.VideoWidth == 0 || _view.VideoHeight == 0)
			{
				_view.LayoutParameters = new FrameLayout.LayoutParams(Width, Height, GravityFlags.Fill);
				return;
			}

			float ratio = (float)_view.VideoWidth / _view.VideoHeight;
			float controlRatio = (float)Width / Height;

			switch (Element.Aspect)
			{
				case Aspect.Fill:
					// This doesn't stretch like other platforms as Android won't display out of aspect ratio
					_view.LayoutParameters = new FrameLayout.LayoutParams(Width, Height, GravityFlags.FillHorizontal | GravityFlags.FillVertical | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical) { LeftMargin = 0, RightMargin = 0, TopMargin = 0, BottomMargin = 0 };
					break;

				case Aspect.AspectFit:
					if (ratio > controlRatio)
					{
						int requiredHeight = (int)(Width / ratio);
						int vertMargin = (Height - requiredHeight) / 2;
						_view.LayoutParameters = new FrameLayout.LayoutParams(Width, requiredHeight, GravityFlags.FillHorizontal | GravityFlags.CenterVertical) { LeftMargin = 0, RightMargin = 0, TopMargin = vertMargin, BottomMargin = vertMargin };
					}
					else
					{
						int requiredWidth = (int)(Height * ratio);
						int horizMargin = (Width - requiredWidth) / 2;
						_view.LayoutParameters = new FrameLayout.LayoutParams(requiredWidth, Height, GravityFlags.CenterHorizontal | GravityFlags.FillVertical) { LeftMargin = horizMargin, RightMargin = horizMargin, TopMargin = 0, BottomMargin = 0 };
					}
					break;

				case Aspect.AspectFill:
					if (ratio > controlRatio)
					{
						int requiredWidth = (int)(Height * ratio);
						int horizMargin = (Width - requiredWidth) / 2;
						_view.LayoutParameters = new FrameLayout.LayoutParams((int)(Height * ratio), Height, GravityFlags.CenterHorizontal | GravityFlags.FillVertical) { TopMargin = 0, BottomMargin = 0, LeftMargin = horizMargin, RightMargin = horizMargin };
					}
					else
					{
						int requiredHeight = (int)(Width / ratio);
						int vertMargin = (Height - requiredHeight) / 2;
						_view.LayoutParameters = new FrameLayout.LayoutParams(Width, requiredHeight, GravityFlags.FillHorizontal | GravityFlags.CenterVertical) { LeftMargin = 0, RightMargin = 0, TopMargin = vertMargin, BottomMargin = vertMargin };
					}

					break;
			}
		}

		void MediaPlayer.IOnCompletionListener.OnCompletion(MediaPlayer mp)
		{
			Element.OnMediaEnded();
		}

		void MediaPlayer.IOnPreparedListener.OnPrepared(MediaPlayer mp)
		{
			Element?.RaiseMediaOpened();

			UpdateLayoutParameters();

			_mediaPlayer = mp;
			mp.Looping = Element.IsLooping;
			mp.SeekTo(0);

			if (Element.AutoPlay)
			{
				_mediaPlayer.Start();
				Controller.CurrentState = MediaElementState.Playing;
			}
			else
			{
				Controller.CurrentState = MediaElementState.Paused;
			}
		}
	}
}