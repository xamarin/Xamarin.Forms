using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AView = Android.Views.View;
using Android.Widget;
using Xamarin.Forms.Internals;
using Android.Media;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	internal sealed class MediaElementRenderer : FrameLayout, IVisualElementRenderer, IViewRenderer, IEffectControlProvider, MediaPlayer.IOnCompletionListener, MediaPlayer.IOnInfoListener, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnErrorListener
	{
		bool _isDisposed;
		int? _defaultLabelFor;
		MediaElement MediaElement { get; set; }
		readonly AutomationPropertiesProvider _automationPropertiesProvider;
		readonly EffectControlProvider _effectControlProvider;
		VisualElementTracker _tracker;

		MediaController _controller;
		FormsVideoView _view;
		MediaPlayer _mediaPlayer;

		public MediaElementRenderer(Context context) : base(context)
		{
			_automationPropertiesProvider = new AutomationPropertiesProvider(this);
			_effectControlProvider = new EffectControlProvider(this);

			Initialize();
		}

		public VisualElement Element => MediaElement;

		VisualElementTracker IVisualElementRenderer.Tracker => _tracker;

		ViewGroup IVisualElementRenderer.ViewGroup => null;

		AView IVisualElementRenderer.View => this;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			AView view = this;
			view.Measure(widthConstraint, heightConstraint);

			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), new Size());
		}

		void Initialize()
		{
		}

		void IViewRenderer.MeasureExactly()
		{
			ViewRenderer.MeasureExactly(this, Element, Context);
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
			MediaElement.SeekRequested += SeekRequested;
			MediaElement.StateRequested += StateRequested;

			if (_tracker == null)
			{
				// Can't set up the tracker in the constructor because it access the Element (for now)
				SetTracker(new VisualElementTracker(this));
			}

			OnElementChanged(new ElementChangedEventArgs<MediaElement>(oldElement as MediaElement, MediaElement));

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);

			Performance.Stop(reference);
		}

		void StateRequested(object sender, StateRequested e)
		{
			switch (e.State)
			{
				case MediaElementState.Playing:
					_view.Start();
					((IMediaElementController)MediaElement).CurrentState = _view.IsPlaying ? MediaElementState.Playing : MediaElementState.Stopped;
					break;

				case MediaElementState.Paused:
					if (_view.CanPause())
					{
						_view.Pause();
						((IMediaElementController)MediaElement).CurrentState = MediaElementState.Paused;
					}
					break;

				case MediaElementState.Stopped:
					_view.Pause();
					_view.SeekTo(0);

					((IMediaElementController)MediaElement).CurrentState = _view.IsPlaying ? MediaElementState.Playing : MediaElementState.Stopped;
					break;
			}

			UpdateLayoutParameters();
			((IMediaElementController)MediaElement).Position = _view.Position;
		}

		void SeekRequested(object sender, SeekRequested e)
		{
			((IMediaElementController)MediaElement).Position = _view.Position;
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
			{
				_defaultLabelFor = LabelFor;
			}

			LabelFor = (int)(id ?? _defaultLabelFor);
		}
		void SetTracker(VisualElementTracker tracker)
		{
			_tracker = tracker;
		}

		void UpdateBackgroundColor()
		{
			SetBackgroundColor(Element.BackgroundColor.ToAndroid());
		}

		void IVisualElementRenderer.UpdateLayout()
		{
			var lp = _view.LayoutParameters;
			lp.Width = Width;
			lp.Height = Height;
			_view.LayoutParameters = lp;

			_tracker?.UpdateLayout();
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;

			if (disposing)
			{
				SetOnClickListener(null);
				SetOnTouchListener(null);

				_automationPropertiesProvider?.Dispose();
				_tracker?.Dispose();

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;

					if (Platform.GetRenderer(Element) == this)
						Element.ClearValue(Platform.RendererProperty);
				}
			}

			base.Dispose(disposing);
		}

		void OnElementChanged(ElementChangedEventArgs<MediaElement> e)
		{
			if (e.OldElement != null)
			{
				ReleaseControl();
			}

			if (e.NewElement != null && !_isDisposed)
			{
				this.EnsureId();
				
				_view = new FormsVideoView(Context);
				_view.SetZOrderMediaOverlay(true);
				_view.SetOnCompletionListener(this);
				_view.SetOnInfoListener(this);
				_view.SetOnPreparedListener(this);
				_view.SetOnErrorListener(this);
				_view.KeepScreenOn = e.NewElement.KeepScreenOn;
				_view.MetadataRetrieved += MetadataRetrieved;

				AddView(_view, -1,-1);

				_controller = new MediaController(Context);
				_controller.SetAnchorView(this);
				_controller.Visibility = e.NewElement.AreTransportControlsEnabled ? ViewStates.Visible : ViewStates.Gone;
				_view.SetMediaController(_controller);

				UpdateLayoutParameters();
				UpdateSource();
				UpdateBackgroundColor();

				ElevationHelper.SetElevation(this, e.NewElement);
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		void MetadataRetrieved(object sender, EventArgs e)
		{
			((IMediaElementController)MediaElement).Duration = _view.DurationTimeSpan;
			((IMediaElementController)MediaElement).VideoHeight = _view.VideoHeight;
			((IMediaElementController)MediaElement).VideoWidth = _view.VideoWidth;
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(MediaElement.Aspect):
					UpdateLayoutParameters();
					break;

				case nameof(MediaElement.Source):
					UpdateSource();
					break;
			}

			ElementPropertyChanged?.Invoke(this, e);
		}

		public void RegisterEffect(Effect effect)
		{
			_effectControlProvider.RegisterEffect(effect);
		}

		void UpdateSource()
		{
			if (MediaElement.Source != null)
			{
				if (MediaElement.Source.Scheme == null)
				{
					_view.SetVideoPath(MediaElement.Source.AbsolutePath);
				}
				else if (MediaElement.Source.Scheme == "ms-appx")
				{
					// video resources should be in the raw folder with Build Action set to AndroidResource
					string uri = "android.resource://" + Context.PackageName + "/raw/" + MediaElement.Source.LocalPath.Substring(1, MediaElement.Source.LocalPath.LastIndexOf('.') - 1).ToLower();
					_view.SetVideoURI(global::Android.Net.Uri.Parse(uri));
				}
				else if (MediaElement.Source.Scheme == "ms-appdata")
				{
					string filePath = string.Empty;

					if (MediaElement.Source.LocalPath.StartsWith("/local"))
					{
						filePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), MediaElement.Source.LocalPath.Substring(7));
					}
					else if (MediaElement.Source.LocalPath.StartsWith("/temp"))
					{
						filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), MediaElement.Source.LocalPath.Substring(6));
					}
					else
					{
						throw new ArgumentException("Invalid Uri", "Source");
					}

					_view.SetVideoPath(filePath);

				}
				else
				{
					if (MediaElement.Source.IsFile)
					{
						_view.SetVideoPath(MediaElement.Source.AbsolutePath);
					}
					else
					{
						_view.SetVideoURI(global::Android.Net.Uri.Parse(MediaElement.Source.AbsoluteUri), MediaElement.HttpHeaders);
					}
				}

				

				if (MediaElement.AutoPlay)
				{
					_view.Start();
					((IMediaElementController)MediaElement).CurrentState = _view.IsPlaying ? MediaElementState.Playing : MediaElementState.Stopped;
				}

			}
			else if (_view.IsPlaying)
			{
				_view.StopPlayback();
				((IMediaElementController)MediaElement).CurrentState = MediaElementState.Stopped;
			}
		}

		void MediaPlayer.IOnCompletionListener.OnCompletion(MediaPlayer mp)
		{
			((IMediaElementController)Element).Position = TimeSpan.FromMilliseconds(_mediaPlayer.CurrentPosition);
			MediaElement.OnMediaEnded();
		}

		void MediaPlayer.IOnPreparedListener.OnPrepared(MediaPlayer mp)
		{
			MediaElement?.RaiseMediaOpened();

			UpdateLayoutParameters();

			_mediaPlayer = mp;
			mp.Looping = MediaElement.IsLooping;
			mp.SeekTo(0);

			if (MediaElement.AutoPlay)
			{
				_mediaPlayer.Start();
				((IMediaElementController)Element).CurrentState = MediaElementState.Playing;
			}
			else
			{
				((IMediaElementController)Element).CurrentState = MediaElementState.Paused;
			}
		}

		void UpdateLayoutParameters()
		{
			if (_view.VideoWidth == 0 || _view.VideoHeight == 0)
			{
				_view.LayoutParameters = new FrameLayout.LayoutParams(Width, Height, GravityFlags.Fill);
				return;
			}

			float ratio = (float)_view.VideoWidth / (float)_view.VideoHeight;
			float controlRatio = (float)Width / Height;

			switch (MediaElement.Aspect)
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

		bool MediaPlayer.IOnErrorListener.OnError(MediaPlayer mp, MediaError what, int extra)
		{
			MediaElement.OnMediaFailed();
			return false;
		}
		

		bool MediaPlayer.IOnInfoListener.OnInfo(MediaPlayer mp, MediaInfo what, int extra)
		{
			System.Diagnostics.Debug.WriteLine(what);
			switch (what)
			{
				case MediaInfo.BufferingStart:
					((IMediaElementController)MediaElement).CurrentState = MediaElementState.Buffering;
					mp.BufferingUpdate += Mp_BufferingUpdate;
					break;

				case MediaInfo.BufferingEnd:
					mp.BufferingUpdate -= Mp_BufferingUpdate;
					((IMediaElementController)MediaElement).CurrentState = MediaElementState.Paused;
					break;

				case MediaInfo.VideoRenderingStart:
					((IMediaElementController)MediaElement).CurrentState = MediaElementState.Playing;
					break;
			}

			_mediaPlayer = mp;
			//_mediaPlayer.SetVideoScalingMode(MediaElement.Aspect == Aspect.AspectFill ? VideoScalingMode.ScaleToFitWithCropping : VideoScalingMode.ScaleToFit);
			return true;
		}

		void Mp_BufferingUpdate(object sender, MediaPlayer.BufferingUpdateEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(e.Percent + "%");
			((IMediaElementController)MediaElement).BufferingProgress = e.Percent / 100f;
		}
	}
}