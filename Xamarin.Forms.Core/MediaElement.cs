using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_MediaElementRenderer))]
	public sealed class MediaElement : View, IMediaElementController
	{
		public static readonly BindableProperty AspectProperty =
		  BindableProperty.Create(nameof(Aspect), typeof(Aspect), typeof(MediaElement), Aspect.AspectFit);

		public static readonly BindableProperty AutoPlayProperty =
		  BindableProperty.Create(nameof(AutoPlay), typeof(bool), typeof(MediaElement), true);

		public static readonly BindableProperty BufferingProgressProperty =
		  BindableProperty.Create(nameof(BufferingProgress), typeof(double), typeof(MediaElement), 0.0);

		public static readonly BindableProperty CurrentStateProperty =
		  BindableProperty.Create(nameof(CurrentState), typeof(MediaElementState), typeof(MediaElement), MediaElementState.Closed);

		public static readonly BindableProperty DurationProperty =
		  BindableProperty.Create(nameof(Duration), typeof(TimeSpan?), typeof(MediaElement), null);

		public static readonly BindableProperty IsLoopingProperty =
		  BindableProperty.Create(nameof(IsLooping), typeof(bool), typeof(MediaElement), false);

		public static readonly BindableProperty KeepScreenOnProperty =
		  BindableProperty.Create(nameof(KeepScreenOn), typeof(bool), typeof(MediaElement), false);

		public static readonly BindableProperty PositionProperty =
		  BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(MediaElement), TimeSpan.Zero);

		public static readonly BindableProperty ShowsPlaybackControlsProperty =
		  BindableProperty.Create(nameof(ShowsPlaybackControls), typeof(bool), typeof(MediaElement), false);

		public static readonly BindableProperty SourceProperty =
		  BindableProperty.Create(nameof(Source), typeof(Uri), typeof(MediaElement));

		public static readonly BindableProperty VideoHeightProperty =
		  BindableProperty.Create(nameof(VideoHeight), typeof(int), typeof(MediaElement));

		public static readonly BindableProperty VideoWidthProperty =
		  BindableProperty.Create(nameof(VideoWidth), typeof(int), typeof(MediaElement));
				
		public bool AutoPlay
		{
			get { return (bool)GetValue(AutoPlayProperty); }
			set { SetValue(AutoPlayProperty, value); }
		}

		public double BufferingProgress
		{
			get { return (double)GetValue(BufferingProgressProperty); }
		}
		
		public bool CanSeek
		{
			get { return Source != null && Duration.HasValue; }
		}

		public MediaElementState CurrentState
		{
			get { return (MediaElementState)GetValue(CurrentStateProperty); }
		}

		public TimeSpan? Duration
		{
			get { return (TimeSpan?)GetValue(DurationProperty); }
		}

		public bool IsLooping
		{
			get { return (bool)GetValue(IsLoopingProperty); }
			set { SetValue(IsLoopingProperty, value); }
		}
		
		public bool KeepScreenOn
		{
			get { return (bool)GetValue(KeepScreenOnProperty); }
			set { SetValue(KeepScreenOnProperty, value); }
		}

		public bool ShowsPlaybackControls
		{
			get { return (bool)GetValue(ShowsPlaybackControlsProperty); }
			set { SetValue(ShowsPlaybackControlsProperty, value); }
		}

		[TypeConverter(typeof(UriTypeConverter))]
		public Uri Source
		{
			get { return (Uri)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

        public IDictionary<string, string> HttpHeaders { get; } = new Dictionary<string, string>();
		
		public TimeSpan Position
		{
			get
			{
				PositionRequested?.Invoke(this, EventArgs.Empty);
				return (TimeSpan)GetValue(PositionProperty);
			}

			set
			{
				SeekRequested?.Invoke(this, new SeekRequested(value));
			}
		}

		public int VideoHeight
		{
			get { return (int)GetValue(VideoHeightProperty); }
		}

		public int VideoWidth
		{
			get { return (int)GetValue(VideoWidthProperty); }
		}


		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<SeekRequested> SeekRequested;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<StateRequested> StateRequested;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler PositionRequested;

		public void Play()
		{
			StateRequested?.Invoke(this, new StateRequested(MediaElementState.Playing));
		}

		public void Pause()
		{
			StateRequested?.Invoke(this, new StateRequested(MediaElementState.Paused));
		}

		public void Stop()
		{
			StateRequested?.Invoke(this, new StateRequested(MediaElementState.Stopped));
		}

		public Aspect Aspect
		{
			get => (Aspect)GetValue(AspectProperty);
			set => SetValue(AspectProperty, value);
		}

		double IMediaElementController.BufferingProgress { get => (double)GetValue(BufferingProgressProperty); set => SetValue(BufferingProgressProperty, value); }
		MediaElementState IMediaElementController.CurrentState { get => (MediaElementState)GetValue(CurrentStateProperty); set => SetValue(CurrentStateProperty, value); }
		TimeSpan? IMediaElementController.Duration { get => (TimeSpan?)GetValue(DurationProperty); set => SetValue(DurationProperty, value); }
		TimeSpan IMediaElementController.Position { get => (TimeSpan)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
		int IMediaElementController.VideoHeight { get => (int)GetValue(VideoHeightProperty); set => SetValue(VideoHeightProperty, value); }
		int IMediaElementController.VideoWidth { get => (int)GetValue(VideoWidthProperty); set => SetValue(VideoWidthProperty, value); }

		public event EventHandler MediaEnded;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void OnMediaFailed()
		{
			MediaFailed?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler MediaFailed;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseMediaOpened()
		{
			MediaOpened?.Invoke(this, EventArgs.Empty);
		}
		
		public event EventHandler MediaOpened;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseSeekCompleted()
		{
			SeekCompleted?.Invoke(this, EventArgs.Empty);
		}
		
		public event EventHandler SeekCompleted;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void OnMediaEnded()
		{
			SetValue(CurrentStateProperty, MediaElementState.Stopped);
			MediaEnded?.Invoke(this, EventArgs.Empty);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public class SeekRequested : EventArgs
	{
		public TimeSpan Position { get; }

		public SeekRequested(TimeSpan position)
		{
			Position = position;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public class StateRequested : EventArgs
	{
		public MediaElementState State { get; }

		public StateRequested(MediaElementState state)
		{
			State = state;
		}
	}

	public interface IMediaElementController
	{
		double BufferingProgress { get; set; }
		MediaElementState CurrentState { get; set; }
		TimeSpan? Duration { get; set; }
		TimeSpan Position { get; set; }
		int VideoHeight { get; set; }
		int VideoWidth { get; set; }
	}

	public interface IMediaElementDelegate
	{

	}
}