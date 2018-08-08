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
	public sealed class MediaElementRenderer : ViewRenderer<MediaElement, FrameLayout>, MediaPlayer.IOnCompletionListener, MediaPlayer.IOnPreparedListener, IMediaElementRenderer
	{
		MediaController _controller;
		FormsVideoView _view;
		MediaPlayer _mediaPlayer;

		public MediaElementRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		double IMediaElementRenderer.BufferingProgress
		{
			get
			{
				if (_view != null)
				{
					return _view.BufferPercentage / 100;
				}

				return 0.0;
			}
		}

		TimeSpan? IMediaElementRenderer.NaturalDuration
		{
			get
			{
				if (_view != null)
				{
					return _view.NaturalDuration;
				}

				return null;
			}
		}

		int IMediaElementRenderer.NaturalVideoHeight
		{
			get
			{
				if (_view != null)
				{
					return _view.VideoHeight;
				}

				return 0;
			}
		}

		int IMediaElementRenderer.NaturalVideoWidth
		{
			get
			{
				if (_view != null)
				{
					return _view.VideoWidth;
				}

				return 0;
			}
		}

		void IMediaElementRenderer.Seek(TimeSpan time)
		{
			if (Control != null)
			{
				try
				{
					_view?.SeekTo((int)time.TotalMilliseconds);
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		TimeSpan IMediaElementRenderer.Position
		{
			get
			{
				try
				{
					return TimeSpan.FromMilliseconds(_view.CurrentPosition);
				}
				catch
				{
					return TimeSpan.Zero;
				}
			}
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
				e.OldElement.SetRenderer(null);

				ReleaseControl();
			}

			if (e.NewElement != null)
			{
				try
				{
					e.NewElement.SetRenderer(this);

					_view = new FormsVideoView(Context);
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

					UpdateSource();
				}
				catch
				{ }
			}
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
					_view.SetVideoPath(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Element.Source.LocalPath.Substring(1)));
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
					Element.SendCurrentState(MediaElementState.Playing);
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
				case nameof(MediaElement.AreTransportControlsEnabled):
					_controller.Visibility = Element.AreTransportControlsEnabled ? ViewStates.Visible : ViewStates.Gone;
					break;

				case nameof(MediaElement.Source):
					UpdateSource();
					break;

				case nameof(MediaElement.CurrentState):
					switch (Element.CurrentState)
					{
						case MediaElementState.Playing:
							if (!_view.IsPlaying)
							{
								_view.Start();
							}
							Element.SendCurrentState(_view.IsPlaying ? MediaElementState.Playing : MediaElementState.Stopped);
							break;

						case MediaElementState.Paused:
							_view.Pause();
							break;

						case MediaElementState.Stopped:
							if (_view.IsPlaying)
							{
								_view.SeekTo(0);
								_view.StopPlayback();
							}
							Element.SendCurrentState(_view.IsPlaying ? MediaElementState.Playing : MediaElementState.Stopped);
							break;
					}

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

				case nameof(MediaElement.Aspect):
					UpdateLayoutParameters();
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
			if (Element.NaturalVideoWidth == 0 || Element.NaturalVideoHeight == 0)
			{
				_view.LayoutParameters = new FrameLayout.LayoutParams(Width, Height, GravityFlags.Fill);
				return;
			}

			float ratio = (float)Element.NaturalVideoWidth / Element.NaturalVideoHeight;
			float controlRatio = (float)Width / Height;

			switch (Element.Aspect)
			{
				case Aspect.Fill:
					// TODO: this doesn't stretch like other platforms...
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
			if (mp.CurrentPosition > 0)
			{
				mp.SeekTo(0);
			}

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
				Element.SendCurrentState(MediaElementState.Playing);
			}
		}
	}
}