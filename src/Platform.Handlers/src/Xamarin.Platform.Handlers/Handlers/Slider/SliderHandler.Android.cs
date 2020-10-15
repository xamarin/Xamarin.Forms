using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Net;

namespace Xamarin.Platform.Handlers
{
	public partial class SliderHandler : AbstractViewHandler<ISlider, SeekBar>
	{
		static ColorStateList? DefaultProgressTintList { get; set; }
		static ColorStateList? DefaultProgressBackgroundTintList { get; set; }
		static PorterDuff.Mode? DefaultProgressTintMode { get; set; }
		static PorterDuff.Mode? DefaultProgressBackgroundTintMode { get; set; }
		static ColorFilter? DefaultThumbColorFilter { get; set; }

		SeekBarChangeListener ChangeListener { get; } = new SeekBarChangeListener();

		protected override SeekBar CreateNativeView()
		{
			var slider = new SeekBar(Context);
			ChangeListener.Handler = this;
			slider.SetOnSeekBarChangeListener(ChangeListener);
			return slider;
		}

		protected override void TearDown(SeekBar nativeView)
		{
			ChangeListener.Handler = null;
			nativeView.SetOnSeekBarChangeListener(null);
		}

		protected override void SetupDefaults(SeekBar nativeView)
		{
			DefaultThumbColorFilter = nativeView.Thumb?.GetColorFilter();
			DefaultProgressTintMode = nativeView.ProgressTintMode;
			DefaultProgressBackgroundTintMode = nativeView.ProgressBackgroundTintMode;
			DefaultProgressTintList = nativeView.ProgressTintList;
			DefaultProgressBackgroundTintList = nativeView.ProgressBackgroundTintList;
		}

		public static void MapMinimum(SliderHandler handler, ISlider slider)
		{
			ViewHandler.CheckParameters(handler, slider);

			handler.TypedNativeView?.UpdateMinimum(slider);
		}

		public static void MapMaximum(SliderHandler handler, ISlider slider)
		{
			ViewHandler.CheckParameters(handler, slider);

			handler.TypedNativeView?.UpdateMaximum(slider);
		}

		public static void MapValue(SliderHandler handler, ISlider slider)
		{
			ViewHandler.CheckParameters(handler, slider);

			handler.TypedNativeView?.UpdateValue(slider);
		}

		public static void MapMinimumTrackColor(SliderHandler handler, ISlider slider)
		{
			ViewHandler.CheckParameters(handler, slider);

			handler.TypedNativeView?.UpdateMinimumTrackColor(slider, DefaultProgressBackgroundTintList, DefaultProgressBackgroundTintMode);
		}

		public static void MapMaximumTrackColor(SliderHandler handler, ISlider slider)
		{
			ViewHandler.CheckParameters(handler, slider);

			handler.TypedNativeView?.UpdateMaximumTrackColor(slider, DefaultProgressTintList, DefaultProgressTintMode);
		}

		public static void MapThumbColor(SliderHandler handler, ISlider slider)
		{
			ViewHandler.CheckParameters(handler, slider);

			handler.TypedNativeView?.UpdateThumbColor(slider, DefaultThumbColorFilter);
		}

		internal class SeekBarChangeListener : Java.Lang.Object, SeekBar.IOnSeekBarChangeListener
		{
			public SliderHandler? Handler { get; set; }
			public ISlider? VirtualView => Handler?.VirtualView;

			public SeekBarChangeListener()
			{
			}

			public void OnProgressChanged(SeekBar? seekBar, int progress, bool fromUser)
			{
				if (VirtualView == null)
					return;

				VirtualView.Value = progress;
			}

			public void OnStartTrackingTouch(SeekBar? seekBar)
			{
				VirtualView?.DragStarted();
			}

			public void OnStopTrackingTouch(SeekBar? seekBar)
			{
				VirtualView?.DragCompleted();
			}
		}
	}
}