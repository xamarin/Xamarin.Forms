using System;
using Tizen.UIExtensions.ElmSharp;
using EColor = ElmSharp.Color;
using ESlider = ElmSharp.Slider;

namespace Microsoft.Maui.Handlers
{
	public partial class SliderHandler : AbstractViewHandler<ISlider, ESlider>
	{
		static EColor? DefaultMinTrackColor;
		static EColor? DefaultMaxTrackColor;
		static EColor? DefaultThumbColor;

		protected override ESlider CreateNativeView() => new ESlider(NativeParent);

		protected override void ConnectHandler(ESlider nativeView)
		{
			nativeView.ValueChanged += OnControlValueChanged;
			nativeView.DragStarted += OnDragStarted;
			nativeView.DragStopped += OnDragStopped;
		}

		protected override void DisconnectHandler(ESlider nativeView)
		{
			nativeView.ValueChanged -= OnControlValueChanged;
			nativeView.DragStarted -= OnDragStarted;
			nativeView.DragStopped -= OnDragStopped;
		}

		protected override void SetupDefaults(ESlider nativeView)
		{
			DefaultMinTrackColor = nativeView.GetBarColor();
			DefaultMaxTrackColor = nativeView.GetBackgroundColor();
			DefaultThumbColor = nativeView.GetHandlerColor();
		}

		public static void MapMinimum(SliderHandler handler, ISlider slider)
		{
			handler.TypedNativeView?.UpdateMinimum(slider);
		}

		public static void MapMaximum(SliderHandler handler, ISlider slider)
		{
			handler.TypedNativeView?.UpdateMaximum(slider);
		}


		public static void MapValue(SliderHandler handler, ISlider slider)
		{
			handler.TypedNativeView?.UpdateValue(slider);
		}

		public static void MapMinimumTrackColor(SliderHandler handler, ISlider slider)
		{
			handler.TypedNativeView?.UpdateMinimumTrackColor(slider, DefaultMinTrackColor);
		}

		public static void MapMaximumTrackColor(SliderHandler handler, ISlider slider)
		{
			handler.TypedNativeView?.UpdateMaximumTrackColor(slider, DefaultMaxTrackColor);
		}

		public static void MapThumbColor(SliderHandler handler, ISlider slider)
		{

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