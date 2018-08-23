using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_MediaElementRenderer))]
	public sealed class MediaElement : View
	{
		public static readonly BindableProperty AreTransportControlsEnabledProperty =
		  BindableProperty.Create(nameof(AreTransportControlsEnabled), typeof(bool), typeof(MediaElement), false);

		public static readonly BindableProperty AspectProperty =
		  BindableProperty.Create(nameof(Aspect), typeof(Aspect), typeof(MediaElement), Aspect.AspectFit);

		public static readonly BindableProperty AutoPlayProperty =
		  BindableProperty.Create(nameof(AutoPlay), typeof(bool), typeof(MediaElement), true);

		public static readonly BindableProperty BufferingProgressProperty =
		  BindableProperty.Create(nameof(BufferingProgress), typeof(double), typeof(MediaElement), 0.0);

		public static readonly BindableProperty IsLoopingProperty =
		  BindableProperty.Create(nameof(IsLooping), typeof(bool), typeof(MediaElement), false);

		public static readonly BindableProperty KeepScreenOnProperty =
		  BindableProperty.Create(nameof(KeepScreenOn), typeof(bool), typeof(MediaElement), false);

		public static readonly BindableProperty SourceProperty =
		  BindableProperty.Create(nameof(Source), typeof(Uri), typeof(MediaElement));

		public static readonly BindableProperty CurrentStateProperty =
		  BindableProperty.Create(nameof(CurrentState), typeof(MediaElementState), typeof(MediaElement), MediaElementState.Closed);

		public static readonly BindableProperty PositionProperty =
		  BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(MediaElement), TimeSpan.Zero, validateValue: ValidatePosition);

		static bool ValidatePosition(BindableObject bindable, object value)
		{
			MediaElement element = bindable as MediaElement;
			if (element != null)
			{
				if (element._renderer != null)
				{
					element._renderer.Seek((TimeSpan)value);
				}
			}

			return true;
		}

		
		IMediaElementRenderer _renderer = null;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetRenderer(IMediaElementRenderer renderer)
		{
			_renderer = renderer;
		}
		
		public bool AreTransportControlsEnabled
		{
			get { return (bool)GetValue(AreTransportControlsEnabledProperty); }
			set { SetValue(AreTransportControlsEnabledProperty, value); }
		}
		
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
			get { return Source != null && NaturalDuration.HasValue; }
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

		public TimeSpan? NaturalDuration
		{
			get
			{
				if (_renderer != null)
				{
					return _renderer.NaturalDuration;
				}

				return null;
			}
		}

		public int NaturalVideoHeight
		{
			get
			{
				if (_renderer != null)
				{
					return _renderer.NaturalVideoHeight;
				}

				return 0;
			}
		}

		public int NaturalVideoWidth
		{
			get
			{
				if (_renderer != null)
				{
					return _renderer.NaturalVideoWidth;
				}

				return 0;
			}
		}
		
		[TypeConverter(typeof(UriTypeConverter))]
		public Uri Source
		{
			get { return (Uri)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

        public IDictionary<string, string> HttpHeaders { get; } = new Dictionary<string, string>();
		
		public MediaElementState CurrentState
		{
			get { return (MediaElementState)GetValue(CurrentStateProperty); }
		}
		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendCurrentState(MediaElementState state)
		{
			SetValue(CurrentStateProperty, state);
			CurrentStateChanged?.Invoke(this, EventArgs.Empty);
		}

		public TimeSpan Position
		{
			get
			{
				if (_renderer != null)
				{
					return _renderer.Position;
				}

				return (TimeSpan)GetValue(PositionProperty);
			}

			set
			{
				TimeSpan newPosition = value;
				if(value < TimeSpan.Zero)
				{
					newPosition = TimeSpan.Zero;
				}
				else if(NaturalDuration.HasValue && value > NaturalDuration.Value)
				{
					newPosition = NaturalDuration.Value;
				}

				SetValue(PositionProperty, newPosition);
			}
		}

		public void Play()
		{
			SendCurrentState(MediaElementState.Playing);
		}

		public void Pause()
		{
			if (CurrentState == MediaElementState.Playing)
			{
				SendCurrentState(MediaElementState.Paused);
			}
		}

		public void Stop()
		{
			if (CurrentState != MediaElementState.Stopped)
			{
				SendCurrentState(MediaElementState.Stopped);
			}
		}

		public Aspect Aspect
		{
			get { return (Aspect)GetValue(AspectProperty); }
			set { SetValue(AspectProperty, value); }
		}

		public event EventHandler CurrentStateChanged;

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
			SendCurrentState(MediaElementState.Stopped);

			MediaEnded?.Invoke(this, EventArgs.Empty);
		}

		internal void RaisePropertyChanged(string propertyName)
		{
			OnPropertyChanged(propertyName);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IMediaElementRenderer
	{
		double BufferingProgress { get; }

		TimeSpan? NaturalDuration { get; }

		int NaturalVideoHeight { get; }

		int NaturalVideoWidth { get; }

		TimeSpan Position { get; }

		void Seek(TimeSpan time);
	}
}