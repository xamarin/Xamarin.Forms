using System;
using ElmSharp;
using Xamarin.Platform.Tizen;

namespace Xamarin.Platform.Handlers
{
	public partial class SliderHandler : AbstractViewHandler<ISlider, Slider>
	{
		static Color? DefaultMinTrackColor;
		static Color? DefaultMaxTrackColor;
		static Color? DefaultThumbColor;

		protected override Slider CreateNativeView() => new Slider(this.NativeParent);

		protected override void ConnectHandler(Slider nativeView)
		{
			nativeView.ValueChanged += OnControlValueChanged;
			nativeView.DragStarted += OnDragStarted;
			nativeView.DragStopped += OnDragStopped;
		}

		protected override void DisconnectHandler(Slider nativeView)
		{
			nativeView.ValueChanged -= OnControlValueChanged;
			nativeView.DragStarted -= OnDragStarted;
			nativeView.DragStopped -= OnDragStopped;
		}

		protected override void SetupDefaults(Slider nativeView)
		{
			DefaultMinTrackColor = nativeView.GetBarColor();
			DefaultMaxTrackColor = nativeView.GetBackgroundColor();
			DefaultThumbColor = nativeView.GetHandlerColor();
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

			handler.TypedNativeView?.UpdateMinimumTrackColor(slider, DefaultMinTrackColor);
		}

		public static void MapMaximumTrackColor(SliderHandler handler, ISlider slider)
		{
			ViewHandler.CheckParameters(handler, slider);

			handler.TypedNativeView?.UpdateMaximumTrackColor(slider, DefaultMaxTrackColor);
		}

		public static void MapThumbColor(SliderHandler handler, ISlider slider)
		{
			ViewHandler.CheckParameters(handler, slider);

			handler.TypedNativeView?.UpdateThumbColor(slider, DefaultThumbColor);
		}

		void OnControlValueChanged(object sender, EventArgs eventArgs)
		{
			if (TypedNativeView == null || VirtualView == null)
				return;

			VirtualView.Value = TypedNativeView.Value;
		}

		void OnDragStarted(object sender, EventArgs e)
		{
			VirtualView?.DragStarted();
		}

		void OnDragStopped(object sender, EventArgs e)
		{
			VirtualView?.DragCompleted();
		}
	}
}